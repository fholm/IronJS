namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler

module Function =

  module private Closure =
    (*Resolves all item expressions for a closure*)
    let private resolveItems ctx (scope:Ast.Scope) =
      let expr ctx name =
        if ctx.Scope.Locals.ContainsKey name
          then Variables.Local.expr ctx name   // Local closed over variable
          else Variables.Closure.expr ctx name // Variable that is a closure in parent also

      scope.Closure |> Map.toSeq
                    |> Seq.sortBy (fun pair -> (snd pair).Index)
                    |> Seq.map (fun pair -> expr ctx (fst pair))
                    |> List.ofSeq

    (*Creates a closure type*)
    let private createType ctx (scope:Ast.Scope) =
      let clrType ctx name =
        if ctx.Scope.Locals.ContainsKey name
          then Variables.Local.clrType ctx name
          else Variables.Closure.clrType ctx name

      Runtime.Closures.createClosureType (
        scope.Closure |> Map.toSeq
                      |> Seq.fold (fun state pair -> (clrType ctx (fst pair), (snd pair).Index) :: state) [] 
                      |> Seq.sortBy (fun pair -> snd pair)
                      |> Seq.map (fun pair -> fst pair)
      )

    (*Creates a new closure type and expression to create an instance of that type*)
    let internal create (ctx:Context) (scope:Ast.Scope) =
      let scopesExpr = if scope.InParentDynamicScope 
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
  let internal invoke (ctx:Context) target args =
    let targetExpr = ctx.Builder ctx target
    let argExprs = [for arg in args -> ctx.Builder ctx arg]

    ctx.TemporaryTypes.Clear()

    let types = typeof<Runtime.Function> 
                :: typeof<Runtime.Object> 
                :: List.foldBack (fun (x:Et) s -> x.Type :: s) argExprs [typeof<Dynamic>]

    let funcType = Expr.delegateType types
    let cacheType = typedefof<Runtime.InvokeCache<_>>.MakeGenericType(funcType)
    let cacheInst = cacheType.GetConstructors().[0].Invoke([|[for x in argExprs -> x.Type]|])
    let cacheConst = Expr.constant cacheInst

    let tmp, locs = if targetExpr :? EtParam then 
                      targetExpr, [] 
                    else 
                      let tmp = Expr.paramT<Runtime.Function> "~tmp"
                      tmp:>Et, [tmp]

    let checkAstId = Expr.Logical.notEq (Expr.field tmp "AstId") (Expr.field cacheConst "AstId")
    let checkClosureId = Expr.Logical.notEq (Expr.field tmp "ClosureId") (Expr.field cacheConst "ClosureId")
    let body = 
      [
        (Expr.ControlFlow.ifThen
          (Expr.Logical.orElse checkAstId checkClosureId)
          (Expr.block[
            (Expr.call cacheConst "Update" [tmp])
            (Expr.assign (Expr.field cacheConst "AstId") (Expr.field tmp "AstId"))
            (Expr.assign (Expr.field cacheConst "ClosureId") (Expr.field tmp "ClosureId"))
          ])
        )
        (Expr.invoke (Expr.field cacheConst "Delegate") (tmp :: (ctx.Globals:>Et) :: argExprs))
        (Expr.field tmp "ReturnBox")
      ]

    Expr.blockWithLocals locs (if targetExpr :? EtParam then body else Expr.assign tmp targetExpr :: body)
