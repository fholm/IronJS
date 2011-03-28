namespace IronJS

module Dlr = 

  open System.Dynamic
  open System.Reflection
  open System.Linq.Expressions

  //Type Aliases
  type private Et = Expression
  type private EtParam = ParameterExpression

  type Expr = Expression
  type ExprParam = ParameterExpression

  type Label = LabelTarget
  type ExprType = ExpressionType
  type Br = BindingRestrictions

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

  type MetaObj = DynamicMetaObject
  type IMetaObjProvider = IDynamicMetaObjectProvider

  //Defaults
  let void' = Et.Default(typeof<System.Void>) :> Et
  let default' typ = Et.Default(typ) :> Et
  let defaultT<'a> = default' typeof<'a>
  let null' = defaultT<System.Object>

  //Variables
  let var name typ = Et.Variable(typ, name)
  let varT<'a> name = var name typeof<'a>
  let paramI i typ = Et.Parameter(typ, sprintf "param%i" i)
  let paramIT<'a> i = paramI i typeof<'a>
  let param name typ = Et.Parameter(typ, name)
  let paramT<'a> name = param name typeof<'a>
  let paramRef name (typ:System.Type) = Et.Parameter(typ.MakeByRefType(), name)
  let paramRefT<'a> name = paramRef name typeof<'a>
  
  let constant value = Et.Constant(value, value.GetType()) :> Et
  let inline const' value = Et.Constant(value, value.GetType()) :> Expr
  let return' label (value:Et) = Et.Return(label, value) :> Et
  let returnVoid label = Et.Return(label) :> Et
  let assign (left:Et) (right:Et) = Et.Assign(left, right) :> Et

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

  let label (typ:System.Type) name = Et.Label(typ, name)
  let labelVoid = label typeof<System.Void>
  let labelT<'a> = label typeof<'a>
  let labelExpr label (typ:System.Type) = Et.Label(label, Et.Default(typ)) :> Et
  let labelExprT<'a> label = labelExpr label typeof<'a>
  let labelExprVal label (value:Et) = Et.Label(label, value) :> Et
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

  let breakBlock (expr:Expr) =
    match expr with
    | :? BlockExpression as block when block.Variables.Count = 0 -> 
      seq block.Expressions
    | _ -> seq [expr]

  let block (parms:ExprParam seq) (exprs:Expr seq) =
    if Seq.length exprs = 0
      then void'
      elif Seq.length parms = 0
        then Et.Block(exprs) :> Expr
        else Et.Block(parms, exprs) :> Expr

  let blockSimple (exprs:Expr seq) = 
    if Seq.length exprs = 0 
      then void' 
      elif Seq.length exprs = 1
        then FSKit.Seq.first exprs
        else Expr.Block(exprs) :> Expr

  let blockTmp type' (f:ExprParam -> Expr seq) =
    let tmp = param (tmpName()) type'
    block [tmp] (f tmp)

  let blockTmpT<'a> f =
    blockTmp typeof<'a> f

  let field expr (name:string) = Et.PropertyOrField(expr, name) :> Expr
  let fieldr (name:string) expr = Et.PropertyOrField(expr, name) :> Expr

  let propertyInfoStatic (pi:PropertyInfo) = Et.Property(null, pi) :> Expr
  let property expr (name:string) = Et.PropertyOrField(expr, name) :> Expr
  let propertyr (name:string) expr = Et.PropertyOrField(expr, name) :> Expr
  let propertyStatic (type':System.Type) name = Et.Property(null, type', name) :> Expr
  let propertyStaticT<'a> = propertyStatic typeof<'a>
  let propertyOrField expr (name:string) = Et.PropertyOrField(expr, name) :> Expr

  let private exprTypes (args:Expr seq) = [|for a in args -> a.Type|]

  let call (expr:Expr) name (args:Expr seq) =
    match FSKit.Reflection.getMethodArgs expr.Type name (exprTypes args) with
    | None -> failwith "No method found with matching name and arguments"
    | Some(method') -> Et.Call(expr, method', args) :> Expr

  let callGeneric (expr:Expr) name typeArgs (args:Expr seq) =
    let exprTypes = (exprTypes args)
    match FSKit.Reflection.getMethodGeneric expr.Type name typeArgs exprTypes with
    | None -> 
      failwith "No method found with matching name, type args and arguments"
    | Some(method') -> Et.Call(expr, method', args) :> Expr

  let callStatic (type':System.Type) name (args:Expr seq) =
    match FSKit.Reflection.getMethodArgs type' name (exprTypes args) with
    | None -> failwith "No method found with matching name and arguments"
    | Some(method') -> Et.Call(null, method', args) :> Expr

  let callStaticT<'a> = callStatic typeof<'a>

  let callStaticGeneric (type':System.Type) name typeArgs (args:Expr seq) =
    match FSKit.Reflection.getMethodGeneric type' name typeArgs (exprTypes args) with
    | None -> 
      failwith "No method found with matching name, type args and arguments"

    | Some(method') -> Et.Call(null, method', args) :> Expr

  let callStaticGenericT<'a> = callStaticGeneric typeof<'a>

  let callInstanceMethod (expr:Expr) (mi:MethodInfo) (args:Expr seq) = 
    Expr.Call(expr, mi, args) :> Expr

  let callMethod (mi:MethodInfo) (args:Expr seq) = 
    Expr.Call(null, mi, args) :> Expr

  let cast typ expr = Et.Convert(expr, typ) :> Et
  let castT<'a> = cast typeof<'a> 

  let castChk typ expr = Et.ConvertChecked(expr, typ) :> Et
  let castChkT<'a> = castChk typeof<'a> 

  let castAs typ expr = Et.TypeAs(expr, typ) :> Et
  let castAsT<'a> = castAs typeof<'a> 

  let castVoid (expr:Expr) = 
    if expr.Type = typeof<System.Void> 
      then expr
      else blockSimple [expr; void']

  let new' (typ:System.Type) = Et.New(typ) :> Et
  let newT<'a> = new' typeof<'a>
  let newGeneric (typ:System.Type) (types) = 
    new' (typ.MakeGenericType(Seq.toArray types))

  let newGenericT<'a> = newGeneric typedefof<'a>

  let newArgs (typ:System.Type) (args:Et seq) = 
    match FSKit.Reflection.getCtor typ [for arg in args -> arg.Type] with
    | None -> failwith "No matching constructor found"
    | Some ctor -> Et.New(ctor, args) :> Expr

  let newArgsT<'a> args = newArgs typeof<'a> args

  let newGenericArgs (typ:System.Type) (types) args = 
    newArgs (typ.MakeGenericType(Seq.toArray types)) args

  let newGenericArgsT<'a> = newGenericArgs typedefof<'a> 

  let delegateType (types:System.Type seq) = 
    Et.GetDelegateType(Seq.toArray types)
    
  let lambda (typ:System.Type) (parms:EtParam seq) body = 
    Et.Lambda(typ, body, parms)

  let lambdaT<'a> (parms:EtParam seq) (body:Et) = Et.Lambda<'a>(body, parms)
  let lambdaAuto (parms:EtParam seq) (body:Et) = Et.Lambda(body, parms)

  let invoke (func:Et) (args:Et seq) = Et.Invoke(func, args) :> Et

  let dynamic typ binder (args:Et seq) = Et.Dynamic(binder, typ, args) :> Et
  let dynamicT<'a> = dynamic typeof<'a>

  let length target = Et.ArrayLength(target) :> Et

  let item (expr:Expr) index = 
    let item = expr.Type.GetProperty("Item")
    Expr.MakeIndex(expr, item, [const' index]) :> Expr

  let index target (exprs:Et seq) = Et.ArrayAccess(target, exprs) :> Expr
  let indexInts target indexes = index target (indexes |> Seq.map (fun (x:int) -> const' x))
  let indexInt target index = indexInts target [index]
  let index0 target = index target [const' 0]
  let index1 target = index target [const' 1]
  let index2 target = index target [const' 2]

  let newArray typ (bounds:Et seq) = Et.NewArrayBounds(typ, bounds) :> Expr
  let newArrayT<'a> = new' typeof<'a>

  let newArrayItems typ (exprs:Et seq) = Et.NewArrayInit(typ, exprs) :> Expr
  let newArrayItemsT<'a> = newArrayItems typeof<'a>

  let newArrayBounds typ (size:Expr) = Expr.NewArrayBounds(typ, size) :> Expr
  let newArrayBoundsT<'a> = newArrayBounds typeof<'a>

  let newArrayEmpty typ = Expr.NewArrayBounds(typ, int0) :> Expr
  let newArrayEmptyT<'a> = newArrayEmpty typeof<'a>

  let throw (typ:System.Type) (args:Expr seq) = 
    Et.Throw(newArgs typ args) :> Expr

  let throwT<'a> (args:Expr seq) = throw typeof<'a> args

  let throwValue expr =
    Et.Throw(expr) :> Expr

  let catch (typ:System.Type) body = Et.Catch(typ, body)
  let catchT<'a> = catch typeof<'a>
  let catchVar (var:EtParam) body = Et.Catch(var, body)

  let tryCatch try' catches = Et.TryCatch(try', Seq.toArray catches) :> Expr
  let tryFinally try' finally' = Et.TryFinally(try', finally') :> Expr
  let tryCatchFinally try' catches finally' = 
    Et.TryCatchFinally(try', finally', Seq.toArray catches) :> Expr

  let tryFault try' fault = Et.TryFault(try', fault) :> Expr

  let bAnd' left right = Et.And(left, right) :> Expr
  let bAndAsn left right = Et.AndAssign(left, right) :> Expr

  let bOr' left right = Et.Or(left, right) :> Expr
  let bOrAsn left right = Et.OrAssign(left, right) :> Expr

  let xor left right = Et.ExclusiveOr(left, right) :> Expr
  let xorAsn left right = Et.ExclusiveOrAssign(left, right) :> Expr

  let rhs left right = Et.RightShift(left, right) :> Expr
  let rhsAsn left right = Et.RightShiftAssign(left, right) :> Expr

  let lhs left right = Et.LeftShift(left, right) :> Expr
  let lhsAsn left right = Et.LeftShiftAssign(left, right) :> Expr

  let cmpl target = (Et.Not target) :> Et

  let if' test ifTrue = Et.IfThen(test, ifTrue) :> Et
  let ifElse test ifTrue ifFalse = Et.IfThenElse(test, ifTrue, ifFalse) :> Et
  let ternary test ifTrue ifFalse = Et.Condition(test, ifTrue, ifFalse) :> Et

  let loop_ test incr body break' (continue':Label) =
    let test = ifElse test void' (Et.Break break')
    let continue' = Et.Label(continue') :> Et

    Et.Loop(
      (Et.Block [test; body; continue'; incr]),
      break'
    ) :> Et

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

  let sub left right = Et.Subtract(left, right) :> Et
  let subChk left right = Et.SubtractChecked(left, right) :> Et
  let subAsn left right = Et.SubtractAssign(left, right) :> Et
  let subAsnChk left right = Et.SubtractAssignChecked(left, right) :> Et

  let add left right = Et.Add(left, right) :> Et
  let addChk left right = Et.AddChecked(left, right) :> Et
  let addAsn left right = Et.AddAssign(left, right) :> Et
  let addAsnChk left right = Et.AddAssignChecked(left, right) :> Et

  let concat left right = callStaticT<System.String> "Concat" [left; right]

  let mul left right = Et.Multiply(left, right) :> Et
  let mulChk left right = Et.MultiplyChecked(left, right) :> Et
  let mulAsn left right = Et.MultiplyAssign(left, right) :> Et
  let mulAsnChk left right = Et.MultiplyAssignChecked(left, right) :> Et

  let div left right = Et.Divide(left, right) :> Et
  let divAsn left right = Et.DivideAssign(left, right) :> Et

  let pow left right = Et.Power(left, right) :> Et
  let powAsn left right = Et.PowerAssign(left, right) :> Et

  let mod' left right = Et.Modulo(left, right) :> Et
  let modAsn left right = Et.ModuloAssign(left, right) :> Et

  let or' left right = Et.OrElse(left, right) :> Et
  let orChain (c:Et list) = 
    match c with
    | [] -> true'
    | _  -> List.fold (fun s x -> or' s x) c.Head c.Tail 

  let and' left right = Et.AndAlso(left, right) :> Et
  let andChain (c:Et list) = 
    match c with
    | [] -> true'
    | _  -> List.fold (fun s x -> and' s x) c.Head c.Tail

  let is' typ target = Et.TypeIs(target, typ) :> Et
  let isT<'a> = is' typeof<'a> 

  let typeEq target typ = Et.TypeEqual(target, typ) :> Et

  let refEq left right = Et.ReferenceEqual(left, right) :> Et
  let refNotEq left right = Et.ReferenceNotEqual(left, right) :> Et

  let eq left right = Et.Equal(left, right) :> Et
  let notEq left right = Et.NotEqual(left, right) :> Et

  let lt left right = Et.LessThan(left, right) :> Et
  let ltEq left right = Et.LessThanOrEqual(left, right) :> Et

  let gt left right = Et.GreaterThan(left, right) :> Et
  let gtEq left right = Et.GreaterThanOrEqual(left, right) :> Et

  let not target = Et.OnesComplement target :> Et
  let notDefault target = notEq target (default' target.Type)

  let isFalse target = Et.IsFalse(target) :> Et
  let isTrue target = Et.IsTrue(target) :> Et

  let isDefault ex = eq ex (default' ex.Type)
  let isNull = isDefault
  let isNotNull = notDefault

  let isNull_Real (expr:Expr) =
    callStaticT<obj> "ReferenceEquals" [castT<obj> expr; defaultT<obj>]

  let isNotNull_Real (expr:Expr) =
    expr |> isNull_Real |> not

  let assignDefault ex = assign ex (default' ex.Type)
  let assignNull = assignDefault

  module V2 = 

    open System

    module Utils =
      let is type' (expr:Expr) = FSKit.Utils.isType type' expr.Type
      let isT<'a> (expr:Expr) = is typeof<'a> expr
      let isVoid (expr:Expr) = FSKit.Utils.isVoid expr.Type

    let toExpr (x:'a when 'a :> Expr) = x :> Expr

    let local name type' = Expr.Variable(type', name)
    let localT<'a> name = var name typeof<'a>
    
    let param name type' = Et.Parameter(type', name)
    let paramT<'a> name = param name typeof<'a>

    let paramRef name (type':Type) = param name (type'.MakeByRefType())
    let paramRefT<'a> name = paramRef name typeof<'a>

    let paramN n type' = param (sprintf "param%i" n) type'
    let paramNT<'a> n = paramI n typeof<'a>
  
    let defaultOf type' = Expr.Default(type') :> Expr
    let defaultOfT<'a> = default' typeof<'a>

    let void' = default' typeof<System.Void>
    let null' = default' typeof<System.Object>
    
    let constant value = Et.Constant(value, value.GetType()) :> Expr
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

    let castChecked type' expr = Et.ConvertChecked(expr, type') :> Expr
    let castCheckedT<'a> = castChk typeof<'a> 

    let castAs type' expr = Expr.TypeAs(expr, type') :> Expr
    let castAsT<'a> = castAs typeof<'a> 

    let castToVoid (expr:Expr) = 
      if expr.Type = typeof<System.Void> 
        then expr
        else blockSimple [expr; void']

    let call (name:string) (args:Expr seq) (expr:Expr) =
      match FSKit.Reflection.getMethodArgs expr.Type name (exprTypes args) with
      | Some method' -> Expr.Call(expr, method', args) :> Expr
      | None -> failwith "No method found with matching name and arguments"

    module Object =
      
      let toString expr = 
        if expr |> Utils.isT<string> then expr else expr |> call "ToString" []

    module String = 
      
      let empty = fieldStaticT<string> "Empty"
      
      let concat left right = 
        let left = left |> Object.toString
        let right = right |> Object.toString
        callStaticT<string> "Concat" [left; right]

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
      typeof<System.Linq.Expressions.Expression>.GetProperty("DebugView", flags)

    let debugView (expr:Expr) = string (_dbgViewProp.GetValue(expr, null))
    let printDebugView (expr:Expr) = printf "%s" (expr |> debugView)

    let is type' (expr:Expr) = FSKit.Utils.isType type' expr.Type
    let isT<'a> (expr:Expr) = is typeof<'a> expr
    let isVoid (expr:Expr) = FSKit.Utils.isVoid expr.Type

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

  module Ext =

    //-------------------------------------------------------------------------
    // Type that flags a containing expression as static
    type Static(expr) =
      inherit Expr()

      member x.Inner = expr

      override x.NodeType = ExprType.Extension
      override x.CanReduce = true
      override x.Reduce() = expr
      override x.Type = expr.Type
        
    let isStatic (expr:Expr) = 
      expr :? Static || expr :? ExprParam || expr :? ConstantExpression

    let static' (expr:Expr) =
      if isStatic expr
        then expr
        else (Static expr) :> Expr 

    let unwrap (expr:Expr) = 
      if expr :? Static then (expr :?> Static).Inner else expr

  module Operators =
    
    let inline (!!!) x = const' x
    let inline (!@) x = Ext.unwrap x

    // Dynamic bind operator for accessing fields
    let (?) (expr:Expr) field = propertyOrField expr field

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
    type System.Linq.Expressions.Expression with
      member x.CallMember(name) = x.CallMember(name, [])
      member x.CallMember(name, args) = call x name args