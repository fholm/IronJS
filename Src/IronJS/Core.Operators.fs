namespace IronJS.Api

open System
open IronJS
open IronJS.Aliases
open IronJS.Utils.Patterns

//------------------------------------------------------------------------------
// Operators
type Operators =

  //----------------------------------------------------------------------------
  // Unary
  //----------------------------------------------------------------------------

  //----------------------------------------------------------------------------
  // typeof
  static member typeOf (o:BoxedValue) = 
    match o with
    | IsNumber _ -> "number" 
    | IsNull -> "object"
    | _ -> TypeTags.Names.[o.Tag]

  static member typeOf expr = Dlr.callStaticT<Operators> "typeOf" [expr]
  
  //----------------------------------------------------------------------------
  // !
  static member not (o) = Dlr.callStaticT<Operators> "not" [o]
  static member not (o:BoxedValue) =
    not (TypeConverter2.ToBoolean o)
    
  //----------------------------------------------------------------------------
  // ~
  static member bitCmpl (o) = Dlr.callStaticT<Operators> "bitCmpl" [o]
  static member bitCmpl (o:BoxedValue) =
    let o = TypeConverter2.ToNumber o
    let o = TypeConverter2.ToInt32 o
    ~~~ o |> double
      
  //----------------------------------------------------------------------------
  // + (unary)
  static member plus (l, r) = Dlr.callStaticT<Operators> "plus" [l; r]
  static member plus (o:BoxedValue) =
    Utils.boxNumber (TypeConverter2.ToNumber o)
    
  //----------------------------------------------------------------------------
  // - (unary)
  static member minus (l, r) = Dlr.callStaticT<Operators> "minus" [l; r]
  static member minus (o:BoxedValue) =
    Utils.boxNumber ((TypeConverter2.ToNumber o) * -1.0)

  //----------------------------------------------------------------------------
  // Binary
  //----------------------------------------------------------------------------

  // in
  static member in' (env, l,r) = Dlr.callStaticT<Operators> "in'" [env; l; r]
  static member in' (env:Environment, l:BoxedValue, r:BoxedValue) = 
    if Utils.Box.isObject r.Tag |> not then
      env.RaiseTypeError("Right operand is not a object")

    match l with
    | IsIndex i -> r.Object.Has(i)
    | _ -> let name = TypeConverter2.ToString(l) in r.Object.Has(name)

  // instanceof
  static member instanceOf (env, l,r) = 
    Dlr.callStaticT<Operators> "instanceOf" [env; l; r]

  static member instanceOf(env:Environment, l:BoxedValue, r:BoxedValue) =
    if Utils.Box.isFunction r.Tag |> not then
      env.RaiseTypeError("Right operand is not a function")

    if Utils.Box.isObject l.Tag |> not 
      then false
      else r.Func.HasInstance(l.Object)
    
  //----------------------------------------------------------------------------
  // <
  static member lt (l, r) = Dlr.callStaticT<Operators> "lt" [l; r]
  static member lt (l:BoxedValue, r:BoxedValue) =
    if Utils.Box.isBothNumber l.Marker r.Marker
      then l.Number < r.Number
      elif l.Tag = TypeTags.String && r.Tag = TypeTags.String
        then l.String < r.String
        else TypeConverter2.ToNumber l < TypeConverter2.ToNumber r
        
  //----------------------------------------------------------------------------
  // <=
  static member ltEq (l, r) = Dlr.callStaticT<Operators> "ltEq" [l; r]
  static member ltEq (l:BoxedValue, r:BoxedValue) =
    if Utils.Box.isBothNumber l.Marker r.Marker
      then l.Number <= r.Number
      elif l.Tag = TypeTags.String && r.Tag = TypeTags.String
        then l.String <= r.String
        else TypeConverter2.ToNumber l <= TypeConverter2.ToNumber r
        
  //----------------------------------------------------------------------------
  // >
  static member gt (l, r) = Dlr.callStaticT<Operators> "gt" [l; r]
  static member gt (l:BoxedValue, r:BoxedValue) =
    if Utils.Box.isBothNumber l.Marker r.Marker
      then l.Number > r.Number
      elif l.Tag = TypeTags.String && r.Tag = TypeTags.String
        then l.String > r.String
        else TypeConverter2.ToNumber l > TypeConverter2.ToNumber r
        
  //----------------------------------------------------------------------------
  // >=
  static member gtEq (l, r) = Dlr.callStaticT<Operators> "gtEq" [l; r]
  static member gtEq (l:BoxedValue, r:BoxedValue) =
    if Utils.Box.isBothNumber l.Marker r.Marker
      then l.Number >= r.Number
      elif l.Tag = TypeTags.String && r.Tag = TypeTags.String
        then l.String >= r.String
        else TypeConverter2.ToNumber l >= TypeConverter2.ToNumber r
        
  //----------------------------------------------------------------------------
  // ==
  static member eq (l, r) = Dlr.callStaticT<Operators> "eq" [l; r]
  static member eq (l:BoxedValue, r:BoxedValue) = 
    if Utils.Box.isNumber l.Marker && Utils.Box.isNumber r.Marker then
      l.Number = r.Number

    elif l.Tag = r.Tag then
      match l.Tag with
      | TypeTags.Undefined -> true
      | TypeTags.String -> l.String = r.String
      | TypeTags.Bool -> l.Bool = r.Bool
      | TypeTags.Clr
      | TypeTags.Function
      | TypeTags.Object -> Object.ReferenceEquals(l.Clr, r.Clr)
      | _ -> false

    else
      if l.Tag = TypeTags.Clr 
        && l.Clr = null 
        && r.Tag = TypeTags.Undefined then true
      
      elif r.Tag = TypeTags.Clr 
        && r.Clr = null 
        && l.Tag = TypeTags.Undefined then true

      elif Utils.Box.isNumber l.Marker && r.Tag = TypeTags.String then
        l.Number = TypeConverter2.ToNumber r.String
        
      elif r.Tag = TypeTags.String && Utils.Box.isNumber r.Marker then
        TypeConverter2.ToNumber l.String = r.Number

      elif l.Tag = TypeTags.Bool then
        let mutable l = Utils.boxNumber(TypeConverter2.ToNumber l)
        Operators.eq(l, r)

      elif r.Tag = TypeTags.Bool then
        let mutable r = Utils.boxNumber(TypeConverter2.ToNumber r)
        Operators.eq(l, r)

      elif r.Tag >= TypeTags.Object then
        match l.Tag with
        | TypeTags.String -> 
          let r = TypeConverter2.ToPrimitive(r.Object, DefaultValue.None)
          Operators.eq(l, r)

        | _ -> 
          if Utils.Box.isNumber l.Marker then
            let r = TypeConverter2.ToPrimitive(r.Object, DefaultValue.None)
            Operators.eq(l, r)
          else
            false

      elif l.Tag >= TypeTags.Object then
        match r.Tag with
        | TypeTags.String -> 
          let l = TypeConverter2.ToPrimitive(l.Object, DefaultValue.None)
          Operators.eq(l, r)

        | _ -> 
          if Utils.Box.isNumber r.Marker then
            let l = TypeConverter2.ToPrimitive(l.Object, DefaultValue.None)
            Operators.eq(l, r)
          else
            false

      else
        false
        
  //----------------------------------------------------------------------------
  // !=
  static member notEq (l, r) = Dlr.callStaticT<Operators> "notEq" [l; r]
  static member notEq (l:BoxedValue, r:BoxedValue) = not (Operators.eq(l, r))
  
  //----------------------------------------------------------------------------
  // ===
  static member same (l, r) = Dlr.callStaticT<Operators> "same" [l; r]
  static member same (l:BoxedValue, r:BoxedValue) = 
    if Utils.Box.isBothNumber l.Marker r.Marker then
      l.Number = r.Number

    elif l.Tag = r.Tag then
      match l.Tag with
      | TypeTags.Undefined -> true
      | TypeTags.String -> l.String = r.String
      | TypeTags.Bool -> l.Bool = r.Bool
      | TypeTags.Clr
      | TypeTags.Function
      | TypeTags.Object -> Object.ReferenceEquals(l.Clr, r.Clr)
      | _ -> false

    else
      false
      
  //----------------------------------------------------------------------------
  // !==
  static member notSame (l, r) = Dlr.callStaticT<Operators> "notSame" [l; r]
  static member notSame (l:BoxedValue, r:BoxedValue) =
    Operators.same(l, r) |> not
    
  //----------------------------------------------------------------------------
  // +
  static member add (l, r) = Dlr.callStaticT<Operators> "add" [l; r]
  static member add (l:BoxedValue, r:BoxedValue) = 
    if Utils.Box.isBothNumber l.Marker r.Marker then
      Utils.boxNumber (l.Number + r.Number)

    elif l.Tag = TypeTags.String || r.Tag = TypeTags.String then
      Utils.boxString (TypeConverter2.ToString(l) + TypeConverter2.ToString(r))

    else
      Utils.boxNumber (TypeConverter2.ToNumber(l) + TypeConverter2.ToNumber(r))
      
  //----------------------------------------------------------------------------
  // -
  static member sub (l, r) = Dlr.callStaticT<Operators> "sub" [l; r]
  static member sub (l:BoxedValue, r:BoxedValue) =
    if Utils.Box.isBothNumber l.Marker r.Marker 
      then Utils.boxNumber (l.Number - r.Number)
      else Utils.boxNumber (TypeConverter2.ToNumber l - TypeConverter2.ToNumber r)
      
  //----------------------------------------------------------------------------
  // /
  static member div (l, r) = Dlr.callStaticT<Operators> "div" [l; r]
  static member div (l:BoxedValue, r:BoxedValue) =
    if Utils.Box.isBothNumber l.Marker r.Marker
      then Utils.boxNumber (l.Number / r.Number)
      else Utils.boxNumber (TypeConverter2.ToNumber l / TypeConverter2.ToNumber r)
      
  //----------------------------------------------------------------------------
  // *
  static member mul (l, r) = Dlr.callStaticT<Operators> "mul" [l; r]
  static member mul (l:BoxedValue, r:BoxedValue) =
    if Utils.Box.isBothNumber l.Marker r.Marker
      then Utils.boxNumber (l.Number * r.Number)
      else Utils.boxNumber (TypeConverter2.ToNumber l * TypeConverter2.ToNumber r)
      
  //----------------------------------------------------------------------------
  // %
  static member mod' (l, r) = Dlr.callStaticT<Operators> "mod'" [l; r]
  static member mod' (l:BoxedValue, r:BoxedValue) =
    if Utils.Box.isBothNumber l.Marker r.Marker
      then Utils.boxNumber (l.Number % r.Number)
      else Utils.boxNumber (TypeConverter2.ToNumber l % TypeConverter2.ToNumber r)
    
  //----------------------------------------------------------------------------
  // &
  static member bitAnd (l, r) = Dlr.callStaticT<Operators> "bitAnd" [l; r]
  static member bitAnd (l:BoxedValue, r:BoxedValue) =
    let l = TypeConverter2.ToNumber l
    let r = TypeConverter2.ToNumber r
    let l = TypeConverter2.ToInt32 l
    let r = TypeConverter2.ToInt32 r
    (l &&& r) |> double
    
  //----------------------------------------------------------------------------
  // |
  static member bitOr (l, r) = Dlr.callStaticT<Operators> "bitOr" [l; r]
  static member bitOr (l:BoxedValue, r:BoxedValue) =
    let l = TypeConverter2.ToNumber l
    let r = TypeConverter2.ToNumber r
    let l = TypeConverter2.ToInt32 l
    let r = TypeConverter2.ToInt32 r
    (l ||| r) |> double
    
  //----------------------------------------------------------------------------
  // ^
  static member bitXOr (l, r) = Dlr.callStaticT<Operators> "bitXOr" [l; r]
  static member bitXOr (l:BoxedValue, r:BoxedValue) =
    let l = TypeConverter2.ToNumber l
    let r = TypeConverter2.ToNumber r
    let l = TypeConverter2.ToInt32 l
    let r = TypeConverter2.ToInt32 r
    (l ^^^ r) |> double
    
  //----------------------------------------------------------------------------
  // <<
  static member bitLhs (l, r) = Dlr.callStaticT<Operators> "bitLhs" [l; r]
  static member bitLhs (l:BoxedValue, r:BoxedValue) =
    let l = TypeConverter2.ToNumber l
    let r = TypeConverter2.ToNumber r
    let l = TypeConverter2.ToInt32 l
    let r = TypeConverter2.ToUInt32 r &&& 0x1Fu
    (l <<< int r) |> double
    
  //----------------------------------------------------------------------------
  // >>
  static member bitRhs (l, r) = Dlr.callStaticT<Operators> "bitRhs" [l; r]
  static member bitRhs (l:BoxedValue, r:BoxedValue) =
    let l = TypeConverter2.ToNumber l
    let r = TypeConverter2.ToNumber r
    let l = TypeConverter2.ToInt32 l
    let r = TypeConverter2.ToUInt32 r &&& 0x1Fu
    (l >>> int r) |> double
    
  //----------------------------------------------------------------------------
  // >>>
  static member bitURhs (l, r) = Dlr.callStaticT<Operators> "bitURhs" [l; r]
  static member bitURhs (l:BoxedValue, r:BoxedValue) =
    let l = TypeConverter2.ToNumber l
    let r = TypeConverter2.ToNumber r
    let l = TypeConverter2.ToUInt32 l
    let r = TypeConverter2.ToUInt32 r &&& 0x1Fu
    (l >>> int r) |> double
    
  //----------------------------------------------------------------------------
  // &&
  static member and' (l, r) = Dlr.callStaticT<Operators> "and'" [l; r]
  static member and' (l:BoxedValue, r:BoxedValue) =
    if not (TypeConverter2.ToBoolean l) then l else r
    
  //----------------------------------------------------------------------------
  // ||
  static member or' (l, r) = Dlr.callStaticT<Operators> "or'" [l; r]
  static member or' (l:BoxedValue, r:BoxedValue) =
    if TypeConverter2.ToBoolean l then l else r
    
