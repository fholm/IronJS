namespace IronJS.Native

open System
open IronJS

(*
//  This module implements the javascript Function object, its prototype, functions and properties.
//
//  DONE:
//  15.3.1.1 Function (p1, p2, … , pn, body) 
//  15.3.2.1 new Function (p1, p2, … , pn, body) 
//  15.3.3.1 Function.prototype 
//  15.3.4.1 Function.prototype.constructor 
//  15.3.4.2 Function.prototype.toString ( )
//  15.3.4.3 Function.prototype.apply (thisArg, argArray) 
//  15.3.4.4 Function.prototype.call (thisArg [ , arg1 [ , arg2, … ] ] ) 
*)

module Function =

  let private constructor' (f:FunctionObject) (_:CommonObject) (args:BoxedValue array) : FunctionObject =
      let args, body = 
        if args.Length = 0 then "", ""
        else 
          let body = args.[args.Length-1] |> TypeConverter.ToString
          let args = 
            args 
            |> Seq.take (args.Length-1) 
            |> Seq.map TypeConverter.ToString
            |> String.concat ", "
          args, body

      let func = sprintf "(function(){ return function(%s){%s}; })();" args body
      let tree = Parsers.Ecma3.parseGlobalSource f.Env func
      let analyzed = Ast.Analyzers.applyDefault tree None
      let compiled = Compiler.Core.compileAsGlobal f.Env analyzed
      (compiled.DynamicInvoke(f, f.Env.Globals) |> Utils.clrBox) :?> FunctionObject

  let private prototype (f:FunctionObject) _ =
    Undefined.Boxed
    
  let toString (toString:FunctionObject) (o:CommonObject) =
    let f = o.CastTo<FO>()
    match f.Env.FunctionSourceStrings.TryGetValue f.FunctionId with
    | true, value -> value
    | _ -> "function() { [native code] }"
      
  let apply (apply:FunctionObject) (func:CommonObject) (this:CommonObject) (args:CommonObject) : BoxedValue =
    let f = func.CastTo<FO>()
    let args = args.CastTo<AO>()
    let getIndex i = args.Get(uint32 i)

    let args =
      Seq.init (int args.Length) getIndex
      |> Seq.cast<obj>
      |> Array.ofSeq

    let argTypes = Utils.addInternalArgs [for a in args -> a.GetType()]
    let type' = Utils.createDelegate argTypes
    let args = Array.append [|func :> obj; this :> obj|] args
    let compiled = f.Compiler f type'
    compiled.DynamicInvoke(args) |> Utils.jsBox
 
  let call (_:FunctionObject) (func:CommonObject) (this:CommonObject) (args:obj array) : BoxedValue =
    let f = func.CastTo<FO>()
    let argTypes = Utils.addInternalArgs [for a in args -> a.GetType()]
    let type' = Utils.createDelegate argTypes
    let args = Array.append [|func :> obj; this :> obj|] args
    (f.Compiler f type').DynamicInvoke args |> Utils.jsBox

  let setupConstructor (env:Environment) =
    let ctor = new Func<FunctionObject, CommonObject, BoxedValue array, FunctionObject>(constructor')
    let ctor = Utils.createHostFunction env ctor
      
    ctor.Prototype <- env.Prototypes.Function
    ctor.ConstructorMode <- ConstructorModes.Host
    ctor.Put("prototype", env.Prototypes.Function, DescriptorAttrs.Immutable)

    env.Globals.Put("Function", ctor)
    env.Constructors <- {env.Constructors with Function = ctor}
    
  let createPrototype (env:Environment) ownPrototype =
    let prototype = new Func<FunctionObject, CommonObject, BoxedValue>(prototype)
    let prototype = Utils.createHostFunction env prototype
    prototype.Prototype <- ownPrototype
    prototype
      
  let setupPrototype (env:Environment) =
    let attrs = DescriptorAttrs.DontEnum

    let call = new Func<FunctionObject, CommonObject, CommonObject, obj array, BoxedValue>(call)
    let call = Utils.createHostFunction env call
    env.Prototypes.Function.Put("call", call, attrs)

    let apply = new Func<FunctionObject, CommonObject, CommonObject, CommonObject, BoxedValue>(apply)
    let apply = Utils.createHostFunction env apply
    env.Prototypes.Function.Put("apply", apply, attrs)
    
    let toString = new Func<FunctionObject, CommonObject, string>(toString)
    let toString = Utils.createHostFunction env toString
    env.Prototypes.Function.Put("toString", toString, attrs)

    env.Prototypes.Function.Put("constructor", env.Constructors.Function, attrs)