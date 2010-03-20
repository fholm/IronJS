module IronJS.Compiler

//Imports
open Ast
open System
open EtTools
open System.Linq.Expressions

//Type Aliases
type internal Et = System.Linq.Expressions.Expression
type internal EtParam = System.Linq.Expressions.ParameterExpression
type internal AstUtils = Microsoft.Scripting.Ast.Utils

//Functions
let internal clrToJsType x = 
  if x = ClrTypes.Integer then Type.Integer
  elif x = ClrTypes.Double then Type.Double
  elif x = ClrTypes.String then Type.String
  else Type.Dynamic

let rec private getVarType (name:string) (scope:Scope) (evaling:string Set) =
  if evaling.Contains(name) then
    Type.None
  else
    let local = scope.Locals.[name]
    
    match local.ForcedType with
    | Some(t) -> clrToJsType(t)
    | None -> 
      match local.UsedAs with
      | Type.Integer 
      | Type.Double  
      | Type.String  
      | Type.Object -> 
        let evalingSet = evaling.Add(name)

        let rec evalUsedWith vars =
          match vars with
          | [] -> Type.None
          | x::xs -> (getVarType x scope evalingSet) ||| (evalUsedWith xs)

        local.UsedWith |> List.ofSeq |> evalUsedWith

      | _ -> Type.Dynamic

let private createDelegateType (types:System.Type list) =
  Et.GetFuncType(List.toArray (List.append types [ClrTypes.Dynamic]))

let rec private createTypedScope (parms: string list) (inTypes:System.Type list) (scope: Scope) = 
  match parms with
  | [] -> scope
  | x::xs -> 
    let typ, types = match inTypes with 
                     | [] -> typeToClr(Type.Dynamic), [] 
                     | x::xs -> x, xs

    let local = { scope.Locals.[x] with ForcedType = Some(typ) }
    createTypedScope xs types { scope with Locals = scope.Locals.Add(x, local) }

type internal Context = {
  Locals: Map<string, EtParam>
  Return: LabelTarget
}

let internal createContext (s:Scope) = {
  Context.Locals =  Map.map (fun k v -> Et.Parameter(v.ForcedType.Value, k)) s.Locals;
  Return = label "~return";
}

let rec internal etgen node ctx =
  match node with
  | Var(n) -> etgen n ctx 
  | Block(n) -> genBlock n ctx
  | _ -> AstUtils.Empty() :> Et

and internal genBlock nodes ctx =
  block [for n in nodes -> etgen n ctx]

let compile func (types:System.Type list) =
  match func with 
  | Function(parms, genericScope, name, body) ->

    let typedScope = createTypedScope parms types genericScope
    let untypedLocals = typedScope.Locals |> Map.filter (fun k v -> v.ForcedType = None)
    let context = createContext typedScope

    let funcType = createDelegateType types
    let parms = [for p in parms -> context.Locals.[p]]
    let body = block [etgen body context; labelExpr context.Return]

    let lambda = EtTools.lambda funcType parms body

    lambda.Compile()

  | _ -> failwith "Can only compile Function nodes"