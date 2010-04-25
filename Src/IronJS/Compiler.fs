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

  let createVar (l:Ast.Local) =
    let clrTyp = Utils.Type.jsToClr l.UsedAs
    if l.ClosedOver 
      then Expr.param l.Name (Constants.strongBoxTypeDef.MakeGenericType([|clrTyp|]))
      else Expr.param l.Name clrTyp

  let createProxy (l:Ast.Local) =
    Expr.param (sprintf "%s_proxy" l.Name) scope.ArgTypes.[l.Index]

  scope.Locals
    |> Map.map(fun _ l -> 
                match l with
                | Parameter -> 
                  match l with
                  | Proxied -> Proxied(createVar l, createProxy l)
                  | Not -> Variable(createVar l, P)
                | Local -> Variable(createVar l, L)
              )

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

  let ctx = {
    Context.New with
      Closure = Dlr.Expr.param "~closure" closureType
      Scope = scope
      Builder = Compiler.ExprGen.builder
      TemporaryTypes = new SafeDict<string, ClrType>()
      Env = env
      Locals = buildVarsMap scope
  }

  let initGlobals = Expr.assign ctx.Globals (Expr.field ctx.Environment "Globals")
  let initClosure = Expr.assign ctx.Closure (Expr.cast closureType (Expr.field ctx.Function "Closure"))

  let body = 
    [ctx.Builder2 ast; Expr.labelExprT<Runtime.Box> ctx.Return]
      |> List.toSeq
      |> Seq.append [initGlobals; initClosure]
      #if DEBUG
      |> Seq.toArray
      |> fun x -> Expr.block x
      #endif

  let parms  = 
    ctx.Locals
      |> Map.toSeq
      |> Seq.filter isParameter
      |> Seq.map toParm
      |> Seq.append (ctx.Function :: ctx.This :: [])
      #if DEBUG
      |> Seq.toArray
      #endif

  let locals = 
    ctx.Locals
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