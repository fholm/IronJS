module IronJS.Runtime.Environment

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Runtime

open System.Dynamic
open System.Collections.Generic

(**)
type private DelegateCell(func:Function, delegateType:ClrType) =
  let hashCode = 37 * (37 * func.AstId + func.ClosureId) + delegateType.GetHashCode()

  member self.AstId = func.AstId
  member self.ClosureId = func.ClosureId
  member self.DelegateType = delegateType

  override self.GetHashCode() = hashCode
  override self.Equals obj = 
    match obj with
    | :? DelegateCell as cell -> 
         self.AstId = cell.AstId
      && self.ClosureId = self.ClosureId
      && self.DelegateType = self.DelegateType
    | _ -> false

(*The currently executing environment*)
type Environment (scopeAnalyzer:Ast.Scope -> ClrType -> ClrType list -> Ast.Scope, 
                  exprGenerator:IEnvironment -> ClrType -> ClrType -> Ast.Scope -> Ast.Node -> EtLambda) =

  inherit IEnvironment()

  let astMap = new Dict<int, Ast.Scope * Ast.Node>()
  let closureMap = new SafeDict<ClrType, int>()
  let delegateCache = new SafeDict<DelegateCell, System.Delegate>()

  //Implementation of IEnvironment.GetDelegate
  override x.GetDelegate func delegateType types =
    let cell = new DelegateCell(func, delegateType)
    let success, delegate' = delegateCache.TryGetValue(cell)
    if success then delegate'
    else
      let scope, body = astMap.[func.AstId]
      let closureType = func.Closure.GetType()
      let lambdaExpr  = exprGenerator x delegateType closureType (scopeAnalyzer scope closureType types) body
      delegateCache.[cell] <- lambdaExpr.Compile()
      delegateCache.[cell]
      
  //Implementation of IEnvironment.AstMap
  override x.AstMap = astMap
  
  //Implementation of IEnvironment.GetClosureId
  override x.GetClosureId clrType = 
    let success, id = closureMap.TryGetValue clrType
    if success 
      then id
      else closureMap.GetOrAdd(clrType, closureMap.Count)

  //Static
  static member Create sa eg =
    let env = new Environment(sa, eg)
    env.Globals <- new Object(env)
    env