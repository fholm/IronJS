module IronJS.Compiler

//Imports
open System
open System.Linq.Expressions

open IronJS
open IronJS.Ast
open IronJS.EtTools
open IronJS.Utils
open IronJS.Runtime

//Types
type private Context = {
  Locals: Map<string, EtParam>
  Return: LabelTarget
  Closure: EtParam
} with
  member self.Globals with get() = field self.Closure "Globals"
  member self.Compiler with get() = field self.Closure "Compiler"

//Functions
let private clrToJsType x = 
  if x = ClrTypes.Integer then Type.Integer
  elif x = ClrTypes.Double then Type.Double
  elif x = ClrTypes.String then Type.String
  else Type.Dynamic

let private evalVarType (name:string) (scope:Scope) =

  let rec getVarType (name:string) (scope:Scope) (evaling:string Set) =
    if evaling.Contains(name) then
      Type.None
    else
      let local = scope.Locals.[name]
      
      match local.ForcedType with
      | Some(t) -> clrToJsType(t)
      | None -> 
      
        let evalingSet = evaling.Add(name)

        let rec evalUsedWith vars =
          match vars with
          | [] -> Type.None
          | x::xs -> (getVarType x scope evalingSet) ||| (evalUsedWith xs)

        let usedAs = (local.UsedWith |> List.ofSeq |> evalUsedWith) ||| local.UsedAs

        match usedAs with
        | Type.Integer 
        | Type.Double  
        | Type.String -> usedAs
        | _ -> Type.Dynamic

  getVarType name scope Set.empty

//
let private createDelegateType (types:System.Type list) =
  Et.GetFuncType(List.toArray (List.append types [ClrTypes.Dynamic]))

//
let private forceType name (scope:Scope) typ =
  let local = { scope.Locals.[name] with ForcedType = Some(typ) }
  { scope with Locals = scope.Locals.Add(name, local) }

//
let rec private injectParameterTypes (parms: string list) (types:System.Type list) (scope: Scope) = 
  match parms with
  | [] -> scope
  | name::parms -> 

    let typ, types = 
      match types with 
      | [] -> typeToClr(Type.Dynamic), []  
      | x::xs -> x, xs

    injectParameterTypes parms types (forceType name scope typ)

//
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
  call ctx.Globals "Set" [(*1*) constant name; (*2*) jsBox (gen value ctx)]

//
let private genAssign left right ctx gen =
  match left with
  | Global(name) -> genAssignGlobal name right ctx gen
  | _ -> EtTools.empty

//
let private genFunc node (ctx:Context) gen =
  create (typeof<JsFunc>) [
    (*1*) create typeof<IronJS.Runtime.Closure> [(*1*) ctx.Globals; (*2*) constant node; (*3*) ctx.Compiler]; 
    (*2*) constant node
  ]

//
let private genInvoke target args (ctx:Context) gen =
  Binders.dynamicInvoke 
    (*target*) (gen target ctx) 
    (*params*) ((ctx.Closure :> Et) :: ctx.Globals :: (genEtList args ctx gen))

//
let private genGlobal name (ctx:Context) gen =
  call ctx.Globals "Get" [constant name]

//
let rec private genEt node ctx =
  match node with
  | Var(n) -> // var x; var x = <expr>;
    genEt n ctx 

  | Block(n) ->  // { <exprs> }
    genBlock n ctx genEt

  | Global(name) -> // foo
    genGlobal name ctx genEt

  | Assign(left, right) -> 
    genAssign left right ctx genEt

  | Ast.Number(Integer(value)) -> // 1
    constant value 

  | Ast.Number(Double(value)) -> // 1.0
    constant value 

  | Ast.String(value) -> // "foo"
    constant value

  | Function(_) -> 
    genFunc node ctx genEt

  | Invoke(target, args) -> 
    genInvoke target args ctx genEt

  | _ -> EtTools.empty

// 
let private resolveLocalTypes (scope:Scope) =
  scope.Locals 
    |> Map.filter (fun k v -> v.ForcedType = None) 
    |> Map.fold 
      (*funct*) (fun scope name v -> forceType name scope (typeToClr (evalVarType name scope))) 
      (*state*) scope

//
let compile func (types:System.Type list) =
  match func with 
  | Function(parms, genericScope, name, body, cache) ->

    let funcType = createDelegateType types
    let found, cached = cache.TryGetValue(funcType)

    if found then 
      cached
    else
      try
        let paramTypedScope = injectParameterTypes parms types genericScope
        let strongTypedScope = resolveLocalTypes paramTypedScope 

        let context = createContext strongTypedScope
        let parameters = [for p in parms -> context.Locals.[p]]

        let locals = 
          context.Locals
          |> Map.filter (fun k v -> not (List.exists (fun p -> p = k) parms)) 
          |> Map.fold (fun s k v -> v :: s) []

        let body = block [(*1*) genEt body context; (*2*) labelExpr context.Return]

        let lambda = EtTools.lambda (**) funcType (**) parameters (*body*) (blockParms locals [body])

        cache.GetOrAdd(funcType, lambda.Compile())
      with
      | x -> failwith "%A" x

  | _ -> failwith "Can only compile Function nodes"