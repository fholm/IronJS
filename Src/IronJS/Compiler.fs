module IronJS.Compiler.Core

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler

type private F = IronJS.Ast.LocalFlags

let private (|Parameter|Local|) (input:Ast.Local) = 
  if Set.contains F.Parameter input.Flags then Parameter else Local

let private (|NeedProxy|Not|) (input:Ast.Local) =
  if Set.contains F.NeedProxy input.Flags then NeedProxy else Not

let private buildVarsMap (scope:Ast.Scope) =

  let createVar (l:Ast.Local) =
    let clrTyp = Utils.Type.jsToClr l.UsedAs
    if l.IsClosedOver 
      then Expr.param l.Name (Type.strongBoxType.MakeGenericType([|clrTyp|]))
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

let private isLocal (_, v:Var) =
  match v with
  | Variable(_, L) -> true
  | Proxied(_, _) -> true
  | _ -> false

let private isParameter (_, v:Var) =
  match v with
  | Variable(_, P) -> true
  | Proxied(_, _) -> true
  | _ -> false

let private toParm (_, v:Var) =
  match v with
  | Variable(p, P) -> p
  | Proxied(_, p)  -> p
  | _ -> failwith "Que?"

let private toLocal (_, v:Var) =
  match v with
  | Variable(p, L) -> p
  | Proxied(l, _)  -> l
  | _ -> failwith "Que?"

let private isProxied (_, v:Var) =
  match v with
  | Proxied(_, _) -> true
  | _ -> false

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

  (*Initialize proxied parameters*)
  let initProxied = 
    ctx.Locals
      |> Map.toSeq
      |> Seq.filter isProxied
      |> Seq.map (fun (_, Proxied(var, proxy)) ->
           if Js.isStrongBox var.Type 
             then Expr.assign var (Expr.newArgs var.Type [proxy])
             else Expr.assign var (Expr.cast var.Type proxy)
         )
      #if DEBUG
      |> Seq.toArray
      #endif

  (*Initialize closed over variables and parameters*)
  let initClosedOver = 
    ctx.Scope.Locals
      |> Map.toSeq
      |> Seq.map (fun pair -> snd pair)
      |> Seq.filter (fun l -> l.IsClosedOver)
      |> Seq.filter (fun l -> not l.NeedsProxy)
      |> Seq.map (fun l -> 
           let expr = ctx.LocalExpr l.Name
           Expr.assign expr (Expr.new' expr.Type) 
         )
      #if DEBUG
      |> Seq.toArray
      #endif

  (*Initialize variables that need to be set as undefined*)
  let initUndefined =
    ctx.Scope.Locals
      |> Map.toSeq
      |> Seq.map (fun pair -> snd pair)
      |> Seq.filter (fun l -> l.InitUndefined)
      |> Seq.map (fun l -> ctx.Locals.[l.Name])
      |> Seq.map (fun v -> 
           let expr = match v with
                      | Expr(expr) -> expr
                      | Variable(p, _) -> p:>Et
                      | _ -> failwith "Que?"
           Assign.value expr Runtime.Undefined.InstanceExpr
         )
      #if DEBUG
      |> Seq.toArray
      #endif
    
  (*Assemble the function body expression*)
  let body = 
    (ctx.Builder2 ast :: Expr.labelExprT<Runtime.Box> ctx.Return :: [])
      |> List.toSeq
      |> Seq.append initUndefined
      |> Seq.append initClosedOver
      |> Seq.append initProxied
      |> Seq.append (initGlobals :: initClosure :: [])
      #if DEBUG
      |> Seq.toArray
      |> fun x -> Expr.block x
      #endif

  (*Resolve all locals that are parameters*)
  let parms  = 
    ctx.Locals
      |> Map.toSeq
      |> Seq.filter isParameter
      |> Seq.map toParm
      |> Seq.append (ctx.Function :: ctx.This :: [])
      #if DEBUG
      |> Seq.toArray
      #endif

  (*Resolve all locals that are normal variables*)
  let vars = 
    ctx.Locals
      |> Map.toSeq
      |> Seq.filter isLocal 
      |> Seq.map toLocal
      |> Seq.append (ctx.Globals :: ctx.Closure :: [])
      #if DEBUG
      |> Seq.toArray
      #endif

  #if DEBUG
  let lmb = Dlr.Expr.lambda delegateType parms (Dlr.Expr.blockWithLocals vars [body])
  #else
  let lmb = Dlr.Expr.lambda delegateType parms (Dlr.Expr.blockWithLocals vars body)
  #endif

  #if INTERACTIVE
  printf "%A" (Fsi.dbgViewProp.GetValue(lmb :> Et, null))
  #endif

  lmb