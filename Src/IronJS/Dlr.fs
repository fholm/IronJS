namespace IronJS

module Dlr = 

  open System.Reflection

  #if CLR2
  open System.Dynamic
  open Microsoft.Scripting.Ast
  #else
  open System.Dynamic
  open System.Linq.Expressions
  #endif

  type Expr = Expression
  type ExprList = System.Collections.Generic.List<Expr>
  type ExprType = ExpressionType

  type Parameter = ParameterExpression
  type ParameterList = System.Collections.Generic.List<Parameter>

  type Lambda = LambdaExpression
  type Constant = ConstantExpression

  type Label = LabelTarget
  type Br = BindingRestrictions

  type MetaObject = DynamicMetaObject
  type IMetaObjectProvider = IDynamicMetaObjectProvider

  module ArrayUtils =
    
    let RemoveLast (a:'a array) =
      let newArray = Array.zeroCreate<'a> (a.Length-1)
      System.Array.Copy(a, newArray, newArray.Length)
      newArray

    let RemoveFirst (a:'a array) =
      let newArray = Array.zeroCreate<'a> (a.Length-1)
      System.Array.Copy(a, 1, newArray, 0, newArray.Length)
      newArray

    let Insert (v:'a) (a:'a array) =
      let newArray = Array.zeroCreate<'a> (a.Length+1)
      System.Array.Copy(a, 0, newArray, 1, a.Length)
      newArray.[0] <- v
      newArray

  //Defaults
  let void' = Expr.Default(typeof<System.Void>) :> Expr
  let default' typ = Expr.Default(typ) :> Expr
  let defaultT<'a> = default' typeof<'a>
  let null' = defaultT<System.Object>

  //Variables
  let var name typ = Expr.Variable(typ, name)
  let varT<'a> name = var name typeof<'a>
  let paramI i typ = Expr.Parameter(typ, sprintf "param%i" i)
  let paramIT<'a> i = paramI i typeof<'a>
  let param name typ = Expr.Parameter(typ, name)
  let paramT<'a> name = param name typeof<'a>
  let paramRef name (typ:System.Type) = Expr.Parameter(typ.MakeByRefType(), name)
  let paramRefT<'a> name = paramRef name typeof<'a>
  
  let constant value = Expr.Constant(value, value.GetType()) :> Expr
  let inline const' value = Expr.Constant(value, value.GetType()) :> Expr
  let return' label (value:Expr) = Expr.Return(label, value) :> Expr
  let returnVoid label = Expr.Return(label) :> Expr
  let assign (left:Expr) (right:Expr) = Expr.Assign(left, right) :> Expr

  //Constants
  let true'   = const' true
  let false'  = const' false
  
  let sbyte_1 = const' -1y
  let sbyte0 = const' 0y
  let sbyte1 = const' 1y
  let sbyte2 = const' 2y

  let byte0 = const' 0uy
  let byte1 = const' 1uy
  let byte2 = const' 2uy

  let short_1 = const' -1s
  let short0 = const' 0s
  let short1 = const' 1s
  let short2 = const' 2s

  let ushort0 = const' 0us
  let ushort1 = const' 1us
  let ushort2 = const' 2us

  let int_1 = const' -1
  let int0 = const' 0
  let int1 = const' 1
  let int2 = const' 2

  let uint0 = const' 0u
  let uint1 = const' 1u
  let uint2 = const' 2u

  let long_1 = const' -1l
  let long0 = const' 0l
  let long1 = const' 1l
  let long2 = const' 2l

  let ulong0 = const' 0ul
  let ulong1 = const' 1ul
  let ulong2 = const' 2ul
  
  let float32_1 = const' -1.0f
  let float320 = const' 0.0f
  let float321 = const' 1.0f
  let float322 = const' 2.0f

  let dbl_1 = const' -1.0
  let dbl0 = const' 0.0
  let dbl1 = const' 1.0
  let dbl2 = const' 2.0

  let label (typ:System.Type) name = Expr.Label(typ, name)
  let labelVoid = label typeof<System.Void>
  let labelT<'a> = label typeof<'a>
  let labelExpr label (typ:System.Type) = Expr.Label(label, Expr.Default(typ)) :> Expr
  let labelExprT<'a> label = labelExpr label typeof<'a>
  let labelExprVal label (value:Expr) = Expr.Label(label, value) :> Expr
  let labelExprVoid label = labelExprVal label (default' typeof<System.Void>)
  let labelBreak () = labelVoid "break"
  let labelContinue () = labelVoid "continue"

  let break' label = Expr.Break label :> Expr
  let continue' label = Expr.Continue label :> Expr
  let goto label (value:Expr) = Expr.Goto(label, value) :> Expr
  let jump label = Expr.Goto label :> Expr

  let private _tmpCounter = ref 0L
  let tmpName () = 
    //As if this will happen, but just to make sure
    if !_tmpCounter = System.Int64.MaxValue then _tmpCounter := 0L
    _tmpCounter := !_tmpCounter + 1L
    sprintf "~tmp_%x" !_tmpCounter

  let tempFor (expr:Expr) =
    param (tmpName()) expr.Type

  let breakBlock (expr:Expr) =
    match expr with
    | :? BlockExpression as block when block.Variables.Count = 0 -> 
      seq block.Expressions
    | _ -> seq [expr]

  let block (parms:Parameter seq) (exprs:Expr seq) =
    if Seq.length exprs = 0
      then void'
      elif Seq.length parms = 0
        then Expr.Block(exprs) :> Expr
        else Expr.Block(parms, exprs) :> Expr

  let blockSimple (exprs:Expr seq) = 
    if Seq.length exprs = 0 
      then void' 
      elif Seq.length exprs = 1
        then FSharp.Seq.first exprs
        else Expr.Block(exprs) :> Expr

  let blockTmp type' (f:Parameter -> Expr seq) =
    let tmp = param (tmpName()) type'
    block [tmp] (f tmp)

  let blockTmpT<'a> f =
    blockTmp typeof<'a> f

  let field expr (name:string) = Expr.PropertyOrField(expr, name) :> Expr
  let fieldr (name:string) expr = Expr.PropertyOrField(expr, name) :> Expr

  let propertyInfoStatic (pi:PropertyInfo) = Expr.Property(null, pi) :> Expr
  let property expr (name:string) = Expr.PropertyOrField(expr, name) :> Expr
  let propertyr (name:string) expr = Expr.PropertyOrField(expr, name) :> Expr
  let propertyStatic (type':System.Type) name = Expr.Property(null, type', name) :> Expr
  let propertyStaticT<'a> = propertyStatic typeof<'a>
  let propertyOrField expr (name:string) = Expr.PropertyOrField(expr, name) :> Expr

  let private exprTypes (args:Expr seq) = [|for a in args -> a.Type|]

  let call (expr:Expr) name (args:Expr seq) =
    match FSharp.Reflection.getMethodArgs expr.Type name (exprTypes args) with
    | None -> failwith "No method found with matching name and arguments"
    | Some(method') -> Expr.Call(expr, method', args) :> Expr

  let callGeneric (expr:Expr) name typeArgs (args:Expr seq) =
    let exprTypes = (exprTypes args)
    match FSharp.Reflection.getMethodGeneric expr.Type name typeArgs exprTypes with
    | None -> 
      failwith "No method found with matching name, type args and arguments"
    | Some(method') -> Expr.Call(expr, method', args) :> Expr

  let callStatic (type':System.Type) name (args:Expr seq) =
    match FSharp.Reflection.getMethodArgs type' name (exprTypes args) with
    | None -> failwith "No method found with matching name and arguments"
    | Some(method') -> Expr.Call(null, method', args) :> Expr

  let callStaticT<'a> = callStatic typeof<'a>

  let callStaticGeneric (type':System.Type) name typeArgs (args:Expr seq) =
    match FSharp.Reflection.getMethodGeneric type' name typeArgs (exprTypes args) with
    | None -> 
      failwith "No method found with matching name, type args and arguments"

    | Some(method') -> Expr.Call(null, method', args) :> Expr

  let callStaticGenericT<'a> = callStaticGeneric typeof<'a>

  let callInstanceMethod (expr:Expr) (mi:MethodInfo) (args:Expr seq) = 
    Expr.Call(expr, mi, args) :> Expr

  let callMethod (mi:MethodInfo) (args:Expr seq) = 
    Expr.Call(null, mi, args) :> Expr

  let cast typ expr = Expr.Convert(expr, typ) :> Expr
  let castT<'a> = cast typeof<'a> 

  let castChk typ expr = Expr.ConvertChecked(expr, typ) :> Expr
  let castChkT<'a> = castChk typeof<'a> 

  let castAs typ expr = Expr.TypeAs(expr, typ) :> Expr
  let castAsT<'a> = castAs typeof<'a> 

  let castVoid (expr:Expr) = 
    if expr.Type = typeof<System.Void> 
      then expr
      else blockSimple [expr; void']

  let new' (typ:System.Type) = Expr.New(typ) :> Expr
  let newT<'a> = new' typeof<'a>
  let newGeneric (typ:System.Type) (types) = 
    new' (typ.MakeGenericType(Seq.toArray types))

  let newGenericT<'a> = newGeneric typedefof<'a>

  let newArgs (typ:System.Type) (args:Expr seq) = 
    match FSharp.Reflection.getCtor typ [for arg in args -> arg.Type] with
    | None -> failwith "No matching constructor found"
    | Some ctor -> Expr.New(ctor, args) :> Expr

  let newArgsT<'a> args = newArgs typeof<'a> args

  let newGenericArgs (typ:System.Type) (types) args = 
    newArgs (typ.MakeGenericType(Seq.toArray types)) args

  let newGenericArgsT<'a> = newGenericArgs typedefof<'a> 

  let delegateType (types:System.Type seq) = 
    Expr.GetDelegateType(Seq.toArray types)
    
  let lambda (typ:System.Type) (parms:Parameter seq) body = 
    Expr.Lambda(typ, body, parms)

  let lambdaT<'a> (parms:Parameter seq) (body:Expr) = Expr.Lambda<'a>(body, parms)
  let lambdaAuto (parms:Parameter seq) (body:Expr) = Expr.Lambda(body, parms)

  let invoke (func:Expr) (args:Expr seq) = Expr.Invoke(func, args) :> Expr
  
  let dynamicExpr typ binder (arg:Expr) = Expr.Dynamic(binder, typ, arg) :> Expr

  let dynamic typ binder (args:Expr seq) = Expr.Dynamic(binder, typ, args) :> Expr
  let dynamicT<'a> = dynamic typeof<'a>

  let length target = Expr.ArrayLength(target) :> Expr

  let item (expr:Expr) index = 
    let item = expr.Type.GetProperty("Item")
    Expr.MakeIndex(expr, item, [const' index]) :> Expr

  let index target (exprs:Expr seq) = Expr.ArrayAccess(target, exprs) :> Expr
  let indexInts target indexes = index target (indexes |> Seq.map (fun (x:int) -> const' x))
  let indexInt target index = indexInts target [index]
  let index0 target = index target [const' 0]
  let index1 target = index target [const' 1]
  let index2 target = index target [const' 2]

  let newArray typ (bounds:Expr seq) = Expr.NewArrayBounds(typ, bounds) :> Expr
  let newArrayT<'a> = new' typeof<'a>

  let newArrayItems typ (exprs:Expr seq) = Expr.NewArrayInit(typ, exprs) :> Expr
  let newArrayItemsT<'a> = newArrayItems typeof<'a>

  let newArrayBounds typ (size:Expr) = Expr.NewArrayBounds(typ, size) :> Expr
  let newArrayBoundsT<'a> = newArrayBounds typeof<'a>

  let newArrayEmpty typ = Expr.NewArrayBounds(typ, int0) :> Expr
  let newArrayEmptyT<'a> = newArrayEmpty typeof<'a>

  let throw (typ:System.Type) (args:Expr seq) = 
    Expr.Throw(newArgs typ args) :> Expr

  let throwT<'a> (args:Expr seq) = throw typeof<'a> args

  let throwValue expr =
    Expr.Throw(expr) :> Expr

  let catch (typ:System.Type) body = Expr.Catch(typ, body)
  let catchT<'a> = catch typeof<'a>
  let catchVar (var:Parameter) body = Expr.Catch(var, body)

  let tryCatch try' catches = Expr.TryCatch(try', Seq.toArray catches) :> Expr
  let tryFinally try' finally' = Expr.TryFinally(try', finally') :> Expr
  let tryCatchFinally try' catches finally' = 
    Expr.TryCatchFinally(try', finally', Seq.toArray catches) :> Expr

  let tryFault try' fault = Expr.TryFault(try', fault) :> Expr

  let bAnd' left right = Expr.And(left, right) :> Expr
  let bAndAsn left right = Expr.AndAssign(left, right) :> Expr

  let bOr' left right = Expr.Or(left, right) :> Expr
  let bOrAsn left right = Expr.OrAssign(left, right) :> Expr

  let xor left right = Expr.ExclusiveOr(left, right) :> Expr
  let xorAsn left right = Expr.ExclusiveOrAssign(left, right) :> Expr

  let rhs left right = Expr.RightShift(left, right) :> Expr
  let rhsAsn left right = Expr.RightShiftAssign(left, right) :> Expr

  let lhs left right = Expr.LeftShift(left, right) :> Expr
  let lhsAsn left right = Expr.LeftShiftAssign(left, right) :> Expr

  let cmpl target = (Expr.Not target) :> Expr

  let if' test ifTrue = Expr.IfThen(test, ifTrue) :> Expr
  let ifElse test ifTrue ifFalse = Expr.IfThenElse(test, ifTrue, ifFalse) :> Expr
  let ternary test ifTrue ifFalse = Expr.Condition(test, ifTrue, ifFalse) :> Expr

  let loop_ test incr body break' (continue':Label) =
    let test = ifElse test void' (Expr.Break break')
    let continue' = Expr.Label(continue') :> Expr

    Expr.Loop(
      (Expr.Block [test; body; continue'; incr]),
      break'
    ) :> Expr

  let for' init test incr body break' continue' = 
    blockSimple [
      (init)
      (loop_ test incr body break' continue')]

  let while' test body break' continue' = 
    loop_ test void' body break' continue'

  let doWhile test body breakLabel continueLabel =
    let restartLabel = labelVoid "~restart"

    block [] [
      labelExprVoid restartLabel
      body
      labelExprVoid continueLabel
      if' test (jump restartLabel)
      labelExprVoid breakLabel
    ]

  let loop break' continue' test body =
    loop_ test void' body break' continue'

  let sub left right = Expr.Subtract(left, right) :> Expr
  let subChk left right = Expr.SubtractChecked(left, right) :> Expr
  let subAsn left right = Expr.SubtractAssign(left, right) :> Expr
  let subAsnChk left right = Expr.SubtractAssignChecked(left, right) :> Expr

  let add left right = Expr.Add(left, right) :> Expr
  let addChk left right = Expr.AddChecked(left, right) :> Expr
  let addAsn left right = Expr.AddAssign(left, right) :> Expr
  let addAsnChk left right = Expr.AddAssignChecked(left, right) :> Expr

  let concat left right = callStaticT<System.String> "Concat" [left; right]

  let mul left right = Expr.Multiply(left, right) :> Expr
  let mulChk left right = Expr.MultiplyChecked(left, right) :> Expr
  let mulAsn left right = Expr.MultiplyAssign(left, right) :> Expr
  let mulAsnChk left right = Expr.MultiplyAssignChecked(left, right) :> Expr

  let div left right = Expr.Divide(left, right) :> Expr
  let divAsn left right = Expr.DivideAssign(left, right) :> Expr

  let pow left right = Expr.Power(left, right) :> Expr
  let powAsn left right = Expr.PowerAssign(left, right) :> Expr

  let mod' left right = Expr.Modulo(left, right) :> Expr
  let modAsn left right = Expr.ModuloAssign(left, right) :> Expr

  let or' left right = Expr.OrElse(left, right) :> Expr
  let orChain (c:Expr list) = 
    match c with
    | [] -> true'
    | _  -> List.fold (fun s x -> or' s x) c.Head c.Tail 

  let and' left right = Expr.AndAlso(left, right) :> Expr
  let andChain (c:Expr list) = 
    match c with
    | [] -> true'
    | _  -> List.fold (fun s x -> and' s x) c.Head c.Tail

  let is' typ target = Expr.TypeIs(target, typ) :> Expr
  let isT<'a> = is' typeof<'a> 

  let typeEq target typ = Expr.TypeEqual(target, typ) :> Expr

  let refEq left right = Expr.ReferenceEqual(left, right) :> Expr
  let refNotEq left right = Expr.ReferenceNotEqual(left, right) :> Expr

  let eq left right = Expr.Equal(left, right) :> Expr
  let notEq left right = Expr.NotEqual(left, right) :> Expr

  let lt left right = Expr.LessThan(left, right) :> Expr
  let ltEq left right = Expr.LessThanOrEqual(left, right) :> Expr

  let gt left right = Expr.GreaterThan(left, right) :> Expr
  let gtEq left right = Expr.GreaterThanOrEqual(left, right) :> Expr

  let not target = Expr.OnesComplement target :> Expr
  let notDefault target = notEq target (default' target.Type)

  let isFalse target = Expr.IsFalse(target) :> Expr
  let isTrue target = Expr.IsTrue(target) :> Expr

  let isDefault ex = eq ex (default' ex.Type)
  let isNull = isDefault
  let isNotNull = notDefault

  let isNull_Real (expr:Expr) =
    callStaticT<obj> "ReferenceEquals" [castT<obj> expr; defaultT<obj>]

  let isNotNull_Real (expr:Expr) =
    expr |> isNull_Real |> not

  let assignDefault ex = assign ex (default' ex.Type)
  let assignNull = assignDefault

  module Fast = 

    open System

    module Utils =
      let is type' (expr:Expr) = FSharp.Utils.isType type' expr.Type
      let isT<'a> (expr:Expr) = is typeof<'a> expr
      let isVoid (expr:Expr) = FSharp.Utils.isVoid expr.Type

    let toExpr (x:'a when 'a :> Expr) = x :> Expr

    let local name type' = Expr.Variable(type', name)
    let localT<'a> name = var name typeof<'a>
    
    let param name type' = Expr.Parameter(type', name)
    let paramT<'a> name = param name typeof<'a>

    let paramRef name (type':Type) = param name (type'.MakeByRefType())
    let paramRefT<'a> name = paramRef name typeof<'a>

    let paramN n type' = param (sprintf "param%i" n) type'
    let paramNT<'a> n = paramI n typeof<'a>
  
    let defaultOf type' = Expr.Default(type') :> Expr
    let defaultOfT<'a> = default' typeof<'a>

    let void' = default' typeof<System.Void>
    let null' = default' typeof<System.Object>
    
    let constant value = Expr.Constant(value, value.GetType()) :> Expr
    let assign (left:Expr) (right:Expr) = Expr.Assign(left, right) :> Expr

    let returnValue label (value:Expr) = Expr.Return(label, value, value.GetType()) :> Expr
    let returnVoid label = Expr.Return(label) :> Expr
    
    let field (name:string) (expr:Expr) = Expr.Field(expr, name) :> Expr
    let fieldStatic (name:string) (type':Type) = Expr.Field(null, type'.GetField(name))
    let fieldStaticT<'a> (name:string) = fieldStatic name typeof<'a>

    let property (name:string) (expr:Expr) = Expr.Property(expr, name) :> Expr
    let propertyOrField (name:string) (expr:Expr) = Expr.PropertyOrField(expr, name) :> Expr

    let cast type' expr = Expr.Convert(expr, type') :> Expr
    let castT<'a> = cast typeof<'a> 

    let castChecked type' expr = Expr.ConvertChecked(expr, type') :> Expr
    let castCheckedT<'a> = castChk typeof<'a> 

    let castAs type' expr = Expr.TypeAs(expr, type') :> Expr
    let castAsT<'a> = castAs typeof<'a> 

    let castToVoid (expr:Expr) = 
      if expr.Type = typeof<System.Void> 
        then expr
        else blockSimple [expr; void']

    let call (name:string) (args:Expr seq) (expr:Expr) =
      match FSharp.Reflection.getMethodArgs expr.Type name (exprTypes args) with
      | Some method' -> Expr.Call(expr, method', args) :> Expr
      | None -> failwith "No method found with matching name and arguments"

    let block (parameters:Parameter array) (expressions:Expr array) =
      if expressions.Length = 0
        then void' 
        elif parameters.Length = 0 
          then Expr.Block(expressions) :> Expr
          else Expr.Block(parameters, expressions) :> Expr

    let blockOfSeq (parameters:Parameter seq) (expressions:Expr seq) =
      if Seq.length expressions = 0
        then void' 
        elif Seq.length parameters = 0 
          then Expr.Block(expressions) :> Expr
          else Expr.Block(parameters, expressions) :> Expr

    let blockTemp tempType (f:Parameter -> Expr array) =
      let tmp = param (tmpName()) tempType
      tmp |> f |> block [|tmp|]

    let blockTempT<'a> f =
      blockTemp typeof<'a> f

    module Object =
      
      let toString expr = 
        if expr |> Utils.isT<string> then expr else expr |> call "ToString" []

    module String = 
      
      let empty = fieldStaticT<string> "Empty"
      
      let concat left right = 
        let left = left |> Object.toString
        let right = right |> Object.toString
        callStaticT<string> "Concat" [left; right]
        
  let rec isStatic (expr:Expr) = 
    if expr :? Parameter || expr :? ConstantExpression || expr :? DefaultExpression then
      true

    elif expr :? IndexExpression then
      let expr = expr :?> IndexExpression
      expr.Object |> isStatic && expr.Arguments.Count = 1 && expr.Arguments.[0] |> isStatic

    elif expr :? MemberExpression then
      let expr = expr :?> MemberExpression
      expr.Expression |> isStatic

    elif expr :? UnaryExpression then
      let expr = expr :?> UnaryExpression
      expr.Operand |> isStatic

    else
      false

  module Restrict =
    let notAtAll = Br.Empty
    let byExpr expr = Br.GetExpressionRestriction(expr)
    let byType expr typ = Br.GetTypeRestriction(expr, typ)
    let byInstance expr instance = Br.GetInstanceRestriction(expr, instance)

    let argRestrict (a:System.Dynamic.DynamicMetaObject) =
      let restriction = 
        if a.HasValue && a.Value = null 
          then Br.GetInstanceRestriction(a.Expression, null')
          else Br.GetTypeRestriction(a.Expression, a.LimitType)

      a.Restrictions.Merge(restriction)

    let byArgs (args:System.Dynamic.DynamicMetaObject seq) =
      Seq.fold (fun (s:Br) a -> s.Merge(argRestrict a)) Br.Empty args

  module FSharp =
    let tuple (itemExprs:Expr list) =
      let type' =
        match itemExprs.Length with
        | 0 -> failwith "Can't create zero length tupple"
        | 1 -> typedefof<System.Tuple<_>>
        | 2 -> typedefof<System.Tuple<_, _>>
        | 3 -> typedefof<System.Tuple<_, _, _>>
        | 4 -> typedefof<System.Tuple<_, _, _, _>>
        | 5 -> typedefof<System.Tuple<_, _, _, _, _>>
        | 6 -> typedefof<System.Tuple<_, _, _, _, _, _>>
        | 7 -> typedefof<System.Tuple<_, _, _, _, _, _, _>>
        | _ -> failwith "Max length on tuples is 7 items"

      newGenericArgs type' [for expr in itemExprs -> expr.Type] itemExprs

  module Utils =
    open System.Reflection

    let private _dbgViewProp = 
      let flags = BindingFlags.NonPublic ||| BindingFlags.Instance
      typeof<Expr>.GetProperty("DebugView", flags)

    let debugView (expr:Expr) = string (_dbgViewProp.GetValue(expr, null))
    let printDebugView (expr:Expr) = printf "%s" (expr |> debugView)

    let is type' (expr:Expr) = FSharp.Utils.isType type' expr.Type
    let isT<'a> (expr:Expr) = is typeof<'a> expr
    let isVoid (expr:Expr) = FSharp.Utils.isVoid expr.Type

    //DEBUG
    let debug x =
      #if DEBUG 
        #if INTERACTIVE
        constant x
        #else
        callStaticT<System.Console> "WriteLine" [x]
        #endif
      #else
        #if INTERACTIVE
        constant x
        #else
        void'
        #endif
      #endif

  module Operators =
    
    let inline (!!!) x = const' x

    // Dot operator for binding field access
    let (.->) a b = propertyOrField a b

    let (.@) a (b:int) = index a [const' b]

    // Binary operators, starts with .
    let (.=) a b = assign a b
    let (.+) a b = add a b
    let (.-) a b = sub a b
    let (.*) a b = mul a b
    let (./) a b = div a b
    let (.%) a b = mod' a b
    let (.**) a b = pow a b

    let (.+=) a b = addAsn a b
    let (.-=) a b = subAsn a b
    let (.*=) a b = mulAsn a b
    let (./=) a b = divAsn a b
    let (.%=) a b = modAsn a b
    let (.^=) a b = powAsn a b

    let (.==) a b = eq a b
    let (.!=) a b = notEq a b
    let (.<) a b  = lt a b
    let (.<=) a b = ltEq a b 
    let (.>) a b  = gt a b
    let (.>=) a b = gtEq a b

    let (.<<) a b = lhs a b
    let (.>>) a b = rhs a b
    let (.^) a b = xor a b
    let (.&) a b  = bAnd' a b
    let (.|) a b  = bOr' a b

    let (.||) a b = or' a b
    let (.&&) a b = and' a b
    
    // Unary operators, starts with !
    let (!~) a = cmpl a
    let (!!) a = not a

  module ExtensionMethods =
    #if CLR2
    type Microsoft.Scripting.Ast.Expression with
    #else
    type System.Linq.Expressions.Expression with
    #endif
      member x.CallMember(name) = x.CallMember(name, [])
      member x.CallMember(name, args) = call x name args