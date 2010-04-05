module IronJS.Compiler.ExprGen

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Ast.Types
open IronJS.Compiler
open IronJS.Compiler.Types
open IronJS.Compiler.Helpers

type private Builder = Node -> Context -> Et

//Handles assignment for Global/Closure/Local
let private assign left right (ctx:Context) builder =
  match left with
  | Global(name) -> Js.Object.set ctx.Globals name (builder right ctx)
  | Local(name)  -> Js.assign (ctx.Scope.Locals.[name].Expr) (builder right ctx)
  | _ -> Expr.objDefault

let private getLocalClrType name (ctx:Context) =
  if ctx.Scope.Locals.ContainsKey name 
    then ToClr ctx.Scope.Locals.[name].UsedAs
    else failwithf "No local named '%s'" name

let private getClosureClrType name (ctx:Context) =
  if ctx.Scope.Closure.ContainsKey name 
    then ctx.Closure.Type.GetField(sprintf "Item%i" ctx.Scope.Closure.[name].Index).FieldType.GetGenericArguments().[0]
    else failwithf "No closure named '%s'" name

let private resolveClosureItems (scope:Scope) (ctx:Context ) =
  Map.fold (fun state key closure -> (key, closure.Index) :: state ) [] scope.Closure
  |> List.sortWith (fun a b -> (snd a) - (snd b))
  |> List.map (fun pair -> 
    let name = (fst pair)
    if scope.Closure.[name].IsLocalInParent
      then ctx.Scope.Locals.[name].Expr :> Et
      else Expr.field ctx.Closure (sprintf "Item%i" ctx.Scope.Closure.[name].Index) 
  )

let private resolveClosureType (scope:Scope) (ctx:Context) =
  Runtime.Closures.getClosureType (
    Map.fold (fun state key closure -> 
      let typ = if closure.IsLocalInParent 
                  then getLocalClrType key ctx 
                  else getClosureClrType key ctx

      (typ, closure.Index) :: state
    ) [] scope.Closure

    |> List.sortWith (fun a b -> (snd a) - (snd b))
    |> List.map (fun pair -> fst pair)
  )

let private func (scope:Scope) (ast:Ast.Types.Node) (ctx:Context) (builder:Builder) =
  let closureType = resolveClosureType scope ctx
  let closureExpr = Expr.newArgs closureType (ctx.Globals :: ctx.Environment :: resolveClosureItems scope ctx)
  Expr.newGenericArgs Runtime.Function.functionTypeDef [closureType] [Expr.constant ast; closureExpr; ctx.Environment]

let private invoke (target:Node) (args:Node list) (ctx:Context) (builder:Builder) =
  Compiler.ExprGen.Helpers.dynamicInvoke (builder target ctx) (ctx.Globals :: [for arg in args -> builder arg ctx])

let private objectShorthand (properties:Map<string, Node> option) (ctx:Context) (builder:Builder) =
  match properties with
  | Some(_) -> failwith "Not supported"
  | None -> Expr.newArgs Runtime.Core.objectTypeDef [ctx.Environment]

let private closureValue name (ctx:Context) =
  Expr.field (Expr.field ctx.Closure (sprintf "Item%i" ctx.Scope.Closure.[name].Index)) "Value"

//Builder function for expression generation
let rec internal builder (ast:Node) (ctx:Context) =
  match ast with
  | Assign(left, right) -> assign left right ctx builder
  | Global(name) -> Js.Object.get ctx.Globals name
  | Local(name) -> ctx.Scope.Locals.[name].Expr :> Et
  | Closure(name) -> closureValue name ctx
  | Block(nodes) -> Expr.block [for node in nodes -> builder node ctx]
  | String(value) -> Expr.constant value
  | Number(value) -> Expr.constant value
  | Return(value) -> Js.makeReturn ctx.Return (builder value ctx)
  | Function(scope, _) -> func scope ast ctx builder
  | Invoke(target, args) -> invoke target args ctx builder
  | Object(properties) -> objectShorthand properties ctx builder
  | _ -> Expr.objDefault
