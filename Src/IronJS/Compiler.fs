module IronJS.Compiler.Core

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler

type private F = IronJS.Ast.LocalFlags

let private buildVarsMap (scope:Ast.Scope) =

  let (|Parameter|Local|) (input:Ast.Local) = 
    if Set.contains F.Parameter input.Flags then Parameter else Local

  let (|Proxied|Not|) (input:Ast.Local) =
    if Set.contains F.NeedProxy input.Flags then Proxied else Not

  let dynamicIndex = ref -1 
  let dynamicExpr = Expr.paramT<Runtime.Box array> "~dynamic"

  let createVar (l:Ast.Local) =
    let clrTyp = Utils.Type.jsToClr l.UsedAs
    if l.ClosedOver 
      then Expr.param l.Name (Constants.strongBoxTypeDef.MakeGenericType([|clrTyp|]))
      else Expr.param l.Name clrTyp

  let createProxy (l:Ast.Local) =
    Expr.param (sprintf "%s_proxy" l.Name) scope.ArgTypes.[l.Index]

  let vars = 
    scope.Locals
      |> Map.map(fun _ l -> 
                  match l with
                  | Parameter -> 
                    match l with
                    | Proxied -> Proxied(createVar l, createProxy l)
                    | Not -> Variable(createVar l, P)
                  | Local     -> 
                    if l.IsDynamic 
                      then dynamicIndex := !dynamicIndex + 1
                           Expr(Expr.Array.access dynamicExpr [Expr.constant !dynamicIndex])
                      else Variable(createVar l, L)
                )

  vars, (!dynamicIndex + 1), dynamicExpr

let isLocal (pair:string * Var) =
  match pair with
  | (_, Variable(_, L)) -> true
  | _ -> false

let isParameter (pair:string * Var) =
  match pair with
  | (_, Variable(_, P)) -> true
  | (_, Proxied(_, _)) -> true
  | _ -> false

let toParm (pair:string * Var) =
  match pair with
  | (_, Variable(p, _)) -> p
  | (_, Proxied(_, p))  -> p
  | _ -> failwith "Que?"

(*Compiles a Ast.Node tree into a DLR Expression-tree*)
let compileAst (env:Runtime.IEnvironment) (delegateType:ClrType) (closureType:ClrType) (scope:Ast.Scope) (ast:Ast.Node) =

  let vars, dynamicCount, dynamicArray = buildVarsMap scope

  let ctx = {
    Context.New with
      DynamicArray = dynamicArray
      DynamicCount = dynamicCount
      Closure = Dlr.Expr.param "~closure" closureType
      Scope = scope
      Builder = Compiler.ExprGen.builder
      TemporaryTypes = new SafeDict<string, ClrType>()
      Env = env
      Locals = vars
  }

  let initGlobals = Expr.assign ctx.Globals (Expr.field ctx.Environment "Globals")
  let initClosure = Expr.assign ctx.Closure (Expr.cast closureType (Expr.field ctx.Function "Closure"))
  let initDynamic = 
    if dynamicCount = 0 then Expr.empty
    else 
      Expr.assign ctx.DynamicArray (Expr.Array.newT<Runtime.Box> [Expr.constant dynamicCount])

  let body = 
    [ctx.Builder2 ast; Expr.labelExprVoid ctx.Return]
      |> List.toSeq
      |> Seq.append [initGlobals; initClosure; initDynamic]
      #if DEBUG
      |> Seq.toArray
      |> fun x -> Expr.block x
      #endif

  let parms  = 
    vars
      |> Map.toSeq
      |> Seq.filter isParameter
      |> Seq.map toParm
      |> Seq.append (ctx.Function :: ctx.This :: ctx.ReturnParam :: [])
      #if DEBUG
      |> Seq.toArray
      #endif

  let locals = 
    vars
      |> Map.toSeq
      |> Seq.filter isLocal 
      |> Seq.map toParm
      |> Seq.append (ctx.Globals :: ctx.Closure :: ctx.DynamicArray :: [])
      #if DEBUG
      |> Seq.toArray
      #endif

  #if DEBUG
  let lmb = Dlr.Expr.lambda delegateType parms (Dlr.Expr.blockWithLocals locals [body])
  #else
  let lmb = Dlr.Expr.lambda delegateType parms (Dlr.Expr.blockWithLocals locals body)
  #endif

  #if INTERACTIVE
  printf "%A" (Fsi.dbgViewProp.GetValue(lmb :> Et, null))
  #endif

  lmb