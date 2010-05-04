module IronJS.Compiler.Core

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler
open IronJS.Compiler.Types
open IronJS.Compiler.ExpressionState

let buildVarsMap (scope:Ast.Types.Scope) =

  let createVar (var:Ast.Types.Variable) =
    let clrTyp = Runtime.Utils.Type.jsToClr var.UsedAs
    if Ast.Variable.isClosedOver var
      then Expr.param var.Name (Type.strongBoxType.MakeGenericType([|clrTyp|]))
      else Expr.param var.Name clrTyp

  let createProxy (var:Ast.Types.Variable) =
    Expr.param (sprintf "%s_proxy" var.Name) scope.ArgTypes.[var.Index]

  let paramCount = ref 0
  let variables = 
    scope.Variables
      |>  Map.map (
            fun _ var -> 
              match Ast.Variable.isParameter var with
              | true -> 
                paramCount := !paramCount + 1
                if scope.ArgTypes.[var.Index] <> Runtime.Utils.Type.jsToClr var.UsedAs then
                  Proxied(createVar var, createProxy var, var.Index)
                else
                  match Ast.Variable.needsProxy var with
                  | true  -> Proxied(createVar var, createProxy var, var.Index)
                  | false -> Variable(createVar var, Param(var.Index))
              | false -> 
                Variable(createVar var, Local)
          )

  if scope.ArgTypes <> null && !paramCount < scope.ArgTypes.Length then
    let extraArgs = Seq.skip !paramCount scope.ArgTypes
    Seq.fold (fun s a -> 
      let name = sprintf "%i" !paramCount
      paramCount := !paramCount + 1
      Map.add name (Variable(Expr.param name a, Param(!paramCount-1))) s
    ) variables extraArgs
  else
    variables

let isLocal (_, var:Variable) =
  match var with
  | Variable(_, Local) -> true
  | Proxied(_, _, _) -> true
  | _ -> false

let isParameter (_, var:Variable) =
  match var with
  | Variable(_, Param(_)) -> true
  | Proxied(_, _, _) -> true
  | _ -> false

let toParm (_, var:Variable) =
  match var with
  | Variable(p, Param(i)) -> p, i
  | Proxied(_, p, i)  -> p, i
  | _ -> failwith "Que?"

let toLocal (_, var:Variable) =
  match var with
  | Variable(p, Local) -> p
  | Proxied(l, _, _)  -> l
  | _ -> failwith "Que?"

let isProxied (_, var:Variable) =
  match var with
  | Proxied(_, _, _) -> true
  | _ -> false

