module IronJS.Runtime.Core

open IronJS
open IronJS.Utils
open IronJS.Tools.Expr
open System.Dynamic
open System.Collections.Generic

type private AstGenFunc = AstTree -> Ast.Types.Scopes -> Ast.Types.Node
type private AnalyzeFunc = Ast.Types.Scope -> ClrType list -> Ast.Types.Scope
type private ExprGenFunc = ClrType -> Ast.Types.Scope -> Ast.Types.Node -> EtLambda

(*Cache Cell used by Environment class to cache compiled delegates*)
type CacheCell = {
  NextDouble  : CacheCell option
  NextString  : CacheCell option
  NextObject  : CacheCell option
  NextDynamic : CacheCell option
  Func : System.Delegate
}

let emptyCacheCell = {
  NextDouble = None
  NextString = None
  NextObject = None
  NextDynamic = None
  Func = null
}

(*Class representing the javascript Undefined type*)
type Undefined() =
  static let instance = Undefined()
  static member Instance with get() = instance
  static member InstanceExpr with get() = constant instance

(**)
type DelegateCell(ast:Ast.Types.Node, closureType:ClrType, types:ClrType list) =
  
  let rec calculateHashAndTypes types (hash:int ref) = 
    match types with
    | [] -> []
    | x::xsTypes ->
      if x = Constants.clrDouble then
        hash := (37 * !hash + Constants.clrDoubleHashCode)
        Constants.clrDouble :: calculateHashAndTypes xsTypes hash
        
      elif x = Constants.clrString then
        hash := (37 * !hash + Constants.clrStringHashCode)
        Constants.clrString :: calculateHashAndTypes xsTypes hash
        
      elif x = typeof<Object> then
        hash := (37 * !hash + ((typeof<Object>).GetHashCode()))
        typeof<Object> :: calculateHashAndTypes xsTypes hash

      else
        hash := (37 * !hash + Constants.clrDynamicHashCode)
        Constants.clrDynamic :: calculateHashAndTypes xsTypes hash

  let rec compareTypes a b =
    match a with
    | [] -> true
    | xA::xsA ->
      match b with
      | xB::xsB -> if xA = xB then compareTypes xsA xsB else false
      | _ -> failwith "Should never happen"
  
  let hashRef = ref (37 * closureType.GetHashCode() + ast.GetHashCode())
  let uniformTypes = calculateHashAndTypes types hashRef
  let hashCode = !hashRef

  member self.Types = uniformTypes
  member self.ClosureType = closureType

  override self.GetHashCode() = 
    hashCode

  override self.Equals obj = 
    match obj with
    | :? DelegateCell as cell ->  
      if cell.Types.Length = self.Types.Length then
        if cell.ClosureType = self.ClosureType then
          compareTypes self.Types cell.Types
        else
          false
      else
        false
    | _ -> false

(*The currently executing environment*)
and Environment (astGenerator:AstGenFunc, scopeAnalyzer:AnalyzeFunc, exprGenerator:ExprGenFunc) =
  let jitCache = new Dictionary<DelegateCell, System.Delegate>()

  member self.GetCachedDelegate (ast:Ast.Types.Node) (closureType:ClrType) (types:ClrType list) =
    let cell = new DelegateCell(ast, closureType, types)
    let success, func = jitCache.TryGetValue(cell)
    if success then Some(func) else None

  member self.CacheCompiledDelegate (ast:Ast.Types.Node) (closureType:ClrType) (types:ClrType list) (func:System.Delegate) =
    let cell = new DelegateCell(ast, closureType, types)
    jitCache.[cell] <- func
    func

  member self.Compile (ast:Ast.Types.Node) (closureType:ClrType) (types:ClrType list) =
    match ast with
    | Ast.Types.Node.Function(scope, body) -> (exprGenerator closureType (scopeAnalyzer scope types) body).Compile()
    | _ -> failwith "Can only compile Ast.Types.Node.Function"

(*Class representing a Javascript native object*)
and Object(env:Environment) =
  let properties = new Dictionary<string, obj>();
  member self.Get name = properties.[name]
  member self.Set name (value:obj) = properties.[name] <- value
  member self.Environment = env

(*DLR meta object for the above Object class*)
and ObjectMeta(expr, jsObj:Object) =
  inherit System.Dynamic.DynamicMetaObject(expr, Restrict.Empty, jsObj)

let objectTypeDef = typedefof<Object>