namespace IronJS.Native

open System
open IronJS
open IronJS.Utils.Patterns

module Function =

  //----------------------------------------------------------------------------
  // 15.3.2 Function
  let private constructor' (f:FunctionObject) (_:CommonObject) (args:BoxedValue array) : FunctionObject =
      let args, body = 
        if args.Length = 0 then "", ""
        else 
          let body = args.[args.Length-1] |> TypeConverter2.ToString
          let args = 
            args 
            |> Seq.take (args.Length-1) 
            |> Seq.map TypeConverter2.ToString
            |> String.concat ", "
          args, body

      let func = sprintf "(function(){ return function(%s){%s}; })();" args body
      let tree = Parsers.Ecma3.parseGlobalSource f.Env func
      let analyzed = Ast.Analyzers.applyDefault tree None
      let compiled = Compiler.Core.compileAsGlobal f.Env analyzed
      (compiled.DynamicInvoke(f, f.Env.Globals) |> Utils.unboxObj) :?> FunctionObject

  //----------------------------------------------------------------------------
  // 15.3.4 Function.prototype
  let private prototype (f:FunctionObject) _ =
    Utils.BoxedConstants.undefined
    
  //----------------------------------------------------------------------------
  // 15.3.4.2 Function.prototype.toString
  let toString (toString:FunctionObject) (o:CommonObject) =
    if o :? FunctionObject then
      let f = o :?> FunctionObject

      match f.Env.FunctionSourceStrings.TryGetValue f.FunctionId with
      | true, value -> value
      | _ -> "function() { [native code] }"

    else
      toString.Env.RaiseTypeError()
      
  //----------------------------------------------------------------------------
  // 15.3.4.3 Function.prototype.apply
  let apply (apply:FunctionObject) (func:CommonObject) (this:CommonObject) (args:CommonObject) : BoxedValue =
    match func with
    | IsFunction f ->
      match args with
      | IsArray args -> 

        let getIndex i = args.Get(uint32 i)

        let args =
          Seq.init (int args.Length) getIndex
          |> Seq.cast<obj>
          |> Array.ofSeq

        let argTypes = Utils.addInternalArgs [for a in args -> a.GetType()]
        let type' = Utils.createDelegate argTypes
        let args = Array.append [|func :> obj; this :> obj|] args
        let compiled = f.Compiler.Compile(f, type')
        let result = compiled.DynamicInvoke(args)
        Utils.box result

      | _ -> apply.Env.RaiseTypeError()

    | _ -> apply.Env.RaiseTypeError()
 
  //----------------------------------------------------------------------------
  // 15.3.4.4 Function.prototype.call
  let call (_:FunctionObject) (func:CommonObject) (this:CommonObject) (args:obj array) : BoxedValue =
    match func with
    | IsFunction f ->
      let argTypes = Utils.addInternalArgs [for a in args -> a.GetType()]
      let type' = Utils.createDelegate argTypes
      let args = Array.append [|func :> obj; this :> obj|] args
      Utils.box (f.Compiler.Compile(f, type').DynamicInvoke args)

    | _ -> failwith "Que?"

  //----------------------------------------------------------------------------
  let setupConstructor (env:Environment) =
    let ctor = new Func<FunctionObject, CommonObject, BoxedValue array, FunctionObject>(constructor')
    let ctor = Api.HostFunction.create env ctor
      
    ctor.Prototype <- env.Prototypes.Function
    ctor.ConstructorMode <- ConstructorModes.Host
    ctor.Put("prototype", env.Prototypes.Function, DescriptorAttrs.Immutable)

    env.Globals.Put("Function", ctor)
    env.Constructors <- {env.Constructors with Function = ctor}
    
  //----------------------------------------------------------------------------
  let createPrototype (env:Environment) ownPrototype =
    let prototype = new Func<FunctionObject, CommonObject, BoxedValue>(prototype)
    let prototype = Api.HostFunction.create env prototype
    prototype.Prototype <- ownPrototype
    prototype
      
  //----------------------------------------------------------------------------
  let setupPrototype (env:Environment) =
    let attrs = DescriptorAttrs.DontEnum

    //Function.prototype.call
    let call = new Func<FunctionObject, CommonObject, CommonObject, obj array, BoxedValue>(call)
    let call = Api.HostFunction.create env call
    env.Prototypes.Function.Put("call", call, attrs)

    //Function.prototype.apply
    let apply = new Func<FunctionObject, CommonObject, CommonObject, CommonObject, BoxedValue>(apply)
    let apply = Api.HostFunction.create env apply
    env.Prototypes.Function.Put("apply", apply, attrs)
    
    //Function.prototype.toString
    let toString = new Func<FunctionObject, CommonObject, string>(toString)
    let toString = Api.HostFunction.create env toString
    env.Prototypes.Function.Put("toString", toString, attrs)

    //Function.prototype.constructor
    env.Prototypes.Function.Put("constructor", env.Constructors.Function, attrs)