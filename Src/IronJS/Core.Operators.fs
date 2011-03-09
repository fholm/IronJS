namespace IronJS

open System
open IronJS
open IronJS.Support.Aliases

(*
//  This file implements all (non-assign) binary and unary operators
//  
//  DONE:
//  11.4.1 The delete Operator
//  11.4.2 The void Operator
//  11.4.3 The typeof Operator
//  11.4.6 Unary + Operator
//  11.4.7 Unary - Operator
//  11.4.8 Bitwise NOT Operator ( ~ )
//  11.4.9 Logical NOT Operator ( ! )
//  11.5.1 Applying the * Operator
//  11.5.2 Applying the / Operator
//  11.5.3 Applying the % Operator
//  11.6.1 The Addition operator ( + )
//  11.6.2 The Subtraction Operator ( - )
//  11.7.1 The Left Shift Operator ( << )
//  11.7.2 The Signed Right Shift Operator ( >> )
//  11.7.3 The Unsigned Right Shift Operator ( >>> )
//  11.8.1 The Less-than Operator ( < )
//  11.8.2 The Greater-than Operator ( > )
//  11.8.3 The Less-than-or-equal Operator ( <= )
//  11.8.4 The Greater-than-or-equal Operator ( >= )
//  11.8.6 The instanceof operator
//  11.8.7 The in operator
//  11.9.1 The Equals Operator ( == )
//  11.9.2 The Does-not-equals Operator ( != )
//  11.9.4 The Strict Equals Operator ( === )
//  11.9.5 The Strict Does-not-equal Operator ( !== )
//  11.10 Binary Bitwise Operators
//  11.11 Binary Logical Operators
//  11.12 Conditional Operator ( ?: )
*)

