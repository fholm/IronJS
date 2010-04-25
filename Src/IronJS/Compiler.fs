module IronJS.Compiler.Core

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler

type private F = IronJS.Ast.LocalFlags

let (|Parameter|Local|) (input:Ast.Local) = 
  if Set.contains F.Parameter input.Flags then Parameter else Local

let (|ClosedOver|Not|) (input:Ast.Local) = 
  if Set.contains F.ClosedOver input.Flags then ClosedOver else Not

let (|NeedProxy|Not|) (input:Ast.Local) =
  if Set.contains F.NeedProxy input.Flags then NeedProxy else Not

let private buildVarsMap (scope:Ast.Scope) =

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
                  | NeedProxy -> Proxied(createVar l, createProxy l)
                  | Not -> Variable(createVar l, P)
                | Local -> 
                  Variable(createVar l, L)
              )

let isLocal (_, v:Var) =
  match v with
  | Variable(_, L) -> true
  | Proxied(_, _) -> true
  | _ -> false

let isParameter (_, v:Var) =
  match v with
  | Variable(_, P) -> true
  | Proxied(_, _) -> true
  | _ -> false

let toParm (_, v:Var) =
  match v with
  | Variable(p, P) -> p
  | Proxied(_, p)  -> p
  | _ -> failwith "Que?"

let toLocal (_, v:Var) =
  match v with
  | Variable(p, L) -> p
  | Proxied(l, _)  -> l
  | _ -> failwith "Que?"

let isProxied (_, v:Var) =
  match v with
  | Proxied(_, _) -> true
  | _ -> false

let initProxies (_, v:Var) =
  match v with
  | Proxied(var, proxy) ->
    if Js.isStrongBox var.Type 
      then Expr.assign var (Expr.newArgs var.Type [proxy])
      else Expr.assign var (Expr.cast var.Type proxy)
  | _ -> failwith "Not allowed"

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

  let initGlobals = 
    let field = Expr.field ctx.Environment "Globals"
    Expr.assign ctx.Globals field

  let initClosure = 
    let field = Expr.field ctx.Function "Closure"
    Expr.assign ctx.Closure (Expr.cast closureType field)

  let initProxied = 
    (Dlr.Expr.block 
      (ctx.Locals
        |> Map.toSeq
        |> Seq.filter isProxied
        |> Seq.map initProxies
        #if DEBUG
        |> Seq.toArray
        #endif
      )
    )

  let initClosedOver = 
    (Dlr.Expr.block 
      (ctx.Scope.Locals
        |> Map.toSeq
        |> Seq.map (fun pair -> snd pair)
        |> Seq.filter (fun l -> l.ClosedOver)
        |> Seq.filter (fun l -> not l.NeedsProxy)
        |> Seq.map (fun l -> 
             let expr = ctx.LocalExpr l.Name
             Expr.assign expr (Expr.new' expr.Type) 
           )
        #if DEBUG
        |> Seq.toArray
        #endif
      )
    )

  let body = 
    [ctx.Builder2 ast; Expr.labelExprT<Runtime.Box> ctx.Return]
      |> List.toSeq
      |> Seq.append [
           initGlobals
           initClosure
           initProxied
           initClosedOver
         ]
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
      |> Seq.map toLocal
      |> Seq.append (ctx.Globals :: ctx.Closure :: [])
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