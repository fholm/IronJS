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

(*The currently executing environment*)
type Environment (astGenerator:AstGenFunc, scopeAnalyzer:AnalyzeFunc, exprGenerator:ExprGenFunc) =
  let jitCache = new Dictionary<Ast.Types.Node, CacheCell>()

  member self.GetCachedDelegate (ast:Ast.Types.Node) (types:ClrType list) =
    //Helper function
    let rec scan types (cell:CacheCell) =
      match types with
      | [] -> 
        if cell.Func = null 
          then None 
          else Some(cell.Func)
      | typ::xsTypes -> 
        let next =
          if   typ = Constants.clrDouble then cell.NextDouble
          elif typ = Constants.clrString then cell.NextString
          elif typ = typedefof<Object>   then cell.NextObject
          else cell.NextDynamic
        match next with
        | None -> None
        | Some(cell) -> scan xsTypes cell

    //Do the scan
    let success, cell = jitCache.TryGetValue(ast)
    if success then scan types cell else None

  member self.StoreCachedDelegate (ast:Ast.Types.Node) (types:ClrType list) (func:System.Delegate) =
    //Helper function
    let rec insert types (cell:CacheCell) =
      match types with
      | [] -> { cell with Func = func }
      | typ::xsTypes ->
          if typ = Constants.clrDouble then 
            match cell.NextDouble with
            | None -> { cell with NextDouble = Some(insert xsTypes emptyCacheCell) }
            | Some(next) -> { cell with NextDouble = Some(insert xsTypes next) }
          elif typ = Constants.clrString then
            match cell.NextString with
            | None -> { cell with NextString = Some(insert xsTypes emptyCacheCell) }
            | Some(next) -> { cell with NextString = Some(insert xsTypes next) }
          elif typ = typedefof<Object> then
            match cell.NextObject with
            | None -> { cell with NextObject = Some(insert xsTypes emptyCacheCell) }
            | Some(next) -> { cell with NextObject = Some(insert xsTypes next) }
          else
            match cell.NextDynamic with
            | None -> { cell with NextDynamic = Some(insert xsTypes emptyCacheCell) }
            | Some(next) -> { cell with NextDynamic = Some(insert xsTypes next) }
    
    //Do the insert
    let success, cell = jitCache.TryGetValue(ast)
    jitCache.[ast] <- if success then insert types cell else insert types emptyCacheCell
    jitCache.[ast].Func

  member self.Compile (ast:Ast.Types.Node) (closureType:ClrType) (types:ClrType list) =
    match ast with
    | Ast.Types.Node.Function(scope, body) ->
      

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

(*Class representing the javascript Undefined type*)
type Undefined() =
  static let instance = Undefined()
  static member Instance with get() = instance
  static member InstanceExpr with get() = constant instance