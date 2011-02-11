namespace IronJS.Api

open System
open IronJS
open IronJS.Support.Aliases
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