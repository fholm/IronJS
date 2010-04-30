module IronJS.Compiler.Core

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler

#nowarn "25"

let private buildVarsMap (scope:Ast.Types.Scope) =

  let (|Parameter|Local|) (lv:Ast.Types.Variable) = 
    if Ast.Variable.isParameter lv then Parameter else Local

  let (|NeedProxy|Not|) (lv:Ast.Types.Variable) =
    if Ast.Variable.needsProxy lv then NeedProxy else Not

  let createVar (l:Ast.Types.Variable) =
    let clrTyp = Utils.Type.jsToClr l.UsedAs
    if Ast.Variable.isClosedOver l 
      then Expr.param l.Name (Type.strongBoxType.MakeGenericType([|clrTyp|]))
      else Expr.param l.Name clrTyp

  let createProxy (l:Ast.Types.Variable) =
    Expr.param (sprintf "%s_proxy" l.Name) scope.ArgTypes.[l.Index]

  scope.Variables
    |> Map.map(fun _ lv -> 
                match lv with
                | Parameter -> 
                  match lv with
                  | NeedProxy -> Proxied(createVar lv, createProxy lv)
                  | Not -> Variable(createVar lv, P)
                | Local -> 
                  Variable(createVar lv, L)
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
let compileAst (env:Runtime.Environment) (delegateType:ClrType) (closureType:ClrType) (scope:Ast.Types.Scope) (ast:Ast.Node) =

  let ctx = {
    Context.New with
      Closure = Dlr.Expr.param "~closure" closureType
      Scope = scope
      Builder = Compiler.ExprGen.builder
      TemporaryTypes = new SafeDict<string, ClrType>()
      Environment = env
      Locals = buildVarsMap scope
  }

  let initGlobals = 
    let field = Expr.field ctx.EnvironmentExpr "Globals"
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
           if Type.isStrongBox var.Type 
             then Expr.assign var (Expr.newArgs var.Type [proxy])
             else Expr.assign var (Expr.cast var.Type proxy)
         )
      #if DEBUG
      |> Seq.toArray
      #endif

  (*Initialize closed over variables and parameters*)
  let initClosedOver = 
    ctx.Scope.Variables
      |> Map.toSeq
      |> Seq.map (fun pair -> snd pair)
      |> Seq.filter (fun lv -> Ast.Variable.isClosedOver lv)
      |> Seq.filter (fun lv -> not (Ast.Variable.needsProxy lv))
      |> Seq.map (fun lv -> 
           let expr = ctx.LocalExpr lv.Name
           Expr.assign expr (Expr.new' expr.Type) 
         )
      #if DEBUG
      |> Seq.toArray
      #endif

  (*Initialize variables that need to be set as undefined*)
  let initUndefined =
    ctx.Scope.Variables
      |> Map.toSeq
      |> Seq.map (fun pair -> snd pair)
      |> Seq.filter (fun lv -> Ast.Variable.initToUndefined lv)
      |> Seq.map (fun lv -> 
           let expr = ctx.LocalExpr lv.Name
           Utils.Assign.value expr Runtime.Undefined.InstanceExpr
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

  #if DEBUG
  printf "%A" (Fsi.dbgViewProp.GetValue(lmb :> Et, null))
  #endif

  lmb