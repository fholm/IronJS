namespace IronJS.Native

open System
open IronJS
open IronJS.DescriptorAttrs
open IronJS.Support.CustomOperators

///
module internal Function =

  ///
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
        let ast, scopeData = source |> Compiler.Parser.parseString f.Env
        scopeData |> Compiler.Analyzer.analyzeScopeChain

        let compiled = Compiler.Core.compileGlobal f.Env ast
        (compiled.DynamicInvoke(f, f.Env.Globals) |> BoxingUtils.ClrBox) :?> FunctionObject
      )

  ///
  let setup (env:Env) =
    let ctor = new Func<FO, CO, Args, FO>(constructor')
    let ctor = Utils.createHostFunction env ctor
      
    ctor.Prototype <- env.Prototypes.Function
    ctor.Put("prototype", env.Prototypes.Function, DescriptorAttrs.Immutable)

    env.Globals.Put("Function", ctor, DescriptorAttrs.DontEnum)
    env.Constructors <- {env.Constructors with Function = ctor}

  module Prototype = 

    ///
    let private prototype (f:FO) _ =
      Undefined.Boxed

    ///
    let private toString (toString:FO) (o:CO) =
      let f = o.CastTo<FO>()
      match f.MetaData.Source with
      | None -> "function() { [native code] }"
      | Some source -> source

    ///
    let private getThisObject (env:Env) (this:BV) =
      match this.Tag with
      | TypeTags.Clr -> env.Globals
      | TypeTags.Undefined -> env.Globals
      | _ -> TC.ToObject(env, this)
      
    ///
    let private apply (_:FO) (func:CO) (this:BV) (args:CO) : BV =
      let func = func.CastTo<FO>()

      let args = 
        if args <> null then
        
          if args :? AO then
        
            let args = args.CastTo<AO>()
            let getIndex i = args.Get(uint32 i)

            Seq.init (int args.Length) getIndex
            |> Seq.cast<obj>
            |> Array.ofSeq
          
          elif args :? ArgumentsObject then
          
            let args = args.CastTo<ArgumentsObject>()
            let getIndex i = args.Get(uint32 i)
            let length = args.GetT<double>("length") |> int32

            Seq.init length getIndex
            |> Seq.cast<obj>
            |> Array.ofSeq

          else
            func.Env.RaiseTypeError()

        else
          Array.zeroCreate 0

      let this = this |> getThisObject func.Env
      let type' = DelegateCache.getDelegate [for a in args -> a.GetType()]
      let args = Array.append [|func :> obj; this :> obj|] args
      let compiled = func.MetaData.GetDelegate(func, type')

      Utils.trapSyntaxError func.Env (fun () -> 
        compiled.DynamicInvoke(args) |> BoxingUtils.JsBox)
 
    ///
    let private call (_:FO) (func:CO) (args:Args) : BV =
      let func = func.CastTo<FO>()

      let this = 
        if args.Length > 0 
          then args.[0] 
          else func.Env.RaiseTypeError()
          
      let args = args $ Seq.skip 1 $ Seq.map TC.ToClrObject $ Seq.toArray
      let argTypes = args $ Array.map TypeUtils.getType

      let type' = argTypes $ DelegateUtils.getCallSiteDelegate
      let this = this $ getThisObject func .Env

      let args = Array.append [|func  :> obj; this :> obj|] args

      Utils.trapSyntaxError func.Env (fun () -> 
        let func = func.MetaData.GetDelegate(func, type')
        func.DynamicInvoke(args) |> BoxingUtils.JsBox
      )
    
    ///
    let create (env:Env) ownPrototype =
      let prototype = 
        Function(prototype) $ Utils.createFunction env (Some 0)

      prototype.Prototype <- ownPrototype
      prototype
      
    ///
    let setup (env:Env) =
      
      //
      let call = VariadicFunction(call) $ Utils.createFunction env (Some 1)
      env.Prototypes.Function.Put("call", call, DontEnum)

      //
      let apply = Function<BV, CO>(apply) $ Utils.createFunction env (Some 2)
      env.Prototypes.Function.Put("apply", apply, DontEnum)
    
      //
      let toString = FunctionReturn<string>(toString) $ Utils.createFunction env (Some 0)
      env.Prototypes.Function.Put("toString", toString, DontEnum)

      //
      env.Prototypes.Function.Put("constructor", env.Constructors.Function, DontEnum)