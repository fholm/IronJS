namespace IronJS.Native

open System
open IronJS

module Function =

  let private constructor' (f:FO) (_:CO) (args:Args) : FO =
      Utils.trapSyntaxError f.Env (fun () ->
        let args, body = 
          if args.Length = 0 then 
            "", ""

          else 

            let funcArgs = 
              args 
              |> Seq.take (args.Length-1) 
              |> Seq.map TypeConverter.ToString
              |> String.concat ", "
              
            let body = 
              args.[args.Length-1] 
              |> TypeConverter.ToString

            funcArgs, body

        let source = sprintf "(function(){ return function(%s){%s}; })();" args body
        let ast = source |> Compiler.Parser.parseString f.Env
        let compiled = Compiler.Core.compileAsGlobal f.Env ast
        (compiled.DynamicInvoke(f, f.Env.Globals) |> BoxingUtils.ClrBox) :?> FunctionObject
      )

  let private prototype (f:FO) _ =
    Undefined.Boxed
    
  let toString (toString:FO) (o:CO) =
    let f = o.CastTo<FO>()
    match f.Env.FunctionSourceStrings.TryGetValue f.FunctionId with
    | true, value -> value
    | _ -> "function() { [native code] }"

  let private getThisObject (env:Env) (this:BV) =
    match this.Tag with
    | TypeTags.Clr -> env.Globals
    | TypeTags.Undefined -> env.Globals
    | _ -> TC.ToObject(env, this)
      
  let apply (apply:FO) (func:CO) (this:BV) (args:CO) : BV =
    let f = func.CastTo<FO>()

    let args = 
      if args <> null then
        
        let args = args.CastTo<AO>()
        let getIndex i = args.Get(uint32 i)

        Seq.init (int args.Length) getIndex
        |> Seq.cast<obj>
        |> Array.ofSeq

      else
        Array.zeroCreate 0

    let this = this |> getThisObject apply.Env
    let argTypes = DelegateCache.addInternalArgs [for a in args -> a.GetType()]
    let type' = DelegateCache.getDelegate argTypes
    let args = Array.append [|func :> obj; this :> obj|] args
    let compiled = f.Compiler f type'

    Utils.trapSyntaxError f.Env (fun () -> 
      compiled.DynamicInvoke(args) |> BoxingUtils.JsBox)
 
  let call (_:FO) (func:CO) (this:BV) (args:ClrArgs) : BV =
    let f = func.CastTo<FO>()
    let argTypes = DelegateCache.addInternalArgs [for a in args -> a.GetType()]
    let type' = DelegateCache.getDelegate argTypes
    let this = this |> getThisObject f.Env
    let args = Array.append [|f :> obj; this :> obj|] args

    Utils.trapSyntaxError f.Env (fun () -> 
      (f.Compiler f type').DynamicInvoke args |> BoxingUtils.JsBox)

  let setupConstructor (env:Env) =
    let ctor = new Func<FO, CO, Args, FO>(constructor')
    let ctor = Utils.createHostFunction env ctor
      
    ctor.Prototype <- env.Prototypes.Function
    ctor.ConstructorMode <- ConstructorModes.Host
    ctor.Put("prototype", env.Prototypes.Function, DescriptorAttrs.Immutable)

    env.Globals.Put("Function", ctor, DescriptorAttrs.DontEnum)
    env.Constructors <- {env.Constructors with Function = ctor}
    
  let createPrototype (env:Env) ownPrototype =
    let prototype = new Func<FO, CO, BV>(prototype)
    let prototype = Utils.createHostFunction env prototype
    prototype.Prototype <- ownPrototype
    prototype
      
  let setupPrototype (env:Env) =
    let attrs = DescriptorAttrs.DontEnum

    let call = new Func<FO, CO, BV, ClrArgs, BV>(call)
    let call = Utils.createHostFunction env call
    env.Prototypes.Function.Put("call", call, attrs)

    let apply = new Func<FO, CO, BV, CO, BV>(apply)
    let apply = Utils.createHostFunction env apply
    env.Prototypes.Function.Put("apply", apply, attrs)
    
    let toString = new Func<FO, CO, string>(toString)
    let toString = Utils.createHostFunction env toString
    env.Prototypes.Function.Put("toString", toString, attrs)

    env.Prototypes.Function.Put("constructor", env.Constructors.Function, attrs)