//------------------------------------------------------------------------------
// Operators
type Operators =

  //----------------------------------------------------------------------------
  // Unary
  //----------------------------------------------------------------------------

  //----------------------------------------------------------------------------
  // typeof
  static member typeOf (o:BoxedValue) = 
    if o.IsNumber then "number"
    elif o.IsNull then "object"
    else TypeTags.Names.[o.Tag]

  static member typeOf expr = Dlr.callStaticT<Operators> "typeOf" [expr]
  
  //----------------------------------------------------------------------------
  // !
  static member not (o) = Dlr.callStaticT<Operators> "not" [o]
  static member not (o:BoxedValue) =
    not (TypeConverter.ToBoolean o)
    
  //----------------------------------------------------------------------------
  // ~
  static member bitCmpl (o) = Dlr.callStaticT<Operators> "bitCmpl" [o]
  static member bitCmpl (o:BoxedValue) =
    let o = TypeConverter.ToNumber o
    let o = TypeConverter.ToInt32 o
    ~~~ o |> double
      
  //----------------------------------------------------------------------------
  // + (unary)
  static member plus (l, r) = Dlr.callStaticT<Operators> "plus" [l; r]
  static member plus (o:BoxedValue) =
    o |> TypeConverter.ToNumber |> BV.Box
    
  //----------------------------------------------------------------------------
  // - (unary)
  static member minus (l, r) = Dlr.callStaticT<Operators> "minus" [l; r]
  static member minus (o:BoxedValue) =
    BV.Box ((TypeConverter.ToNumber o) * -1.0)

  //----------------------------------------------------------------------------
  // Binary
  //----------------------------------------------------------------------------
  
  //----------------------------------------------------------------------------
  // in
  static member in' (env, l,r) = Dlr.callStaticT<Operators> "in'" [env; l; r]
  static member in' (env:Environment, l:BoxedValue, r:BoxedValue) = 
    if not r.IsObject then
      env.RaiseTypeError("Right operand is not a object")

    let mutable index = 0u
    if CoreUtils.TryConvertToIndex(l, &index) then
      r.Object.Has(index)

    else
      let name = TypeConverter.ToString(l)
      r.Object.Has(name)
    
  //----------------------------------------------------------------------------
  // instanceof
  static member instanceOf (env, l,r) = 
    Dlr.callStaticT<Operators> "instanceOf" [env; l; r]

  static member instanceOf(env:Environment, l:BoxedValue, r:BoxedValue) =
    if r.IsFunction |> not then
      env.RaiseTypeError("Right operand is not a function")

    if not l.IsObject
      then false
      else r.Func.HasInstance(l.Object)
    
  //----------------------------------------------------------------------------
  // <
  static member lt (l, r) = Dlr.callStaticT<Operators> "lt" [l; r]
  static member lt (l:BoxedValue, r:BoxedValue) =
    if l.IsNumber && r.IsNumber
      then l.Number < r.Number
      elif l.Tag = TypeTags.String && r.Tag = TypeTags.String
        then l.String < r.String
        else TypeConverter.ToNumber l < TypeConverter.ToNumber r
        
  //----------------------------------------------------------------------------
  // <=
  static member ltEq (l, r) = Dlr.callStaticT<Operators> "ltEq" [l; r]
  static member ltEq (l:BoxedValue, r:BoxedValue) =
    if l.IsNumber && r.IsNumber
      then l.Number <= r.Number
      elif l.Tag = TypeTags.String && r.Tag = TypeTags.String
        then l.String <= r.String
        else TypeConverter.ToNumber l <= TypeConverter.ToNumber r
        
  //----------------------------------------------------------------------------
  // >
  static member gt (l, r) = Dlr.callStaticT<Operators> "gt" [l; r]
  static member gt (l:BoxedValue, r:BoxedValue) =
    if l.IsNumber && r.IsNumber
      then l.Number > r.Number
      elif l.Tag = TypeTags.String && r.Tag = TypeTags.String
        then l.String > r.String
        else TypeConverter.ToNumber l > TypeConverter.ToNumber r
        
  //----------------------------------------------------------------------------
  // >=
  static member gtEq (l, r) = Dlr.callStaticT<Operators> "gtEq" [l; r]
  static member gtEq (l:BoxedValue, r:BoxedValue) =
    if l.IsNumber && r.IsNumber
      then l.Number >= r.Number
      elif l.Tag = TypeTags.String && r.Tag = TypeTags.String
        then l.String >= r.String
        else TypeConverter.ToNumber l >= TypeConverter.ToNumber r
        
  //----------------------------------------------------------------------------
  // ==
  static member eq (l, r) = Dlr.callStaticT<Operators> "eq" [l; r]
  static member eq (l:BoxedValue, r:BoxedValue) = 
    if l.IsNumber && r.IsNumber then
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

      elif l.IsNumber && r.Tag = TypeTags.String then
        l.Number = TypeConverter.ToNumber r.String
        
      elif r.Tag = TypeTags.String && r.IsNumber then
        TypeConverter.ToNumber l.String = r.Number

      elif l.Tag = TypeTags.Bool then
        let mutable l = BV.Box(TypeConverter.ToNumber l)
        Operators.eq(l, r)

      elif r.Tag = TypeTags.Bool then
        let mutable r = BV.Box(TypeConverter.ToNumber r)
        Operators.eq(l, r)

      elif r.Tag >= TypeTags.Object then
        match l.Tag with
        | TypeTags.String -> 
          let r = TypeConverter.ToPrimitive(r.Object, DefaultValueHint.None)
          Operators.eq(l, r)

        | _ -> 
          if l.IsNumber then
            let r = TypeConverter.ToPrimitive(r.Object, DefaultValueHint.None)
            Operators.eq(l, r)
          else
            false

      elif l.Tag >= TypeTags.Object then
        match r.Tag with
        | TypeTags.String -> 
          let l = TypeConverter.ToPrimitive(l.Object, DefaultValueHint.None)
          Operators.eq(l, r)

        | _ -> 
          if r.IsNumber then
            let l = TypeConverter.ToPrimitive(l.Object, DefaultValueHint.None)
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
    if l.IsNumber && r.IsNumber then
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
    if l.IsNumber && r.IsNumber then
      BV.Box (l.Number + r.Number)

    elif l.Tag = TypeTags.String || r.Tag = TypeTags.String then
      BV.Box (TypeConverter.ToString(l) + TypeConverter.ToString(r))

    else
      BV.Box (TypeConverter.ToNumber(l) + TypeConverter.ToNumber(r))
      
  //----------------------------------------------------------------------------
  // -
  static member sub (l, r) = Dlr.callStaticT<Operators> "sub" [l; r]
  static member sub (l:BoxedValue, r:BoxedValue) =
    if l.IsNumber && r.IsNumber
      then BV.Box (l.Number - r.Number)
      else BV.Box (TypeConverter.ToNumber l - TypeConverter.ToNumber r)
      
  //----------------------------------------------------------------------------
  // /
  static member div (l, r) = Dlr.callStaticT<Operators> "div" [l; r]
  static member div (l:BoxedValue, r:BoxedValue) =
    if l.IsNumber && r.IsNumber
      then BV.Box (l.Number / r.Number)
      else BV.Box (TypeConverter.ToNumber l / TypeConverter.ToNumber r)
      
  //----------------------------------------------------------------------------
  // *
  static member mul (l, r) = Dlr.callStaticT<Operators> "mul" [l; r]
  static member mul (l:BoxedValue, r:BoxedValue) =
    if l.IsNumber && r.IsNumber
      then BV.Box (l.Number * r.Number)
      else BV.Box (TypeConverter.ToNumber l * TypeConverter.ToNumber r)
      
  //----------------------------------------------------------------------------
  // %
  static member mod' (l, r) = Dlr.callStaticT<Operators> "mod'" [l; r]
  static member mod' (l:BoxedValue, r:BoxedValue) =
    if l.IsNumber && r.IsNumber
      then BV.Box (l.Number % r.Number)
      else BV.Box (TypeConverter.ToNumber l % TypeConverter.ToNumber r)
    
  //----------------------------------------------------------------------------
  // &
  static member bitAnd (l, r) = Dlr.callStaticT<Operators> "bitAnd" [l; r]
  static member bitAnd (l:BoxedValue, r:BoxedValue) =
    let l = TypeConverter.ToNumber l
    let r = TypeConverter.ToNumber r
    let l = TypeConverter.ToInt32 l
    let r = TypeConverter.ToInt32 r
    (l &&& r) |> double
    
  //----------------------------------------------------------------------------
  // |
  static member bitOr (l, r) = Dlr.callStaticT<Operators> "bitOr" [l; r]
  static member bitOr (l:BoxedValue, r:BoxedValue) =
    let l = TypeConverter.ToNumber l
    let r = TypeConverter.ToNumber r
    let l = TypeConverter.ToInt32 l
    let r = TypeConverter.ToInt32 r
    (l ||| r) |> double
    
  //----------------------------------------------------------------------------
  // ^
  static member bitXOr (l, r) = Dlr.callStaticT<Operators> "bitXOr" [l; r]
  static member bitXOr (l:BoxedValue, r:BoxedValue) =
    let l = TypeConverter.ToNumber l
    let r = TypeConverter.ToNumber r
    let l = TypeConverter.ToInt32 l
    let r = TypeConverter.ToInt32 r
    (l ^^^ r) |> double
    
  //----------------------------------------------------------------------------
  // <<
  static member bitLhs (l, r) = Dlr.callStaticT<Operators> "bitLhs" [l; r]
  static member bitLhs (l:BoxedValue, r:BoxedValue) =
    let l = TypeConverter.ToNumber l
    let r = TypeConverter.ToNumber r
    let l = TypeConverter.ToInt32 l
    let r = TypeConverter.ToUInt32 r &&& 0x1Fu
    (l <<< int r) |> double
    
  //----------------------------------------------------------------------------
  // >>
  static member bitRhs (l, r) = Dlr.callStaticT<Operators> "bitRhs" [l; r]
  static member bitRhs (l:BoxedValue, r:BoxedValue) =
    let l = TypeConverter.ToNumber l
    let r = TypeConverter.ToNumber r
    let l = TypeConverter.ToInt32 l
    let r = TypeConverter.ToUInt32 r &&& 0x1Fu
    (l >>> int r) |> double
    
  //----------------------------------------------------------------------------
  // >>>
  static member bitURhs (l, r) = Dlr.callStaticT<Operators> "bitURhs" [l; r]
  static member bitURhs (l:BoxedValue, r:BoxedValue) =
    let l = TypeConverter.ToNumber l
    let r = TypeConverter.ToNumber r
    let l = TypeConverter.ToUInt32 l
    let r = TypeConverter.ToUInt32 r &&& 0x1Fu
    (l >>> int r) |> double
    
  //----------------------------------------------------------------------------
  // &&
  static member and' (l, r) = Dlr.callStaticT<Operators> "and'" [l; r]
  static member and' (l:BoxedValue, r:BoxedValue) =
    if not (TypeConverter.ToBoolean l) then l else r
    
  //----------------------------------------------------------------------------
  // ||
  static member or' (l, r) = Dlr.callStaticT<Operators> "or'" [l; r]
  static member or' (l:BoxedValue, r:BoxedValue) =
    if TypeConverter.ToBoolean l then l else r

module ExtOperators =
  
  let (?<-) (a:'a when 'a :> CO) (b:string) (c:FO) = a.Put(b, c)