namespace IronJS.Native

open System
open IronJS
open IronJS.Api.Extensions
open IronJS.Utils.Patterns

module Function =

  //----------------------------------------------------------------------------
  // 15.3.2 Function
  let private constructor' (f:IjsFunc) (_:IjsObj) (args:IjsBox array) : IjsFunc =
      let args, body = 
        if args.Length = 0 then "", ""
        else 
          let body = args.[args.Length-1] |> Api.TypeConverter.toString
          let args = 
            args 
            |> Seq.take (args.Length-1) 
            |> Seq.map Api.TypeConverter.toString
            |> String.concat ", "
          args, body

      let func = sprintf "(function(){ return function(%s){%s}; })();" args body
      let tree = Parsers.Ecma3.parseGlobalSource f.Env func
      let analyzed = Ast.Analyzers.applyDefault tree None
      let compiled = Compiler.Core.compileAsGlobal f.Env analyzed
      (compiled.DynamicInvoke(f, f.Env.Globals) |> Utils.unboxObj) :?> IjsFunc

  //----------------------------------------------------------------------------
  // 15.3.4 Function.prototype
  let private prototype (f:IjsFunc) _ =
    Utils.BoxedConstants.undefined
    
  //----------------------------------------------------------------------------
  // 15.3.4.2 Function.prototype.toString
  let toString (toString:IjsFunc) (o:IjsObj) =
    if o :? IjsFunc then
      let f = o :?> IjsFunc

      match f.Env.FunctionSourceStrings.TryGetValue f.FunctionId with
      | true, value -> value
      | _ -> "function() { [native code] }"

    else
      Api.Environment.raiseTypeError toString.Env ""
      
  //----------------------------------------------------------------------------
  // 15.3.4.3 Function.prototype.apply
  let apply (apply:IjsFunc) (func:IjsObj) (this:IjsObj) (args:IjsObj) : IjsBox =
    match func with
    | IsFunction f ->
      match args with
      | IsArrayOrArguments -> 

        let args = args :?> IjsArray
        
        let getIndex i =
          args.Methods.GetIndex.Invoke(args, uint32 i)

        let args =
          Seq.init (int args.Length) getIndex
          |> Seq.cast<obj>
          |> Array.ofSeq

        let argTypes = Utils.addInternalArgs [for a in args -> a.GetType()]
        let type' = Utils.createDelegate argTypes
        let args = Array.append [|func :> obj; this :> obj|] args
        let compiled = f.Compiler.compile(f, type')
        let result = compiled.DynamicInvoke(args)
        Utils.box result

      | IsOther -> Api.Environment.raiseTypeError apply.Env ""

    | _ -> Api.Environment.raiseTypeError apply.Env ""
 
  //----------------------------------------------------------------------------
  // 15.3.4.4 Function.prototype.call
  let call (_:IjsFunc) (func:IjsObj) (this:IjsObj) (args:obj array) : IjsBox =
    match func with
    | IsFunction f ->
      let argTypes = Utils.addInternalArgs [for a in args -> a.GetType()]
      let type' = Utils.createDelegate argTypes
      let args = Array.append [|func :> obj; this :> obj|] args
      Utils.box (f.Compiler.compile(f, type').DynamicInvoke args)

    | _ -> failwith "Que?"

  //----------------------------------------------------------------------------
  let setupConstructor (env:IjsEnv) =
    let ctor = new Func<IjsFunc, IjsObj, IjsBox array, IjsFunc>(constructor')
    let ctor = Api.HostFunction.create env ctor
      
    ctor.Prototype <- env.Prototypes.Function
    ctor.ConstructorMode <- ConstructorModes.Host
    ctor.put("prototype", env.Prototypes.Function, DescriptorAttrs.Immutable)

    env.Globals.put("Function", ctor)
    env.Constructors <- {env.Constructors with Function = ctor}
    
  //----------------------------------------------------------------------------
  let createPrototype (env:IjsEnv) ownPrototype =
    let prototype = new Func<IjsFunc, IjsObj, IjsBox>(prototype)
    let prototype = Api.HostFunction.create env prototype
    prototype.Prototype <- ownPrototype
    prototype
      
  //----------------------------------------------------------------------------
  let setupPrototype (env:IjsEnv) =
    let attrs = DescriptorAttrs.DontEnum

    //Function.prototype.call
    let call = new Func<IjsFunc, IjsObj, IjsObj, obj array, IjsBox>(call)
    let call = Api.HostFunction.create env call
    env.Prototypes.Function.put("call", call, attrs)

    //Function.prototype.apply
    let apply = new Func<IjsFunc, IjsObj, IjsObj, IjsObj, IjsBox>(apply)
    let apply = Api.HostFunction.create env apply
    env.Prototypes.Function.put("apply", apply, attrs)
    
    //Function.prototype.toString
    let toString = new Func<IjsFunc, IjsObj, IjsStr>(toString)
    let toString = Api.HostFunction.create env toString
    env.Prototypes.Function.put("toString", toString, attrs)

    //Function.prototype.constructor
    env.Prototypes.Function.put("constructor", env.Constructors.Function, attrs)