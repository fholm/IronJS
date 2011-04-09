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
  static member typeOf (o:BV) = 
    if o.IsNumber then "number"
    elif o.IsNull then "object"
    else TypeTags.Names.[o.Tag]

  static member typeOf expr = Dlr.callStaticT<Operators> "typeOf" [expr]
  
  //----------------------------------------------------------------------------
  // !
  static member not (o) = Dlr.callStaticT<Operators> "not" [o]
  static member not (o:BoxedValue) =
    not (TC.ToBoolean o)
    
  //----------------------------------------------------------------------------
  // ~
  static member bitCmpl (o) = Dlr.callStaticT<Operators> "bitCmpl" [o]
  static member bitCmpl (o:BV) =
    let o = TC.ToNumber o
    let o = TC.ToInt32 o
    ~~~ o |> double
      
  //----------------------------------------------------------------------------
  // + (unary)
  static member plus (l, r) = Dlr.callStaticT<Operators> "plus" [l; r]
  static member plus (o:BV) =
    o |> TC.ToNumber |> BV.Box
    
  //----------------------------------------------------------------------------
  // - (unary)
  static member minus (l, r) = Dlr.callStaticT<Operators> "minus" [l; r]
  static member minus (o:BV) =
    BV.Box ((TC.ToNumber o) * -1.0)

  //----------------------------------------------------------------------------
  // Binary
  //----------------------------------------------------------------------------
  
  //----------------------------------------------------------------------------
  // in
  static member in' (env, l,r) = Dlr.callStaticT<Operators> "in'" [env; l; r]
  static member in' (env:Env, l:BV, r:BV) = 
    if not r.IsObject then
      env.RaiseTypeError("Right operand is not a object")

    let mutable index = 0u
    if TC.TryToIndex(l, &index) then
      r.Object.Has(index)

    else
      let name = TC.ToString(l)
      r.Object.Has(name)
    
  //----------------------------------------------------------------------------
  // instanceof
  static member instanceOf (env, l,r) = 
    Dlr.callStaticT<Operators> "instanceOf" [env; l; r]

  static member instanceOf(env:Env, l:BV, r:BV) =
    if r.IsFunction |> not then
      env.RaiseTypeError("Right operand is not a function")

    if not l.IsObject
      then false
      else r.Func.HasInstance(l.Object)
    
  //----------------------------------------------------------------------------
  // <
  static member lt (l, r) = Dlr.callStaticT<Operators> "lt" [l; r]
  static member lt (l:BV, r:BV) =
    if l.IsNumber && r.IsNumber
      then l.Number < r.Number
      elif l.Tag = TypeTags.String && r.Tag = TypeTags.String
        then l.String < r.String
        else
          let l' = TC.ToPrimitive(l, DefaultValueHint.Number)
          let r' = TC.ToPrimitive(r, DefaultValueHint.Number)
          if l'.IsString && r'.IsString
            then l'.String < r'.String
            else TC.ToNumber l' < TC.ToNumber r'
        
  //----------------------------------------------------------------------------
  // <=
  static member ltEq (l, r) = Dlr.callStaticT<Operators> "ltEq" [l; r]
  static member ltEq (l:BV, r:BV) =
    if l.IsNumber && r.IsNumber
      then l.Number <= r.Number
      elif l.Tag = TypeTags.String && r.Tag = TypeTags.String
        then l.String <= r.String
        else
          let r' = TC.ToPrimitive(r, DefaultValueHint.Number)
          let l' = TC.ToPrimitive(l, DefaultValueHint.Number)
          if l'.IsString && r'.IsString
            then l'.String <= r'.String
            else TC.ToNumber l' <= TC.ToNumber r'
        
  //----------------------------------------------------------------------------
  // >
  static member gt (l, r) = Dlr.callStaticT<Operators> "gt" [l; r]
  static member gt (l:BV, r:BV) =
    if l.IsNumber && r.IsNumber
      then l.Number > r.Number
      elif l.Tag = TypeTags.String && r.Tag = TypeTags.String
        then l.String > r.String
        else
          let r' = TC.ToPrimitive(r, DefaultValueHint.Number)
          let l' = TC.ToPrimitive(l, DefaultValueHint.Number)
          if l'.IsString && r'.IsString
            then l'.String > r'.String
            else TC.ToNumber l' > TC.ToNumber r'
        
  //----------------------------------------------------------------------------
  // >=
  static member gtEq (l, r) = Dlr.callStaticT<Operators> "gtEq" [l; r]
  static member gtEq (l:BV, r:BV) =
    if l.IsNumber && r.IsNumber
      then l.Number >= r.Number
      elif l.Tag = TypeTags.String && r.Tag = TypeTags.String
        then l.String >= r.String
        else
          let l' = TC.ToPrimitive(l, DefaultValueHint.Number)
          let r' = TC.ToPrimitive(r, DefaultValueHint.Number)
          if l'.IsString && r'.IsString
            then l'.String >= r'.String
            else TC.ToNumber l' >= TC.ToNumber r'
        
  //----------------------------------------------------------------------------
  // ==
  static member eq (l, r) = Dlr.callStaticT<Operators> "eq" [l; r]
  static member eq (l:BV, r:BV) = 
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

      elif l.IsNumber && r.IsString then
        l.Number = TC.ToNumber r.String
        
      elif l.IsString && r.IsNumber then
        TC.ToNumber l.String = r.Number

      elif l.Tag = TypeTags.Bool then
        let mutable l = BV.Box(TC.ToNumber l)
        Operators.eq(l, r)

      elif r.Tag = TypeTags.Bool then
        let mutable r = BV.Box(TC.ToNumber r)
        Operators.eq(l, r)

      elif r.Tag >= TypeTags.Object then
        match l.Tag with
        | TypeTags.String -> 
          let r = TC.ToPrimitive(r.Object, DefaultValueHint.None)
          Operators.eq(l, r)

        | _ -> 
          if l.IsNumber then
            let r = TC.ToPrimitive(r.Object, DefaultValueHint.None)
            Operators.eq(l, r)
          else
            false

      elif l.Tag >= TypeTags.Object then
        match r.Tag with
        | TypeTags.String -> 
          let l = TC.ToPrimitive(l.Object, DefaultValueHint.None)
          Operators.eq(l, r)

        | _ -> 
          if r.IsNumber then
            let l = TC.ToPrimitive(l.Object, DefaultValueHint.None)
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
    let fallback() =
      let l' = l |> TC.ToPrimitive
      let r' = r |> TC.ToPrimitive
      if l'.IsString || r'.IsString 
        then (TC.ToString l' + TC.ToString r') |> BV.Box
        else (TC.ToNumber l' + TC.ToNumber r') |> BV.Box

    if l.IsNumber then
      if r.IsNumber then
        (l.Number + r.Number) |> BV.Box
      elif r.IsString then
        (TC.ToString l + r.String) |> BV.Box
      else
        fallback()
    elif l.IsString then
      if r.IsNumber then
        (l.String + TC.ToString r) |> BV.Box
      elif r.IsString then
        (l.String + r.String) |> BV.Box
      else
        fallback()
    else
      fallback()

  //----------------------------------------------------------------------------
  // -
  static member sub (l, r) = Dlr.callStaticT<Operators> "sub" [l; r]
  static member sub (l:BoxedValue, r:BoxedValue) =
    if l.IsNumber && r.IsNumber
      then BV.Box (l.Number - r.Number)
      else BV.Box (TC.ToNumber l - TC.ToNumber r)
      
  //----------------------------------------------------------------------------
  // /
  static member div (l, r) = Dlr.callStaticT<Operators> "div" [l; r]
  static member div (l:BoxedValue, r:BoxedValue) =
    if l.IsNumber && r.IsNumber
      then BV.Box (l.Number / r.Number)
      else BV.Box (TC.ToNumber l / TC.ToNumber r)
      
  //----------------------------------------------------------------------------
  // *
  static member mul (l, r) = Dlr.callStaticT<Operators> "mul" [l; r]
  static member mul (l:BoxedValue, r:BoxedValue) =
    if l.IsNumber && r.IsNumber
      then BV.Box (l.Number * r.Number)
      else BV.Box (TC.ToNumber l * TC.ToNumber r)
      
  //----------------------------------------------------------------------------
  // %
  static member mod' (l, r) = Dlr.callStaticT<Operators> "mod'" [l; r]
  static member mod' (l:BoxedValue, r:BoxedValue) =
    if l.IsNumber && r.IsNumber
      then BV.Box (l.Number % r.Number)
      else BV.Box (TC.ToNumber l % TC.ToNumber r)
    
  //----------------------------------------------------------------------------
  // &
  static member bitAnd (l, r) = Dlr.callStaticT<Operators> "bitAnd" [l; r]
  static member bitAnd (l:BoxedValue, r:BoxedValue) =
    let l = TC.ToNumber l
    let r = TC.ToNumber r
    let l = TC.ToInt32 l
    let r = TC.ToInt32 r
    (l &&& r) |> double
    
  //----------------------------------------------------------------------------
  // |
  static member bitOr (l, r) = Dlr.callStaticT<Operators> "bitOr" [l; r]
  static member bitOr (l:BoxedValue, r:BoxedValue) =
    let l = TC.ToNumber l
    let r = TC.ToNumber r
    let l = TC.ToInt32 l
    let r = TC.ToInt32 r
    (l ||| r) |> double
    
  //----------------------------------------------------------------------------
  // ^
  static member bitXOr (l, r) = Dlr.callStaticT<Operators> "bitXOr" [l; r]
  static member bitXOr (l:BoxedValue, r:BoxedValue) =
    let l = TC.ToNumber l
    let r = TC.ToNumber r
    let l = TC.ToInt32 l
    let r = TC.ToInt32 r
    (l ^^^ r) |> double
    
  //----------------------------------------------------------------------------
  // <<
  static member bitLhs (l, r) = Dlr.callStaticT<Operators> "bitLhs" [l; r]
  static member bitLhs (l:BoxedValue, r:BoxedValue) =
    let l = TC.ToNumber l
    let r = TC.ToNumber r
    let l = TC.ToInt32 l
    let r = TC.ToUInt32 r &&& 0x1Fu
    (l <<< int r) |> double
    
  //----------------------------------------------------------------------------
  // >>
  static member bitRhs (l, r) = Dlr.callStaticT<Operators> "bitRhs" [l; r]
  static member bitRhs (l:BoxedValue, r:BoxedValue) =
    let l = TC.ToNumber l
    let r = TC.ToNumber r
    let l = TC.ToInt32 l
    let r = TC.ToUInt32 r &&& 0x1Fu
    (l >>> int r) |> double
    
  //----------------------------------------------------------------------------
  // >>>
  static member bitURhs (l, r) = Dlr.callStaticT<Operators> "bitURhs" [l; r]
  static member bitURhs (l:BoxedValue, r:BoxedValue) =
    let l = TC.ToNumber l
    let r = TC.ToNumber r
    let l = TC.ToUInt32 l
    let r = TC.ToUInt32 r &&& 0x1Fu
    (l >>> int r) |> double
    
  //----------------------------------------------------------------------------
  // &&
  static member and' (l, r) = Dlr.callStaticT<Operators> "and'" [l; r]
  static member and' (l:BoxedValue, r:BoxedValue) =
    if not (TC.ToBoolean l) then l else r

  // I seriously hate parts of the ECMA spec.
  static member and' (l:BoxedValue, r:string, g:CO) =
    if TC.ToBoolean l 
      then GlobalScopeHelper.GetGlobal(g, r)
      else l
    
  //----------------------------------------------------------------------------
  // ||
  static member or' (l, r) = Dlr.callStaticT<Operators> "or'" [l; r]
  static member or' (l:BoxedValue, r:BoxedValue) =
    if TC.ToBoolean l then l else r

  static member or' (l:BoxedValue, r:string, g:CO) =
    if TC.ToBoolean l
      then l 
      else GlobalScopeHelper.GetGlobal(g, r)