namespace IronJS.Native

open System
open IronJS
open IronJS.DescriptorAttrs
open IronJS.Support.CustomOperators

///
module internal Function =

  ///
  let private constructor' (f:FO) (_:CO) (args:Args) : FO =
    
    let makeFunction () = 
    
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
      let result = compiled.DynamicInvoke(f, f.Env.Globals) |> BoxingUtils.ClrBox
      result :?> FO

    Utils.trapSyntaxError f.Env makeFunction

  ///
  let setup (env:Env) =
    let ctor = new Func<FO, CO, Args, FO>(constructor')
    let ctor = ctor $ Utils.createConstructor env (Some 1)

    ctor.MetaData.Name <- "Function"      
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
            |> Array.ofSeq

          elif args :? ArgumentsObject then
          
            let args = args.CastTo<ArgumentsObject>()
            let getIndex i = args.Get(uint32 i)
            let length = args.GetT<double>("length") |> int32

            Seq.init length getIndex
            |> Array.ofSeq

          else
            func.Env.RaiseTypeError()

        else
          Array.zeroCreate 0

      let this = this |> getThisObject func.Env
      let type' = DelegateUtils.getCallSiteDelegate [for a in args -> a.GetType()]
      let args =
        if type' = typeof<VariadicFunction> then
          [|func :> obj; this :> obj; args :> obj |]
        else
          let args = args |> Array.map (fun a -> a:> obj)
          Array.append [|func :> obj; this :> obj|] args

      let compiled = func.MetaData.GetDelegate(func, type')

      Utils.trapSyntaxError func.Env (fun () -> 
        compiled.DynamicInvoke(args) |> BoxingUtils.JsBox)
 
    ///
    let private call (f:FO) (func:CO) (args:Args) : BV =
      let func = func.CastTo<FO>()

      let this = 
        if args.Length > 0 
          then args.[0] 
          else Undefined.Boxed

      let skip = if args.Length = 0 then 0 else 1
      let args = args |> Seq.skip skip |> Seq.toArray
      let argsClr = args |> Array.map TC.ToClrObject
      let argTypes = argsClr |> Array.map TypeUtils.getType

      let type' = argTypes $ DelegateUtils.getCallSiteDelegate
      let this = this $ getThisObject func .Env

      let args =
        if type' = typeof<VariadicFunction> then
          [|func :> obj; this :> obj; args :> obj |]
        else
          Array.append [|func :> obj; this :> obj|] argsClr

      let compiled = func.MetaData.GetDelegate(func, type')

      Utils.trapSyntaxError func.Env (fun () -> 
        compiled.DynamicInvoke(args) |> BoxingUtils.JsBox)

    ///
    let create (env:Env) ownPrototype =
      let prototype = 
        Function(prototype) $ Utils.createFunction env (Some 0)

      prototype.Prototype <- ownPrototype
      prototype
      
    ///
    let setup (env:Env) =
      //
      let proto = env.Prototypes.Function
      proto.Put("constructor", env.Constructors.Function, DontEnum)

      //
      let call = call $ Utils.createVariadicFunc env (Some 1)
      proto.Put("call", call, DontEnum)

      //
      let apply = apply $ Utils.createFunc2 env (Some 2)
      proto.Put("apply", apply, DontEnum)
    
      //
      let toString = toString $ Utils.createFunc0 env (Some 0)
      proto.Put("toString", toString, DontEnum)