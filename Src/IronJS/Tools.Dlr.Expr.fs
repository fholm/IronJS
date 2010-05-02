namespace IronJS.Tools.Dlr

open IronJS
open IronJS.Aliases
open System.Linq.Expressions

(*Tools for working with DLR expressions*)
module Expr =

  //Defaults
  let void' = AstUtils.Empty() :> Et
  let default' typ = Et.Default(typ) :> Et
  let defaultT<'a> = default' typeof<'a>
  let null' = defaultT<ClrObject>

  //Variables
  let var name typ = Et.Variable(typ, name)
  let varT<'a> name = var name typeof<'a>
  let param name typ = Et.Parameter(typ, name)
  let paramT<'a> name = param name typeof<'a>
  
  //Values
  let constant value = Et.Constant(value, value.GetType()) :> Et
  let return' label (value:Et) = Et.Return(label, value) :> Et
  let assign (left:Et) (right:Et) = Et.Assign(left, right) :> Et

  //DEBUG
  let debug x =
    #if DEBUG
      constant x
    #else
      void'
    #endif

  //Constants
  let true' = constant true
  let false' = constant false
  let int0 = constant 0
  let int1 = constant 1

  //Labels
  let label (typ:ClrType) name = Et.Label(typ, name)
  let labelVoid = label typeof<System.Void>
  let labelT<'a> = label typeof<'a>
  let labelExpr label (typ:ClrType) = Et.Label(label, Et.Default(typ)) :> Et
  let labelExprT<'a> label = labelExpr label typeof<'a>
  let labelExprVal label (value:Et) = Et.Label(label, value) :> Et
  let labelExprVoid label = labelExprVal label (default' typeof<System.Void>)
  let dynamicLabelExpr label = Et.Label(label, null') :> Et
  let dynamicLabel = labelT<obj>

  let labelBreak() = Et.Label(typeof<System.Void>, "~break")
  let labelContinue() = Et.Label(typeof<System.Void>, "~continue")

  //Blocks
  let blockWithLocals (parms:EtParam seq) (exprs:Et seq) = 
    if Seq.length exprs = 0 then void' else Et.Block(parms, exprs) :> Et

  let block exprs = 
    blockWithLocals [] exprs

  let mutable private tmpCounter = 0

  let blockTmp typ (fn:EtParam -> Et list) = 
    tmpCounter <- tmpCounter + 1
    let tmp = Et.Parameter(typ, sprintf "~tmp_%i" tmpCounter)
    blockWithLocals [tmp] (fn tmp)

  let blockTmpT<'a> = 
    blockTmp typeof<'a>
    
  let blockTmpVarT<'a> (var:Et) fn = 
    if var :? EtParam 
      then block (fn var) 
      else 
        tmpCounter <- tmpCounter + 1
        let tmp = Et.Parameter(typeof<'a>, sprintf "~tmp_%i" tmpCounter)
        blockWithLocals [tmp] ([assign tmp var] @ (fn tmp))

  //Object fields and properties
  let field expr (name:string) = Et.Field(expr, name) :> Et
  let property expr (name:string) = Et.Property(expr, name) :> Et
  let propertyOrField expr (name:string) = Et.PropertyOrField(expr, name) :> Et

  //Static function call
  let callStatic (typ:ClrType) name (args:Et seq) = 
    match typ.GetMethod(name) with
    | null -> failwith "No method named '%s' found" name
    | m -> Et.Call(null, m, args) :> Et

  let callStaticT<'a> name (args:Et list) = 
    callStatic typeof<'a> name args

  let callStaticGeneric (typ:ClrType) (name:string) (typArgs:ClrType seq) (args:Et seq) =
    let argTypes = args |> Seq.map (fun x -> x.Type) |> Seq.toArray
    let concreteMethd = 
      match typ.GetMethod(name, argTypes) with
      | null -> failwith "No method named '%s' taking these type arguments found" name
      | m    -> m.MakeGenericMethod(Seq.toArray typArgs)

    Et.Call(null, concreteMethd, args) :> Et

  let callStaticGenericT<'a> name typArgs args = 
    callStaticGeneric typeof<'a> name typArgs args

  //Function call
  let call (expr:Et) name (args:Et list) =
    Et.Call(expr, expr.Type.GetMethod(name), args) :> Et

  //Casts
  let cast typ expr = Et.Convert(expr, typ) :> Et
  let castT<'a> = cast typeof<'a> 

  let castChk typ expr = Et.ConvertChecked(expr, typ) :> Et
  let castChkT<'a> = castChk typeof<'a> 

  let castAs typ expr = Et.TypeAs(expr, typ) :> Et
  let castAsT<'a> = castAs typeof<'a> 

  //Object creation
  let new' (typ:System.Type) = Et.New(typ) :> Et
  let newT<'a> = new' typeof<'a>

  let newGeneric (typ:System.Type) (types:ClrType seq) = 
    new' (typ.MakeGenericType(Seq.toArray types))

  let newGenericT<'a> = 
    newGeneric typedefof<'a>

  let newArgs (typ:System.Type) (args:Et seq) = 
    Et.New(Tools.Type.getCtor typ [for arg in args -> arg.Type], args) :> Et

  let newArgsT<'a> = 
    newArgs typeof<'a>

  let newGenericArgs (typ:System.Type) (types:ClrType seq) (args:Et seq) = 
    newArgs (typ.MakeGenericType(Seq.toArray types)) args

  let newGenericArgsT<'a> = 
    newGenericArgs typedefof<'a> 

  //Delegates
  let delegateType (types:ClrType seq) = Et.GetDelegateType(Seq.toArray types)
  let lambda (typ:ClrType) (parms:EtParam seq) (body:Et) = Et.Lambda(typ, body, parms)
  let invoke (func:Et) (args:Et seq) = Et.Invoke(func, args) :> Et

  //Dynamic
  let dynamic typ binder (args:Et seq) = Et.Dynamic(binder, typ, args) :> Et
  let dynamicT<'a> = dynamic typeof<'a>

  //Array
  let length target = Et.ArrayLength(target) :> Et
  let index target (index:Et) = Et.ArrayIndex(target, index) :> Et
  let access target (exprs:Et seq) = Et.ArrayAccess(target, exprs) :> Et

  let newArray typ (bounds:Et seq) = Et.NewArrayBounds(typ, bounds) :> Et
  let newArrayT<'a> = new' typeof<'a>

  let newArrayItems typ (exprs:Et seq) = Et.NewArrayInit(typ, exprs) :> Et
  let newArrayItemsT<'a> = newArrayItems typeof<'a>

  //Exceptions
  let throw (typ:System.Type) (args:Et seq) = Et.Throw(newArgs typ args) :> Et
  let catchVar (var:EtParam) body = Et.Catch(var, body)
  let catch (typ:ClrType) body = Et.Catch(typ, body)
  let catchT<'a> = catch typeof<'a>

  //Bit ops
  let bAnd' left right = Et.And(left, right) :> Et
  let bAndAsn left right = Et.AndAssign(left, right) :> Et

  let bOr' left right = Et.Or(left, right) :> Et
  let bOrAsn left right = Et.OrAssign(left, right) :> Et

  let xor left right = Et.ExclusiveOr(left, right) :> Et
  let xorAsn left right = Et.ExclusiveOrAssign(left, right) :> Et

  let rhs left right = Et.RightShift(left, right) :> Et
  let rhsAsn left right = Et.RightShiftAssign(left, right) :> Et

  let lhs left right = Et.LeftShift(left, right) :> Et
  let lhsAsn left right = Et.LeftShiftAssign(left, right) :> Et

  let cmpl target = (Et.Not target) :> Et

  //Control Flow
  let if' test ifTrue = Et.IfThen(test, ifTrue) :> Et
  let ifElse test ifTrue ifFalse = Et.IfThenElse(test, ifTrue, ifFalse) :> Et
  let ternary test ifTrue ifFalse = Et.Condition(test, ifTrue, ifFalse) :> Et
  let for' init test incr body = block [init; AstUtils.Loop(test, incr, body, void')]

  //Math
  let sub left right = Et.Subtract(left, right) :> Et
  let subChk left right = Et.SubtractChecked(left, right) :> Et
  let subAsn left right = Et.SubtractAssign(left, right) :> Et
  let subAsnChk left right = Et.SubtractAssignChecked(left, right) :> Et

  let add left right = Et.Add(left, right) :> Et
  let addChk left right = Et.AddChecked(left, right) :> Et
  let addAsn left right = Et.AddAssign(left, right) :> Et
  let addAsnChk left right = Et.AddAssignChecked(left, right) :> Et

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

  //Logical
  let or' left right = Et.OrElse(left, right) :> Et
  let orChain (c:Et list) = 
    match c with
    | [] -> true'
    | _  -> List.foldBack (fun x s -> or' x s) c.Tail c.Head 

  let and' left right = Et.AndAlso(left, right) :> Et
  let andChain (c:Et list) = 
    match c with
    | [] -> true'
    | _  -> List.foldBack (fun x s -> and' x s) c.Tail c.Head 

  let is' typ target = Et.TypeIs(target, typ) :> Et
  let isT<'a> = is' typeof<'a> 

  let typeEq target typ = Et.TypeEqual(target, typ) :> Et

  let isFalse target = Et.IsFalse(target) :> Et
  let isTrue target = Et.IsTrue(target) :> Et

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
