module IronJS.Runtime.Environment

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Runtime

open System.Dynamic
open System.Collections.Generic

type private AnalyzeFunc = Ast.Scope -> ClrType -> ClrType list -> Ast.Scope
type private ExprGenFunc = IEnvironment -> ClrType -> Ast.Scope -> Ast.Node -> (EtLambda * ClrType list)

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
      
    elif x = Runtime.Object.TypeDef then
      hash := (37 * !hash + Runtime.Object.TypeDefHashCode)
      Runtime.Object.TypeDef :: calculateHashAndTypes xsTypes hash

    elif x = Runtime.Function.TypeDef then
      hash := (37 * !hash + Runtime.Function.TypeDefHashCode)
      Runtime.Function.TypeDef :: calculateHashAndTypes xsTypes hash

    else
      hash := (37 * !hash + Constants.clrDynamicHashCode)
      Constants.clrDynamic :: calculateHashAndTypes xsTypes hash

(**)
let private compareTypes (a:'a list) (b:'a list) =
  if not (a.Length = b.Length) 
    then  false
    else  let rec compareTypes' a b =
            match a with
            | []      -> true
            | xA::xsA ->
              match b with
              | xB::xsB -> if xA = xB then compareTypes' xsA xsB else false
              | _       -> failwith "Should never happen"

          compareTypes' a b

(**)
type DelegateCell(ast:Ast.Node, closureType:ClrType, types:ClrType list) =
  let hashRef = ref (37 * closureType.GetHashCode() + ast.GetHashCode())
  let uniformTypes = calculateHashAndTypes types hashRef
  let hashCode = !hashRef

  member self.Ast = ast
  member self.Types = uniformTypes
  member self.ClosureType = closureType

  override self.GetHashCode() = hashCode
  override self.Equals obj = 
    match obj with
    | :? DelegateCell as cell -> 
      if cell.Ast = self.Ast && cell.ClosureType = self.ClosureType 
        then compareTypes cell.Types self.Types
        else false
    | _ -> false

(*The currently executing environment*)
and Environment (scopeAnalyzer:AnalyzeFunc, exprGenerator:ExprGenFunc) =
  inherit IEnvironment()

  let astMap = new Dict<int, Ast.Scope * Ast.Node>()
  let closureMap = new Dict<ClrType, int>()
  let jitCache = new Dict<DelegateCell, System.Delegate * ClrType list>()

  //Implementation of IEnvironment.GetDelegate
  override x.GetDelegate ast closureType types =
    let cell = new DelegateCell(ast, closureType, types)
    match x.GetCachedDelegate cell with
    | Some(func) -> func
    | None -> x.CacheCompiledDelegate cell (x.Compile ast closureType types)

  override x.AstMap = astMap
  override x.GetClosureId clrType = 
    let success, id = closureMap.TryGetValue clrType
    if success then id
    else
      closureMap.Add(clrType, closureMap.Count)
      closureMap.Count-1

  //Private members
  member private self.GetCachedDelegate cell =
    let success, func = jitCache.TryGetValue(cell)
    if success then Some(func) else None

  member private self.CacheCompiledDelegate cell func =
    jitCache.[cell] <- func
    func

  member private self.Compile ast closureType types =
    match ast with
    | Ast.Node.Function(astId) -> 
      let scope, body = astMap.[astId]
      let lambda, paramTypes = exprGenerator self closureType (scopeAnalyzer scope closureType types) body
      lambda.Compile(), paramTypes

    | _ -> failwith "Can only compile Ast.Types.Node.Function"

  static member Create sa eg =
    let env = new Environment(sa, eg)
    (env :> IEnvironment).Globals <- new Object(env)
    env