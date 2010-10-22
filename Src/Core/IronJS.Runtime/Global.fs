namespace IronJS.Native

open System
open IronJS
open IronJS.Compiler
open IronJS.Aliases
open IronJS.Api.Extensions

module Global =

  //15.1.2.1
  let eval (target:Compiler.EvalTarget) =
    match target.Target.Tag with
    | TypeTags.String ->
      let tree = 
        Ast.LocalScope(
         {Ast.Scope.New with Closures=target.Closures}, 
          Ast.Parsers.Ecma3.parse target.Function.Env target.Target.String)

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
          target.LocalScope,
          target.ClosureScope,
          target.DynamicScope))

    | _ -> target.Target

  //15.1.2.2
  let parseInt (str:IjsStr) =
    Utils.boxNumber (double (System.Int32.Parse(str)))

  //15.1.2.3
  let parseFloat (str:IjsStr) =
    Utils.boxNumber (Api.TypeConverter.toNumber str)

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
    env.Globals <- Api.Environment.createObject env
    env.Globals.put("NaN", NaN) //15.1.1.1
    env.Globals.put("Infinity", PosInf) //15.1.1.2
    env.Globals.put("undefined", Undefined.Instance) //15.1.1.3

    //15.1.2.1
    env.Globals.put("eval", 
      (Api.HostFunction.create
        env (new Func<Compiler.EvalTarget, IjsBox>(eval))))

    //15.1.2.2
    env.Globals.put("parseFloat",
      (Api.HostFunction.create
        env (new Func<IjsStr, IjsBox>(parseFloat))))
    
    //15.1.2.3
    env.Globals.put("parseInt", 
      (Api.HostFunction.create
        env (new Func<IjsStr, IjsBox>(parseInt))))
    
    //15.1.2.4
    env.Globals.put("isNaN", 
      (Api.HostFunction.create
        env (new Func<IjsNum, IjsBox>(isNaN))))
    
    //15.1.2.5
    env.Globals.put("isFinite", 
      (Api.HostFunction.create
        env (new Func<IjsNum, IjsBox>(isFinite))))