namespace IronJS.Native

open System
open IronJS
open IronJS.Compiler
open IronJS.Aliases

module Global =

  //15.1.2.1
  let eval (target:Compiler.EvalTarget) =
    match target.Target.Type with
    | TypeCodes.String ->
      let tree = 
        Ast.LocalScope(
         {Ast.Scope.New with Closures=target.Closures}, 
          Ast.Parsers.Ecma3.parse target.Target.String
        )

      let levels = 
        Some(
          target.GlobalLevel, 
          target.ClosureLevel, 
          target.LocalLevel)

      let compiled = 
        Core.compile {
          Ast = Ast.applyAnalyzers tree levels
          TargetMode = TargetMode.Eval
          Delegate = None
          Environment = target.Function.Env
        }

      Utils.box (
        compiled.DynamicInvoke(
          target.Function,
          target.This,
          target.Local,
          target.ScopeChain,
          target.DynamicChain))

    | _ -> target.Target

  //15.1.2.2
  let parseInt (str:IjsStr) =
    Utils.boxDouble (double (System.Int32.Parse(str)))

  //15.1.2.3
  let parseFloat (str:IjsStr) =
    Utils.boxDouble (Api.TypeConverter.toNumber str)

  //15.1.2.4
  let isNaN (number:IjsNum) =
    Utils.boxBool (number = Double.NaN)

  //15.1.2.5
  let isFinite (number:IjsNum) =
    Utils.boxBool (
      not(
         number = Double.NaN
      || number = Double.PositiveInfinity
      || number = Double.NegativeInfinity
    ))

  //15.1.1
  let setup (env:IjsEnv) =
    //It's a g-thing.
    let g = IjsObj(env.Base_Class, env.Object_prototype, Classes.Object, 0u)

    env.Globals <- g

    //15.1.1.1
    Api.Object.putProperty(g, "NaN", 
      NaN, PropertyAttrs.DontDelete ||| PropertyAttrs.DontEnum)

    //15.1.1.2
    Api.Object.putProperty(g, "Infinity", 
      PosInf, PropertyAttrs.DontDelete ||| PropertyAttrs.DontEnum)

    //15.1.1.3
    Api.Object.putProperty(g, "undefined",
      Undefined.Instance, PropertyAttrs.DontDelete ||| PropertyAttrs.DontEnum)

    //15.1.2.1
    Api.Object.putProperty(
      g, "eval", 
      Api.DelegateFunction<_>.create(
        env, new Func<Compiler.EvalTarget, IjsBox>(eval)), PropertyAttrs.All)

    //15.1.2.3
    Api.Object.putProperty(
      g, "parseFloat", 
      Api.DelegateFunction<_>.create(
        env, new Func<IjsStr, IjsBox>(parseFloat)), PropertyAttrs.All)

    //15.1.2.3
    Api.Object.putProperty(
      g, "parseInt", 
      Api.DelegateFunction<_>.create(
        env, new Func<IjsStr, IjsBox>(parseInt)), PropertyAttrs.All)

    //15.1.2.4
    Api.Object.putProperty(
      g, "isNaN", 
      Api.DelegateFunction<_>.create(
        env, new Func<IjsNum, IjsBox>(isNaN)), PropertyAttrs.All)

    //15.1.2.5
    Api.Object.putProperty(
      g, "isFinite", 
      Api.DelegateFunction<_>.create(
        env, new Func<IjsNum, IjsBox>(isFinite)), PropertyAttrs.All)
