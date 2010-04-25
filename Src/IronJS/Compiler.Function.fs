namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler
open IronJS.Parser

module Function =

  module private Closure =
    (*Resolves all item expressions for a closure*)
    let private resolveItems ctx (scope:Ast.Scope) =
      let expr name =
        if ctx.Scope.Locals.ContainsKey name
          then Variables.Local.expr ctx name   // Local closed over variable
          else Variables.Closure.expr ctx name // Variable that is a closure in parent also

      scope.Closure 
        |> Map.toSeq
        |> Seq.sortBy (fun pair -> (snd pair).Index)
        |> Seq.map (fun pair -> expr (fst pair))
        |> List.ofSeq

    (*Creates a closure type*)
    let private createType ctx (scope:Ast.Scope) =
      let clrType name =
        if ctx.Scope.Locals.ContainsKey name
          then Variables.Local.clrType ctx name
          else Variables.Closure.clrType ctx name

      Runtime.Closures.createClosureType (
        scope.Closure 
          |> Map.toSeq
          |> Seq.fold (fun state pair -> (clrType (fst pair), (snd pair).Index) :: state) [] 
          |> Seq.sortBy (fun pair -> snd pair)
          |> Seq.map (fun pair -> fst pair)
      )

    (*Creates a new closure type and expression to create an instance of that type*)
    let internal create (ctx:Context) (scope:Ast.Scope) =
      let scopesExpr = if scope.InLocalDS 
                         then let args = [ctx.Closure :> Et; ctx.LocalScopes :> Et; Dlr.Expr.constant ctx.Scope.ScopeLevel]
                              Dlr.Expr.callStaticT<Runtime.Helpers.Closures> "BuildScopes" args
                         else ctx.ClosureScopes
  
      let closureType = createType ctx scope
      let dynScopesExpr = Dlr.Expr.newArgs typeof<Runtime.Scope ResizeArray> [ctx.ClosureScopes]
      Dlr.Expr.newArgs closureType (scopesExpr :: resolveItems ctx scope)

  (*Defines a new function*)
  let internal definition (ctx:Context) astId =
    let scope, _ = ctx.Env.AstMap.[astId]
    let closureExpr = Closure.create ctx scope
    let functionArgs = [
      Dlr.Expr.constant astId
      Dlr.Expr.constant (ctx.Env.GetClosureId (closureExpr.Type))
      closureExpr
      ctx.Environment
    ]

    Dlr.Expr.newArgs Runtime.Function.TypeDef functionArgs

  (*Invokes a function*)
  let internal invoke (ctx:Context) target args (returnBox:Et) =
    //TODO: Wow this is ugly, redo.
    let targetExpr = ctx.Builder ctx target
    let argExprs = [for arg in args -> ctx.Builder ctx arg]

    ctx.TemporaryTypes.Clear()

    let funcType = Runtime.Delegate.getFor (List.map (fun (x:Et) -> x.Type) argExprs)
    let cacheType = typedefof<Runtime.InvokeCache<_>>.MakeGenericType(funcType)
    let cacheInst = cacheType.GetConstructors().[0].Invoke([|[for x in argExprs -> x.Type]|])
    let cacheConst = Expr.constant cacheInst

    let tmp = Expr.paramT<Runtime.Function> "~invokeTmp"

    let checkAstId = Expr.Logic.notEq (Expr.field tmp "AstId") (Expr.field cacheConst "AstId")
    let checkClosureId = Expr.Logic.notEq (Expr.field tmp "ClosureId") (Expr.field cacheConst "ClosureId")
    let body = 
      [
        (Expr.Flow.if'
          (Expr.Logic.or' checkAstId checkClosureId)
          (Expr.block[
            (Expr.call cacheConst "Update" [tmp])
            (Expr.assign (Expr.field cacheConst "AstId") (Expr.field tmp "AstId"))
            (Expr.assign (Expr.field cacheConst "ClosureId") (Expr.field tmp "ClosureId"))
          ])
        )
        (Expr.invoke (Expr.field cacheConst "Delegate") (tmp:>Et :: (ctx.Globals:>Et) :: argExprs))
      ]

    if targetExpr.Type = typeof<Runtime.Box> then

      if targetExpr :? EtParam || targetExpr :? System.Linq.Expressions.IndexExpression then
        (Expr.Flow.ifElse
          (Expr.Logic.eq (Expr.field targetExpr "Type") (Expr.constant TypeCodes.function'))
          (Expr.blockWithLocals [tmp] (Expr.assign tmp (Utils.Box.fieldByTypeCode targetExpr TypeCodes.function') :: body))
          (Expr.empty)
        )
      else
        failwith "Not supported"

    elif targetExpr.Type = typeof<Runtime.Function> then
      Expr.blockWithLocals [tmp] (Expr.assign tmp targetExpr :: body)

    else
      failwith "Can't call non-function"
