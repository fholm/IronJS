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
  | Local(name) -> Js.assign (ctx.Scope.Locals.[name].Expr) (builder right ctx)
  | _ -> Expr.objDefault

let private typeIsGeneric (typ:ClrType) = typ.IsGenericType && not typ.IsGenericTypeDefinition 

let private getGenericArgument n (typ:ClrType) = 
  if typeIsGeneric typ 
    then typ.GetGenericArguments().[n] 
    else failwith "%A is not a generic type" typ

let private getStrongBoxType = getGenericArgument 0
let private closureFieldName name ctx = sprintf "Item%i" ctx.Scope.Closure.[name].Index
let private getFieldType name (typ:ClrType) = (typ.GetField name).FieldType

let private getLocalClrType name ctx =
  if ctx.Scope.Locals.ContainsKey name
    then ToClr ctx.Scope.Locals.[name].UsedAs
    else failwithf "No local variable named '%s' exist" name

let private getClosureClrType name ctx =
  if ctx.Scope.Closure.ContainsKey name 
    then getStrongBoxType (getFieldType (closureFieldName name ctx) ctx.Closure.Type)
    else failwithf "No closure variable named '%s' exist" name

let private getClosureJsType = (IronJS.Operators.pair getClosureClrType) >> ToJs

let private getVariableType name local ctx =
  if local then getLocalClrType name ctx else getClosureClrType name ctx

let private getVariableExpr name local ctx =
  if local
    then ctx.Scope.Locals.[name].Expr :> Et
    else Expr.field ctx.Closure (sprintf "Item%i" ctx.Scope.Closure.[name].Index) 

let private resolveClosureItems (scope:Scope) ctx =
  Map.toList scope.Closure
  |> List.sortWith (fun a b -> (snd a).Index - (snd b).Index)
  |> List.map (fun pair -> getVariableExpr (fst pair) ((snd pair).IsLocalInParent) ctx)

let private resolveClosureType (scope:Scope) ctx =
  Runtime.Closures.getClosureType (
    Map.fold (fun state key closure -> (getVariableType key closure.IsLocalInParent ctx, closure.Index) :: state) [] scope.Closure
    |> List.sortWith (fun a b -> (snd a) - (snd b))
    |> List.map (fun pair -> fst pair)
  )

let private func scope (ast:Ast.Types.Node) ctx =
  let closureType = resolveClosureType scope ctx
  let closureExpr = Expr.newArgs closureType (ctx.Globals :: ctx.Environment :: resolveClosureItems scope ctx)
  Expr.newGenericArgs Runtime.Function.functionTypeDef [closureType] [Expr.constant ast; closureExpr; ctx.Environment]

let private invoke target args ctx (builder:Builder) =
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
  | Function(scope, _) -> func scope ast ctx
  | Invoke(target, args) -> invoke target args ctx builder
  | Object(properties) -> objectShorthand properties ctx builder
  | _ -> Expr.objDefault