//------------------------------------------------------------------------------
// HostFunction API
module HostFunction =

  [<ReferenceEquality>]
  type DispatchTarget<'a when 'a :> Delegate> = {
    Delegate : System.Type
    Function : HostFunction<'a>
    Invoke: Dlr.Expr -> Dlr.Expr seq -> Dlr.Expr
  }

  //----------------------------------------------------------------------------
  let marshalArgs (args:Dlr.ExprParam array) (env:Dlr.Expr) i t =
    if i < args.Length 
      then TypeConverter2.ConvertTo(env, args.[i], t)
      else
        if FSKit.Utils.isTypeT<BoxedValue> t
          then Expr.BoxedConstants.undefined else Dlr.default' t
      
  //----------------------------------------------------------------------------
  let marshalBoxParams (f:HostFunction<_>) args m =
    args
    |> Seq.skip f.ArgTypes.Length
    |> Seq.map Expr.box
    |> fun x -> Seq.append m [Dlr.newArrayItemsT<BoxedValue> x]
    
  //----------------------------------------------------------------------------
  let marshalObjectParams (f:HostFunction<_>) (args:Dlr.ExprParam array) m =
    args
    |> Seq.skip f.ArgTypes.Length
    |> Seq.map TypeConverter2.ToClrObject
    |> fun x -> Seq.append m [Dlr.newArrayItemsT<System.Object> x]
    
  //----------------------------------------------------------------------------
  let private createParam i t = Dlr.param (sprintf "a%i" i) t
  
  //----------------------------------------------------------------------------
  let private addEmptyParamsObject<'a> (args:Dlr.ExprParam array) =
    args |> Array.map (fun x -> x :> Dlr.Expr)
         |> FSKit.Array.appendOne Dlr.newArrayEmptyT<'a> 
         |> Seq.ofArray
  
  //----------------------------------------------------------------------------
  let compileDispatcher (target:DispatchTarget<'a>) = 
    let f = target.Function

    let argTypes = FSKit.Reflection.getDelegateArgTypes target.Delegate
    let args = argTypes |> Array.mapi createParam
    let passedArgs = args |> Seq.skip f.MarshalMode |> Array.ofSeq

    let func = args.[0] :> Dlr.Expr
    let env = Dlr.field func "Env"

    let marshalled = f.ArgTypes |> Seq.mapi (marshalArgs passedArgs env)
    let marshalled = 
      let paramsExist = f.ArgTypes.Length < passedArgs.Length 

      match f.ParamsMode with
      | ParamsModes.BoxParams -> 
        if paramsExist
          then marshalBoxParams f passedArgs marshalled
          else addEmptyParamsObject<BoxedValue> passedArgs 

      | ParamsModes.ObjectParams when paramsExist -> 
        if paramsExist
          then marshalObjectParams f passedArgs marshalled
          else addEmptyParamsObject<System.Object> passedArgs 

      | _ -> marshalled

    let invoke = target.Invoke func marshalled
    let body = 
      if FSKit.Utils.isTypeT<BoxedValue> f.ReturnType 
        then invoke
        elif FSKit.Utils.isVoid f.ReturnType 
          then Expr.voidAsUndefined invoke
          else Expr.box invoke
            
    let lambda = Dlr.lambda target.Delegate args body
    Debug.printExpr lambda
    lambda.Compile()

  //----------------------------------------------------------------------------
  let generateInvoke<'a when 'a :> Delegate> f args =
    let casted = Dlr.castT<HostFunction<'a>> f
    Dlr.invoke (Dlr.field casted "Delegate") args
  
  //----------------------------------------------------------------------------
  let compile<'a when 'a :> Delegate> (x:FunctionObject) (delegate':System.Type) =
    compileDispatcher {
      Delegate = delegate'
      Function = x :?> HostFunction<'a>
      Invoke = generateInvoke<'a>
    }
    
  //----------------------------------------------------------------------------
  let create (env:Environment) (delegate':'a) =
    let h = HostFunction<'a>(env, delegate')
    let f = h :> FunctionObject
    let o = f :> CommonObject

    o.Put("length", double h.ArgsLength, DescriptorAttrs.Immutable)
    env.AddCompiler(f, compile<'a>)

    f

//------------------------------------------------------------------------------
module DynamicScope =
  
  //----------------------------------------------------------------------------
  let findObject (name:string) (dc:DynamicScope) stop =
    let rec find (dc:DynamicScope) =
      match dc with
      | [] -> None
      | (level, o)::xs ->
        if level >= stop then
          let mutable h = null
          let mutable i = 0
          if o.Has(name)
            then Some o
            else find xs
        else
          None

    find dc
      
  //----------------------------------------------------------------------------
  let findVariable (name:string) (dc:DynamicScope) stop = 
    match findObject name dc stop with
    | Some o -> Some(o.Get(name))
    | _ -> None

  //----------------------------------------------------------------------------
  let get (name:string) dc stop (g:CommonObject) (s:Scope) i =
    match findObject name dc stop with
    | Some o -> o.Get(name)
    | _ -> if s = null then g.Get(name) else s.[i]
      
  //----------------------------------------------------------------------------
  let set (name:string) (v:BoxedValue) dc stop (g:CommonObject) (s:Scope) i =
    match findObject name dc stop with
    | Some o -> o.Put(name, v)
    | _ -> 
      if s = null 
        then g.Put(name, v)
        else s.[i] <- v
          
  //----------------------------------------------------------------------------
  let call<'a when 'a :> Delegate> (name:string) args dc stop g (s:Scope) i =

    let this, func = 
      match findObject name dc stop with
      | Some o -> o, o.Get(name)
      | _ -> g, if s=null then g.Get(name) else s.[i]

    if func.Tag >= TypeTags.Function then
      let func = func.Func
      let internalArgs = [|func :> obj; this :> obj|]
      let compiled = func.Compiler.Compile<'a>(func)
      Utils.box (compiled.DynamicInvoke(Array.append internalArgs args))

    else
      Errors.runtime "Can only call javascript function dynamically"
        
  //----------------------------------------------------------------------------
  let delete (dc:DynamicScope) (g:CommonObject) (name:string) =
    match findObject name dc -1 with
    | Some o -> o.Delete(name)
    | _ -> g.Delete(name)

  //----------------------------------------------------------------------------
  let push (dc:DynamicScope byref) new' level =
    dc <- (level, new') :: dc
      
  //----------------------------------------------------------------------------
  let pop (dc:DynamicScope byref) =
    dc <- List.tail dc

  module Reflected =
    let set = Utils.Reflected.methodInfo "Api.DynamicScope" "set"
    let get = Utils.Reflected.methodInfo "Api.DynamicScope" "get"
    let call = Utils.Reflected.methodInfo "Api.DynamicScope" "call"
    let delete = Utils.Reflected.methodInfo "Api.DynamicScope" "delete"
    let push = Utils.Reflected.methodInfo "Api.DynamicScope" "push"
    let pop = Utils.Reflected.methodInfo "Api.DynamicScope" "pop"