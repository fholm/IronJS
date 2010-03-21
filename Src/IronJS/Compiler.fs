module IronJS.Compiler

//Imports
open IronJS
open Ast
open System
open EtTools
open System.Linq.Expressions

//Type Aliases
type private Et = System.Linq.Expressions.Expression
type private EtParam = System.Linq.Expressions.ParameterExpression
type private AstUtils = Microsoft.Scripting.Ast.Utils
type private JsObj = IronJS.Runtime.JsObj

//Functions
let private clrToJsType x = 
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

type private Context = {
  Locals: Map<string, EtParam>
  Return: LabelTarget
  Closure: EtParam
} with
  member self.Globals with get() = field self.Closure "Globals"

let private createContext (s:Scope) = 
  let locals = Map.map (fun k v -> Et.Parameter(v.ForcedType.Value, k)) s.Locals
  {
    Locals = locals;
    Return = label "~return";
    Closure = locals.["~closure"];
  }

let rec private genEt node ctx =
  match node with
  | Var(n) -> genEt n ctx 
  | Block(n) -> genBlock n ctx
  | Assign(left, right) -> genAssign left right ctx
  | Ast.Number(value) -> constant value
  | Ast.String(value) -> constant value
  | Function(_) -> genFunc node ctx
  | _ -> EtTools.empty

and private genBlock nodes ctx =
  block [for n in nodes -> genEt n ctx]

and private genAssign left right ctx =
  match left with
  | Global(name) -> genAssign_Global name right ctx
  | _ -> EtTools.empty

and private genAssign_Global name value (ctx:Context) =
  call ctx.Globals "Set" [constant name; jsBox (genEt value ctx)]

and private genFunc node ctx =
  create (typeof<JsObj>) [(createOption typeof<IronJS.Runtime.Closure> [ctx.Globals; constant node]);]

let compile func (types:System.Type list) =
  match func with 
  | Function(parms, genericScope, name, body, cache) ->

    let funcType = createDelegateType types
    let found, cached = cache.TryGetValue(funcType)

    if found then 
      cached
    else
      let typedScope = createTypedScope parms types genericScope
      let untypedLocals = typedScope.Locals |> Map.filter (fun k v -> v.ForcedType = None)
      let context = createContext typedScope
      
      let parms = [for p in parms -> context.Locals.[p]]
      let body = block [genEt body context; labelExpr context.Return]
      let lambda = EtTools.lambda funcType parms body

      cache.GetOrAdd(funcType, lambda.Compile())

  | _ -> failwith "Can only compile Function nodes"