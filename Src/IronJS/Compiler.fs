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

//
let private createDelegateType (types:System.Type list) =
  Et.GetFuncType(List.toArray (List.append types [Types.ClrDynamic]))

//
let private forceType name (scope:Scope) typ =
  let local = { scope.Locals.[name] with ForcedType = Some(typ) }
  { scope with Locals = scope.Locals.Add(name, local) }

//
let private createContext (s:Scope) = 
  let locals = Map.map (fun k v -> Et.Parameter(v.ForcedType.Value, k)) s.Locals 
  {
    Locals = locals;
    Return = label "~return";
    Closure = locals.["~closure"];
  }

//
let private evalLocalType (name:string) (scope:Scope) =

  let rec evalType name (scope:Scope) (evaling:string Set) =
    if evaling.Contains name then Types.JsTypes.None
    else
      let local = scope.Locals.[name]
      
      match local.ForcedType with
      | Some(t) -> IronJS.Types.ToJs t
      | None -> 
      
        let rec evalUsedWith = function
          | [] -> Types.JsTypes.None
          | x::xs -> evalType x scope (evaling.Add name) ||| evalUsedWith xs

        let usedAs = (local.UsedWith |> List.ofSeq |> evalUsedWith) ||| local.UsedAs

        match usedAs with
        | Types.JsTypes.Integer 
        | Types.JsTypes.Double  
        | Types.JsTypes.String -> usedAs
        | _ -> Types.JsTypes.Dynamic

  evalType name scope Set.empty

// 
let private resolveLocalTypes (scope:Scope) =
  scope.Locals 
    |> Map.filter (fun k v -> v.ForcedType = None) 
    |> Map.fold 
      (*funct*) (fun scope name v -> forceType name scope (Types.ToClr (evalLocalType name scope))) 
      (*state*) scope

//
let rec private injectParameterTypes (parms: string list) (types:System.Type list) (scope: Scope) = 
  match parms with
  | [] -> scope
  | name::parms -> 
    let typ, types = match types with 
                     | []    -> Types.ClrDynamic, []  
                     | x::xs -> x, xs

    injectParameterTypes parms types (forceType name scope typ)

(*
  Expression Tree Generation
*)
//
let private genEtList (nodes:Node list) (ctx:Context) gen =
  [for node in nodes -> gen node ctx]

//
let private genAssignGlobal name value (ctx:Context) gen =
  call ctx.Globals "Set" [(*1*) constant name; (*2*) jsBox (gen value ctx)]

//
let private genAssign left right ctx gen =
  match left with
  | Global(name) -> genAssignGlobal name right ctx gen
  | _ -> EtTools.empty

//
let private genBlock nodes ctx gen =
  block [for n in nodes -> gen n ctx]

let private getClosureType (vars:string list) (ctx:Context) =

  let getVarType name =
    if ctx.Locals.ContainsKey name 
      then ctx.Locals.[name].Type
      else failwith "fuuuuck"

  let closureType = 
    Closures.getClosureType (fix (fun f vars -> match vars with | [] -> [] | x::xs -> getVarType x :: f xs) vars)

  closureType, [for var in vars -> ctx.Locals.[var] :> Et]

//
let private genFunc node (ctx:Context) gen =
  match node with
  | Function(parms, scope, name, body, cache) ->
    
    let closureType, closureParams = getClosureType scope.Closure ctx

    create (typeof<JsFunc>) [
      (*1*) create closureType (ctx.Globals :: constant node :: ctx.Compiler :: closureParams);
      (*2*) constant node
    ]

  | _ -> failwith "Can only compile Function nodes"

//
let private genInvoke target args (ctx:Context) gen =
  Binders.dynamicInvoke 
    (*target*) (gen target ctx) 
    (*params*) ((ctx.Closure :> Et) :: ctx.Globals :: (genEtList args ctx gen))

//
let private genGlobal name (ctx:Context) =
  call ctx.Globals "Get" [constant name]

//
let private genLocal name (ctx:Context) =
  ctx.Locals.[name] :> Et

// 
let private genClosure name (ctx:Context) =
  empty

//
let private genReturn node (ctx:Context) gen =
  gotoReturn ctx.Return (jsBox (gen node ctx))

//
let rec private genEt node ctx =
  match node with
  | Var(node) -> // var x; var x = <expr>;
    genEt node ctx 

  | Block(node) ->  // { <exprs> }
    genBlock node ctx genEt

  | Local(name) -> // foo
    genLocal name ctx

  | Closure(name) -> // foo
    genClosure name ctx

  | Global(name) -> // foo
    genGlobal name ctx

  | Assign(left, right) -> // foo = <expr>
    genAssign left right ctx genEt

  | Ast.Number(Integer(value)) -> // 1
    constant value 

  | Ast.Number(Double(value)) -> // 1.0
    constant value 

  | Ast.String(value) -> // "foo"
    constant value

  | Function(_) -> // function foo([<exprs>]) { [<exprs>] }
    genFunc node ctx genEt

  | Invoke(target, args) -> // foo([<exprs>])
    genInvoke target args ctx genEt

  | Return(node) -> // return <expr>
    genReturn node ctx genEt

  | _ -> EtTools.empty

//
let compile func (types:System.Type list) =
  match func with 
  | Function(parms, genericScope, name, body, cache) ->

    let funcType = createDelegateType types
    let found, cached = cache.TryGetValue(funcType)

    if found then cached
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
      | x -> reraise()

  | _ -> failwith "Can only compile Function nodes"