let builder (ctx:Context) (ast:Ast.Node) =
  match ast with
  //Simple
  | Ast.String(value)  -> static' (Expr.constant value)
  | Ast.Number(value)  -> static' (Expr.constant value)
  | Ast.Integer(value) -> static' (Expr.constant value)
  | Ast.Null           -> static' Expr.null'
  | Ast.Pass           -> static' Expr.void'

  //Assign
  | Ast.Assign(left, right) -> Assign.build ctx left right

  //Block
  | Ast.Block(nodes) -> volatile' (Expr.block [for n in nodes -> (ctx.Build n).Et])

  //Functions
  | Ast.Function(astId) -> Function.define ctx astId
  | Ast.Invoke(target, args, _) -> Function.invoke ctx target args
  | Ast.Return(value) -> Function.return' ctx value

  //Objects
  | Ast.Object(properties, id) -> Object.new' ctx properties id
  | Ast.Property(object', name) -> Object.getProperty ctx (ctx.Build object') name None 

  //Loops
  | Ast.ForIter(init, test, incr, body) -> Loops.forIter ctx init test incr body
  | Ast.BinaryOp(op, left, right) -> BinaryOp.build ctx op left right

  //Variable access
  | Ast.Global(name, _) -> 
    let typ = Context.temporaryType ctx name
    forceVolatile (Object.getProperty ctx (static' ctx.Internal.Globals) name typ)

  | Ast.Closure(name, _) -> static' (Context.closureExpr ctx name)
  | Ast.Variable(name, _) -> static' (Context.variableExpr ctx name)
  | Ast.This -> static' ctx.Internal.This

  | _ -> failwithf "No builder for '%A'" ast

(*Compiles a Ast.Node tree into a DLR Expression-tree*)
let compileAst (env:Runtime.Environment) (delegateType:ClrType) (closureType:ClrType) (scope:Ast.Types.Scope) (ast:Ast.Node) =

  let ctx = {
    Context.New with
      Scope = scope
      Variables = buildVarsMap scope
      Builder = builder
      Environment = env
      Internal = 
      {
        Types.InternalVariables.New with 
          Closure = Dlr.Expr.param "~closure" closureType
      }
  }

  let initGlobals = 
    let expr = Expr.field (Context.environmentExpr ctx) "Globals"
    Expr.assign ctx.Internal.Globals expr

  let initClosure = 
    let expr = Expr.field ctx.Internal.Function "Closure"
    Expr.assign ctx.Internal.Closure (Expr.cast closureType expr)

  let initEnvironment = 
    let expr = Expr.field ctx.Internal.Function "Environment"
    Expr.assign ctx.Internal.Environment expr

  (*Initialize proxied parameters*)
  let initProxied = 
    ctx.Variables
      |>  Map.toSeq
      |>  Seq.filter isProxied
      |>  Seq.map (fun (_, Proxied(var, proxy, _)) ->
            Utils.assign ctx var proxy
          )
      #if DEBUG
      |>  Seq.toArray
      #endif

  (*Initialize closed over variables and parameters*)
  let initClosedOver = 
    ctx.Scope.Variables
      |>  Map.toSeq
      |>  Seq.map (fun pair -> snd pair)
      |>  Seq.filter (fun lv -> Ast.Variable.isClosedOver lv)
      |>  Seq.filter (fun lv -> not (Ast.Variable.needsProxy lv))
      |>  Seq.map (fun lv -> 
            let expr = Context.variableExpr ctx lv.Name
            Utils.assign ctx expr (Expr.new' expr.Type) 
          )
      #if DEBUG
      |>  Seq.toArray
      #endif

  (*Initialize variables that need to be set as undefined*)
  let initUndefined =
    ctx.Scope.Variables
      |>  Map.toSeq
      |>  Seq.map (fun pair -> snd pair)
      |>  Seq.filter (fun lv -> Ast.Variable.initToUndefined lv)
      |>  Seq.map (fun lv -> 
            let expr = Context.variableExpr ctx lv.Name
            Utils.assign ctx expr Runtime.Undefined.InstanceExpr
          )
      #if DEBUG
      |>  Seq.toArray
      #endif

  (*The function main body*)
  let mainBody = ctx.Build ast

  (*Builds the if-statements and the end of each
  function that updates the object caches*)
  let objectCacheUpdateExpressions = 
    ctx.ObjectCaches 
    |>  Map.toSeq 
    |>  Seq.map (fun pair ->
          let cache = snd pair
          let last = Expr.field cache "LastCreated"
          (Expr.if'  
            (Expr.and' 
              (Expr.notEq last Expr.defaultT<Runtime.Object>)
              (Expr.notEq (Expr.field last "ClassId") (Expr.field cache "ClassId"))
            )
            (Expr.block [
              (Expr.assign (Expr.field cache "ClassId") (Expr.field last "ClassId"))
              (Expr.assign (Expr.field cache "Class") (Expr.field last "Class"))
              (Expr.assign (Expr.field cache "InitSize") (Expr.property (Expr.field last "Properties") "Length"))
            ])
          )
        )
    #if DEBUG
    |> Seq.toArray
    #endif
        
  (*Assemble the function body expression*)
  let body = 
    (Expr.field ctx.Internal.Environment "ReturnBox" :: [])
      |>  Seq.append objectCacheUpdateExpressions
      |>  Seq.append [Expr.labelExprVoid ctx.Return]
      |>  Seq.append (mainBody.Et :: [])
      |>  Seq.append initUndefined
      |>  Seq.append initClosedOver
      |>  Seq.append initProxied
      |>  Seq.append (initEnvironment :: initGlobals :: initClosure :: [])
      #if DEBUG
      |>  Seq.toArray
      |>  fun x -> Expr.block x
      #endif
    
  (*Resolve all locals that are parameters*)
  let parms  = 
    ctx.Variables
      |>  Map.toSeq
      |>  Seq.filter isParameter
      |>  Seq.map toParm
      |>  Seq.sortBy (fun pair -> snd pair)
      |>  Seq.map (fun pair -> fst pair)
      |>  Seq.append (Context.internalParams ctx)
      #if DEBUG
      |>  Seq.toArray
      #endif

  (*Resolve all locals that are normal variables*)
  let locals = 
    ctx.Variables
      |>  Map.toSeq
      |>  Seq.filter isLocal 
      |>  Seq.map toLocal
      |>  Seq.append (Context.internalLocals ctx)
      #if DEBUG
      |>  Seq.toArray
      #endif

  #if DEBUG
  let lmb = Dlr.Expr.lambda delegateType parms (Dlr.Expr.blockWithLocals locals [body])
  #else
  let lmb = Dlr.Expr.lambda delegateType parms (Dlr.Expr.blockWithLocals locals body)
  #endif

  #if DEBUG
  printf "%A" (Fsi.dbgViewProp.GetValue(lmb :> Et, null))
  #endif

  lmb

let compileAst2 (env:Runtime.Environment) (scope:Ast.Types.Scope) (ast:Ast.Node) (closure:ClrType) (delegate':ClrType) (argTypes:ClrType list) =
  let analyzedScope = Analyzer.analyze scope closure argTypes
  let lambdaExpr = compileAst env delegate' closure analyzedScope ast
  lambdaExpr.Compile()

let compileFile (env:Runtime.Environment) fileName =
  let scope, ast, astMap = Ast.Core.parseFile env.AstMap fileName
  env.AstMap <- astMap

  let globalType = Runtime.Delegate.getFor [] typeof<Runtime.Box>
  let exprTree = compileAst env globalType typeof<Runtime.Closure> scope ast
  let compiled = exprTree.Compile() :?> System.Func<Runtime.Function, Runtime.Object, Runtime.Box>

  let globalClosure = new Runtime.Closure(List.empty)
  let globalFunc = new Runtime.Function(-1, nativeint -1, globalClosure, env)

  fun () -> compiled.Invoke(globalFunc, env.Globals) |> ignore
