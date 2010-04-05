module IronJS.Runtime.Environment

open IronJS
open IronJS.Utils
open IronJS.Tools.Expr
open System.Dynamic
open System.Collections.Generic

type private AstGenFunc = AstTree -> Ast.Types.Scopes -> Ast.Types.Node
type private AnalyzeFunc = Ast.Types.Scope -> ClrType list -> Ast.Types.Scope
type private ExprGenFunc = ClrType -> Ast.Types.Scope -> Ast.Types.Node -> (EtLambda * ClrType list)

(**)
let rec private calculateHashAndTypes types (hash:int ref) = 
  match types with
  | [] -> []
  | x::xsTypes ->
    if x = Constants.clrDouble then
      hash := (37 * !hash + Constants.clrDoubleHashCode)
      Constants.clrDouble :: calculateHashAndTypes xsTypes hash
      
    elif x = Constants.clrString then
      hash := (37 * !hash + Constants.clrStringHashCode)
      Constants.clrString :: calculateHashAndTypes xsTypes hash
      
    elif x = Runtime.Core.objectTypeDef then
      hash := (37 * !hash + Runtime.Core.objectTypeDefHashCode)
      Runtime.Core.objectTypeDef :: calculateHashAndTypes xsTypes hash

    elif x = Runtime.Function.functionTypeDef then
      hash := (37 * !hash + Runtime.Function.functionTypeDefHashCode)
      Runtime.Function.functionTypeDef :: calculateHashAndTypes xsTypes hash

    else
      hash := (37 * !hash + Constants.clrDynamicHashCode)
      Constants.clrDynamic :: calculateHashAndTypes xsTypes hash

(**)
let rec private compareTypes a b =
  match a with
  | [] -> true
  | xA::xsA ->
    match b with
    | xB::xsB -> if xA = xB then compareTypes xsA xsB else false
    | _ -> failwith "Should never happen"

(**)
type DelegateCell(ast:Ast.Types.Node, closureType:ClrType, types:ClrType list) =
  let hashRef = ref (37 * closureType.GetHashCode() + ast.GetHashCode())
  let uniformTypes = calculateHashAndTypes types hashRef
  let hashCode = !hashRef

  member self.Types = uniformTypes
  member self.ClosureType = closureType

  override self.GetHashCode() = hashCode
  override self.Equals obj = 
    match obj with
    | :? DelegateCell as cell ->  
      if cell.Types.Length = self.Types.Length then
        if cell.ClosureType = self.ClosureType 
          then compareTypes self.Types cell.Types
          else false
      else
        false
    | _ -> false

(*The currently executing environment*)
and Environment (astGenerator:AstGenFunc, scopeAnalyzer:AnalyzeFunc, exprGenerator:ExprGenFunc) =
  let jitCache = new Dictionary<DelegateCell, System.Delegate * ClrType list>()

  //Implementation of IEnvironment interface
  interface Runtime.Core.IEnvironment with
    member self.GetDelegate (ast:Ast.Types.Node) (closureType:ClrType) (types:ClrType list) =
      let cell = new DelegateCell(ast, closureType, types)
      match self.GetCachedDelegate cell with
      | Some(func) -> func
      | None -> self.CacheCompiledDelegate cell (self.Compile ast closureType types)

  //Private members
  member private self.GetCachedDelegate cell =
    let success, func = jitCache.TryGetValue(cell)
    if success then Some(func) else None

  member private self.CacheCompiledDelegate cell (func) =
    jitCache.[cell] <- func
    func

  member private self.Compile (ast:Ast.Types.Node) (closureType:ClrType) (types:ClrType list) =
    match ast with
    | Ast.Types.Node.Function(scope, body) -> 
      let lambda, paramTypes = (exprGenerator closureType (scopeAnalyzer scope types) body)
      lambda.Compile(), paramTypes

    | _ -> failwith "Can only compile Ast.Types.Node.Function"