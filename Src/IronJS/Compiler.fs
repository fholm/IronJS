module IronJS.Compiler

//Imports
open System
open System.Linq.Expressions

open IronJS
open IronJS.Ast
open IronJS.EtTools
open IronJS.Utils
open IronJS.Runtime

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
  member self.Compiler with get() = field self.Closure "Compiler"

let private createContext (s:Scope) = 
  let locals = Map.map (fun k v -> Et.Parameter(v.ForcedType.Value, k)) s.Locals
  {
    Locals = locals;
    Return = label "~return";
    Closure = locals.["~closure"];
  }

//
let private genEtList (nodes:Node list) (ctx:Context) gen =
  [for node in nodes -> gen node ctx]

//
let private genBlock nodes ctx gen =
  block [for n in nodes -> gen n ctx]

//
let private genAssignGlobal name value (ctx:Context) gen =
  call ctx.Globals "Set" [constant name; jsBox (gen value ctx)]

//
let private genAssign left right ctx gen =
  match left with
  | Global(name) -> genAssignGlobal name right ctx gen
  | _ -> EtTools.empty

//
let private genFunc node (ctx:Context) gen =
  create (typeof<JsFunc>) [
    create typeof<IronJS.Runtime.Closure> [ctx.Globals; constant node; ctx.Compiler]; 
    constant node
  ]

//
let private genInvoke target args ctx gen =
  Binders.dynamicInvoke (gen target ctx) (genEtList args ctx gen)

//
let private genGlobal name (ctx:Context) gen =
  call ctx.Globals "Get" [constant name]

//
let rec private genEt node ctx =
  match node with
  | Var(n) -> genEt n ctx 
  | Block(n) -> genBlock n ctx genEt
  | Global(name) -> genGlobal name ctx genEt
  | Assign(left, right) -> genAssign left right ctx genEt
  | Ast.Number(value) -> constant value 
  | Ast.String(value) -> constant value
  | Function(_) -> genFunc node ctx genEt
  | Invoke(target, args) -> genInvoke target args ctx genEt
  | _ -> EtTools.empty

//
let compile func (types:System.Type list) =
  match func with 
  | Function(parms, genericScope, name, body, cache) ->

    let funcType = createDelegateType types
    let found, cached = cache.TryGetValue(funcType)

    if found then 
      cached, funcType
    else
      let typedScope = createTypedScope parms types genericScope
      let untypedLocals = typedScope.Locals |> Map.filter (fun k v -> v.ForcedType = None)
      let context = createContext typedScope
      
      let parms = [for p in parms -> context.Locals.[p]]
      let body = block [genEt body context; labelExpr context.Return]
      let lambda = EtTools.lambda funcType parms body

      cache.GetOrAdd(funcType, lambda.Compile()), funcType

  | _ -> failwith "Can only compile Function nodes"