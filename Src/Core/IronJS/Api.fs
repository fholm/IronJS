namespace IronJS.Api

open System
open IronJS
open IronJS.Aliases
open IronJS.Utils.Patterns

module Extensions = 

  type Object with 

    member o.put (name, v:IjsBox) =
      o.Methods.PutBoxProperty.Invoke(o, name, v)

    member o.put (name, v:IjsBool) =
      let v = if v then TaggedBools.True else TaggedBools.False
      o.Methods.PutValProperty.Invoke(o, name, v)

    member o.put (name, v:IjsNum) =
      o.Methods.PutValProperty.Invoke(o, name, v)

    member o.put (name, v:ClrObject) =
      o.Methods.PutRefProperty.Invoke(o, name, v, TypeTags.Clr)

    member o.put (name, v:IjsStr) =
      o.Methods.PutRefProperty.Invoke(o, name, v, TypeTags.String)

    member o.put (name, v:Undefined) =
      o.Methods.PutRefProperty.Invoke(o, name, v, TypeTags.Undefined)

    member o.put (name, v:IjsObj) =
      o.Methods.PutRefProperty.Invoke(o, name, v, TypeTags.Object)

    member o.put (name, v:IjsFunc) =
      o.Methods.PutRefProperty.Invoke(o, name, v, TypeTags.Function)

    member o.put (name, v:IjsRef, tc:TypeTag) =
      o.Methods.PutRefProperty.Invoke(o, name, v, tc)

    member o.get (name) =
      o.Methods.GetProperty.Invoke(o, name)

    member o.has (name) =
      o.Methods.HasProperty.Invoke(o, name)

    member o.delete (name) =
      o.Methods.DeleteProperty.Invoke(o, name)
      
    member o.put (index, v:IjsBox) =
      o.Methods.PutBoxIndex.Invoke(o, index, v)

    member o.put (index, v:IjsBool) =
      let v = if v then TaggedBools.True else TaggedBools.False
      o.Methods.PutValIndex.Invoke(o, index, v)

    member o.put (index, v:IjsNum) =
      o.Methods.PutValIndex.Invoke(o, index, v)

    member o.put (index, v:ClrObject) =
      o.Methods.PutRefIndex.Invoke(o, index, v, TypeTags.Clr)

    member o.put (index, v:IjsStr) =
      o.Methods.PutRefIndex.Invoke(o, index, v, TypeTags.String)

    member o.put (index, v:Undefined) =
      o.Methods.PutRefIndex.Invoke(o, index, v, TypeTags.Undefined)

    member o.put (index, v:IjsObj) =
      o.Methods.PutRefIndex.Invoke(o, index, v, TypeTags.Object)

    member o.put (index, v:IjsFunc) =
      o.Methods.PutRefIndex.Invoke(o, index, v, TypeTags.Function)

    member o.put (index, v:IjsRef, tc:TypeTag) =
      o.Methods.PutRefIndex.Invoke(o, index, v, tc)

    member o.get (index) =
      o.Methods.GetIndex.Invoke(o, index)

    member o.has (index) =
      o.Methods.HasIndex.Invoke(o, index)

    member o.delete (index) =
      o.Methods.DeleteIndex.Invoke(o, index)

module Environment =

  open Extensions

  //----------------------------------------------------------------------------
  let hasCompiler (env:IjsEnv) funcId =
    env.Compilers.ContainsKey funcId

  //----------------------------------------------------------------------------
  let addCompilerId (env:IjsEnv) funId compiler =
    if hasCompiler env funId |> not then
      env.Compilers.Add(funId, FunctionCompiler compiler)

  //----------------------------------------------------------------------------
  let addCompiler (env:IjsEnv) (f:IjsFunc) compiler =
    if hasCompiler env f.FunctionId |> not then
      f.Compiler <- FunctionCompiler compiler
      env.Compilers.Add(f.FunctionId, f.Compiler)
    
  //----------------------------------------------------------------------------
  let createObject (env:IjsEnv) =
    let o = IjsObj(env.Base_Class, env.Object_prototype, Classes.Object, 0u)
    o.Methods <- env.Object_methods
    o

  //----------------------------------------------------------------------------
  let createObjectWithMap (env:IjsEnv) map =
    let o = IjsObj(map, env.Object_prototype, Classes.Object, 0u)
    o.Methods <- env.Object_methods
    o

  //----------------------------------------------------------------------------
  let createArray (env:IjsEnv) size =
    let o = IjsObj(env.Array_Class, env.Array_prototype, Classes.Array, size)
    o.Methods <- env.Object_methods
    o.Methods.PutValProperty.Invoke(o, "length", double size)
    o
    
  //----------------------------------------------------------------------------
  let createString (env:IjsEnv) (s:IjsStr) =
    let o = IjsObj(env.String_Class, env.String_prototype, Classes.String, 0u)
    o.Methods <- env.Object_methods
    o.Methods.PutValProperty.Invoke(o, "length", double s.Length)
    o.Value.Box.Clr <-s
    o.Value.Box.Tag <- TypeTags.String
    o
    
  //----------------------------------------------------------------------------
  let createNumber (env:IjsEnv) n =
    let o = IjsObj(env.Number_Class, env.Number_prototype, Classes.Number, 0u)
    o.Methods <- env.Object_methods
    o.Value.Box.Number <- n
    o
    
  //----------------------------------------------------------------------------
  let createBoolean (env:IjsEnv) b =
    let o = IjsObj(env.Boolean_Class, env.Boolean_prototype, Classes.Boolean, 0u)
    o.Methods <- env.Object_methods
    o.Value.Box.Bool <- b
    o.Value.Box.Tag <- TypeTags.Bool
    o
  
  //----------------------------------------------------------------------------
  let createPrototype (env:IjsEnv) =
    let o = IjsObj(env.Prototype_Class, env.Object_prototype, Classes.Object, 0u)
    o.Methods <- env.Object_methods
    o
  
  //----------------------------------------------------------------------------
  let createFunction env id (args:int) chain dc =
    let proto = createPrototype env
    let func = IjsFunc(env, id, chain, dc)

    (func :> IjsObj).Methods <- env.Object_methods
    func.ConstructorMode <- ConstructorModes.User

    proto.put("constructor", func)
    func.put("prototype", proto)
    func.put("length", double args)

    func
    
  //----------------------------------------------------------------------------
  module Reflected =

    let createObject = 
      Utils.Reflected.methodInfo "Api.Environment" "createObject"

    let createObjectWithMap = 
      Utils.Reflected.methodInfo "Api.Environment" "createObjectWithMap"

    let createArray = 
      Utils.Reflected.methodInfo "Api.Environment" "createArray"

    let createString = 
      Utils.Reflected.methodInfo "Api.Environment" "createString"

    let createNumber = 
      Utils.Reflected.methodInfo "Api.Environment" "createNumber"

    let createBoolean = 
      Utils.Reflected.methodInfo "Api.Environment" "createBoolean"

    let createPrototype = 
      Utils.Reflected.methodInfo "Api.Environment" "createPrototype"

    let createFunction = 
      Utils.Reflected.methodInfo "Api.Environment" "createFunction"

//------------------------------------------------------------------------------
// Static class containing all type conversions
//------------------------------------------------------------------------------
type TypeConverter =

  //----------------------------------------------------------------------------
  static member toBox(b:IjsBox) = b
  static member toBox(d:IjsNum) = Utils.boxNumber d
  static member toBox(b:IjsBool) = Utils.boxBool b
  static member toBox(s:IjsStr) = Utils.boxString s
  static member toBox(o:IjsObj) = Utils.boxObject o
  static member toBox(f:IjsFunc) = Utils.boxFunction f
  static member toBox(c:ClrObject) = Utils.boxClr c
  static member toBox(expr:Dlr.Expr) = 
    Dlr.callStaticT<TypeConverter> "toBox" [expr]
    
  //----------------------------------------------------------------------------
  static member toClrObject(d:IjsNum) = box d
  static member toClrObject(b:IjsBool) = box b
  static member toClrObject(s:IjsStr) = box s
  static member toClrObject(o:IjsObj) = box o
  static member toClrObject(f:IjsFunc) = box f
  static member toClrObject(c:ClrObject) = c
  static member toClrObject(b:IjsBox) =
    match b with
    | Number -> b.Number :> ClrObject
    | Undefined -> null
    | String -> b.String :> ClrObject
    | Boolean -> b.Bool :> ClrObject
    | Clr -> b.Clr
    | Object -> b.Object :> ClrObject
    | Function -> b.Func :> ClrObject

  static member toClrObject (expr:Dlr.Expr) =
    Dlr.callStaticT<TypeConverter> "toClrObject" [expr]

  //----------------------------------------------------------------------------
  static member toString (b:IjsBool) = if b then "true" else "false"
  static member toString (s:IjsStr) = s
  static member toString (u:Undefined) = "undefined"
  static member toString (b:IjsBox) =
    match b with
    | Undefined -> "undefined"
    | String -> b.String
    | Number -> TypeConverter.toString b.Number
    | Boolean -> TypeConverter.toString b.Bool
    | Clr -> TypeConverter.toString b.Clr
    | Object -> TypeConverter.toString b.Object
    | Function -> TypeConverter.toString (b.Func :> IjsObj)

  static member toString (o:IjsObj) = 
    let mutable v = o.Methods.Default.Invoke(o, DefaultValue.String)
    TypeConverter.toString(v)

  static member toString (d:IjsNum) = 
    if System.Double.IsInfinity d then "Infinity" else d.ToString()

  static member toString (c:ClrObject) = 
    if c = null then "null" else c.ToString()

  static member toString (expr:Dlr.Expr) =
    Dlr.callStaticT<TypeConverter> "toString" [expr]
      
  //----------------------------------------------------------------------------
  static member toPrimitive (b:IjsBool, _:byte) = Utils.boxBool b
  static member toPrimitive (d:IjsNum, _:byte) = Utils.boxNumber d
  static member toPrimitive (s:IjsStr, _:byte) = Utils.boxString s
  static member toPrimitive (u:Undefined, _:byte) = Utils.BoxedConstants.undefined
  static member toPrimitive (o:IjsObj, h:byte) = o.Methods.Default.Invoke(o, h)
  static member toPrimitive (o:IjsObj) = o.Methods.Default.Invoke(o, 0uy)
  static member toPrimitive (b:IjsBox, h:byte) =
    match b with
    | Number
    | Boolean
    | String
    | Undefined -> b
    | Clr -> TypeConverter.toPrimitive(b.Clr, h)
    | Object
    | Function -> b.Object.Methods.Default.Invoke(b.Object, h)
  
  static member toPrimitive (c:ClrObject, _:byte) = 
    Utils.boxClr (if c = null then null else c.ToString())

  static member toPrimitive (expr:Dlr.Expr) =
    Dlr.callStaticT<TypeConverter> "toPrimitive" [expr]
      
  //----------------------------------------------------------------------------
  static member toBoolean (b:IjsBool) = b
  static member toBoolean (d:IjsNum) = d > 0.0 || d < 0.0
  static member toBoolean (c:ClrObject) = if c = null then false else true
  static member toBoolean (s:IjsStr) = s.Length > 0
  static member toBoolean (u:Undefined) = false
  static member toBoolean (o:IjsObj) = true
  static member toBoolean (b:IjsBox) =
    match b with 
    | Number -> TypeConverter.toBoolean b
    | Boolean -> b.Bool
    | Undefined -> false
    | String -> b.String.Length > 0
    | Clr -> TypeConverter.toBoolean b.Clr
    | Object 
    | Function -> true
    
  static member toBoolean (expr:Dlr.Expr) =
    Dlr.callStaticT<TypeConverter> "toBoolean" [expr]

  //----------------------------------------------------------------------------
  static member toNumber (b:IjsBool) : double = if b then 1.0 else 0.0
  static member toNumber (d:IjsNum) = d
  static member toNumber (c:ClrObject) = if c = null then 0.0 else 1.0
  static member toNumber (u:Undefined) = IjsNum.NaN
  static member toNumber (b:IjsBox) =
    match b with
    | Number -> b.Number
    | Boolean -> if b.Bool then 1.0 else 0.0
    | String -> TypeConverter.toNumber(b.String)
    | Undefined -> NaN
    | Clr -> TypeConverter.toNumber b.Clr
    | Object 
    | Function -> TypeConverter.toNumber(b.Object)

  static member toNumber (o:IjsObj) : IjsNum = 
    TypeConverter.toNumber(o.Methods.Default.Invoke(o, DefaultValue.Number))

  static member toNumber (s:IjsStr) = 
    let mutable d = 0.0
    if Double.TryParse(s, anyNumber, invariantCulture, &d) 
      then d
      else NaN

  static member toNumber (expr:Dlr.Expr) = 
    Dlr.callStaticT<TypeConverter> "toNumber" [expr]
        
  //----------------------------------------------------------------------------
  static member toObject (env:IjsEnv, o:IjsObj) = o
  static member toObject (env:IjsEnv, b:Box) =
    match b with
    | Function
    | Object -> b.Object
    | Undefined
    | Clr -> Errors.Generic.notImplemented()
    | Number -> Environment.createNumber env b.Number
    | String -> Environment.createString env b.String
    | Boolean -> Environment.createBoolean env b.Bool

  static member toObject (env:Dlr.Expr, expr:Dlr.Expr) =
    Dlr.callStaticT<TypeConverter> "toObject" [env; expr]
      
  //----------------------------------------------------------------------------
  static member toInt32 (d:IjsNum) = int d
  static member toUInt32 (d:IjsNum) = uint32 d
  static member toUInt16 (d:IjsNum) = uint16 d
  static member toInteger (d:IjsNum) : double = 
    if d = NaN
      then 0.0
      elif d = 0.0 || d = NegInf || d = PosInf
        then d
        else double (Math.Sign d) * Math.Floor(Math.Abs d)
                
  //-------------------------------------------------------------------------
  static member convertTo (env:Dlr.Expr) (expr:Dlr.Expr) (t:System.Type) =
    if Object.ReferenceEquals(expr.Type, t) then expr
    elif t.IsAssignableFrom(expr.Type) then Dlr.cast t expr
    else 
      if   t = typeof<IjsNum> then TypeConverter.toNumber expr
      elif t = typeof<IjsStr> then TypeConverter.toString expr
      elif t = typeof<IjsBool> then TypeConverter.toBoolean expr
      elif t = typeof<IjsBox> then TypeConverter.toBox expr
      elif t = typeof<IjsObj> then TypeConverter.toObject(env, expr)
      elif t = typeof<ClrObject> then TypeConverter.toClrObject expr
      else Errors.Generic.noConversion expr.Type t

  static member convertToT<'a> env expr = 
    TypeConverter.convertTo env expr typeof<'a>
    
//------------------------------------------------------------------------------
// Operators
//------------------------------------------------------------------------------
type Operators =

  //----------------------------------------------------------------------------
  // Unary
  //----------------------------------------------------------------------------

  //----------------------------------------------------------------------------
  // typeof
  static member typeOf (o:IjsBox) = TypeTags.Names.[o.Tag]
  static member typeOf expr = Dlr.callStaticT<Operators> "typeOf" [expr]
  
  //----------------------------------------------------------------------------
  // !
  static member not (o) = Dlr.callStaticT<Operators> "not" [o]
  static member not (o:IjsBox) =
    not (TypeConverter.toBoolean o)
    
  //----------------------------------------------------------------------------
  // ~
  static member bitCmpl (o) = Dlr.callStaticT<Operators> "bitCmpl" [o]
  static member bitCmpl (o:IjsBox) =
    let o = TypeConverter.toNumber o
    let o = TypeConverter.toInt32 o
    Utils.boxNumber (double (~~~ o))
      
  //----------------------------------------------------------------------------
  // + (unary)
  static member plus (l, r) = Dlr.callStaticT<Operators> "plus" [l; r]
  static member plus (o:IjsBox) =
    Utils.boxNumber (TypeConverter.toNumber o)
    
  //----------------------------------------------------------------------------
  // - (unary)
  static member minus (l, r) = Dlr.callStaticT<Operators> "minus" [l; r]
  static member minus (o:IjsBox) =
    Utils.boxNumber ((TypeConverter.toNumber o) * -1.0)

  //----------------------------------------------------------------------------
  // Binary
  //----------------------------------------------------------------------------
    
  //----------------------------------------------------------------------------
  // <
  static member lt (l, r) = Dlr.callStaticT<Operators> "lt" [l; r]
  static member lt (l:IjsBox, r:IjsBox) =
    if Utils.Box.isBothNumber l.Marker r.Marker
      then l.Number < r.Number
      elif l.Tag = TypeTags.String && r.Tag = TypeTags.String
        then l.String < r.String
        else TypeConverter.toNumber l < TypeConverter.toNumber r
        
  //----------------------------------------------------------------------------
  // <=
  static member ltEq (l, r) = Dlr.callStaticT<Operators> "ltEq" [l; r]
  static member ltEq (l:IjsBox, r:IjsBox) =
    if Utils.Box.isBothNumber l.Marker r.Marker
      then l.Number <= r.Number
      elif l.Tag = TypeTags.String && r.Tag = TypeTags.String
        then l.String <= r.String
        else TypeConverter.toNumber l <= TypeConverter.toNumber r
        
  //----------------------------------------------------------------------------
  // >
  static member gt (l, r) = Dlr.callStaticT<Operators> "gt" [l; r]
  static member gt (l:IjsBox, r:IjsBox) =
    if Utils.Box.isBothNumber l.Marker r.Marker
      then l.Number > r.Number
      elif l.Tag = TypeTags.String && r.Tag = TypeTags.String
        then l.String > r.String
        else TypeConverter.toNumber l > TypeConverter.toNumber r
        
  //----------------------------------------------------------------------------
  // >=
  static member gtEq (l, r) = Dlr.callStaticT<Operators> "gtEq" [l; r]
  static member gtEq (l:IjsBox, r:IjsBox) =
    if Utils.Box.isBothNumber l.Marker r.Marker
      then l.Number >= r.Number
      elif l.Tag = TypeTags.String && r.Tag = TypeTags.String
        then l.String >= r.String
        else TypeConverter.toNumber l >= TypeConverter.toNumber r
        
  //----------------------------------------------------------------------------
  // ==
  static member eq (l, r) = Dlr.callStaticT<Operators> "eq" [l; r]
  static member eq (l:IjsBox, r:IjsBox) = 
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
        l.Number = TypeConverter.toNumber r.String
        
      elif r.Tag = TypeTags.String && Utils.Box.isNumber r.Marker then
        TypeConverter.toNumber l.String = r.Number

      elif l.Tag = TypeTags.Bool then
        let mutable l = Utils.boxNumber(TypeConverter.toNumber l)
        Operators.eq(l, r)

      elif r.Tag = TypeTags.Bool then
        let mutable r = Utils.boxNumber(TypeConverter.toNumber r)
        Operators.eq(l, r)

      elif r.Tag >= TypeTags.Object then
        match l.Tag with
        | TypeTags.String -> 
          let mutable r = TypeConverter.toPrimitive(r.Object)
          Operators.eq(l, r)

        | _ -> 
          if Utils.Box.isNumber l.Marker then
            let mutable r = TypeConverter.toPrimitive(r.Object)
            Operators.eq(l, r)
          else
            false

      elif l.Tag >= TypeTags.Object then
        match r.Tag with
        | TypeTags.String -> 
          let mutable l = TypeConverter.toPrimitive(l.Object)
          Operators.eq(l, r)

        | _ -> 
          if Utils.Box.isNumber r.Marker then
            let mutable l = TypeConverter.toPrimitive(l.Object)
            Operators.eq(l, r)
          else
            false

      else
        false
        
  //----------------------------------------------------------------------------
  // !=
  static member notEq (l, r) = Dlr.callStaticT<Operators> "notEq" [l; r]
  static member notEq (l:IjsBox, r:IjsBox) = not (Operators.eq(l, r))
  
  //----------------------------------------------------------------------------
  // ===
  static member same (l, r) = Dlr.callStaticT<Operators> "same" [l; r]
  static member same (l:IjsBox, r:IjsBox) = 
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
  static member notSame (l:IjsBox, r:IjsBox) =
    not (Operators.same(l, r))
    
  //----------------------------------------------------------------------------
  // +
  static member add (l, r) = Dlr.callStaticT<Operators> "add" [l; r]
  static member add (l:IjsBox, r:IjsBox) = 
    if Utils.Box.isBothNumber l.Marker r.Marker then
      Utils.boxNumber (l.Number + r.Number)

    elif l.Tag = TypeTags.String || r.Tag = TypeTags.String then
      Utils.boxString (TypeConverter.toString(l) + TypeConverter.toString(r))

    else
      Utils.boxNumber (TypeConverter.toNumber(l) + TypeConverter.toNumber(r))
      
  //----------------------------------------------------------------------------
  // -
  static member sub (l, r) = Dlr.callStaticT<Operators> "sub" [l; r]
  static member sub (l:IjsBox, r:IjsBox) =
    if Utils.Box.isBothNumber l.Marker r.Marker then
      Utils.boxNumber (l.Number - r.Number)

    else
      Utils.boxNumber (TypeConverter.toNumber(l) - TypeConverter.toNumber(r))
      
  //----------------------------------------------------------------------------
  // /
  static member div (l, r) = Dlr.callStaticT<Operators> "div" [l; r]
  static member div (l:IjsBox, r:IjsBox) =
    if Utils.Box.isBothNumber l.Marker r.Marker then
      Utils.boxNumber (l.Number / r.Number)

    else
      Utils.boxNumber (TypeConverter.toNumber(l) / TypeConverter.toNumber(r))
      
  //----------------------------------------------------------------------------
  // *
  static member mul (l, r) = Dlr.callStaticT<Operators> "mul" [l; r]
  static member mul (l:IjsBox, r:IjsBox) =
    if Utils.Box.isBothNumber l.Marker r.Marker then
      Utils.boxNumber (l.Number * r.Number)

    else
      Utils.boxNumber (TypeConverter.toNumber(l) * TypeConverter.toNumber(r))
      
  //----------------------------------------------------------------------------
  // %
  static member mod' (l, r) = Dlr.callStaticT<Operators> "mod'" [l; r]
  static member mod' (l:IjsBox, r:IjsBox) =
    if Utils.Box.isBothNumber l.Marker r.Marker then
      Utils.boxNumber (l.Number % r.Number)

    else
      Utils.boxNumber (TypeConverter.toNumber l % TypeConverter.toNumber r)
    
  //----------------------------------------------------------------------------
  // &
  static member bitAnd (l, r) = Dlr.callStaticT<Operators> "bitAnd" [l; r]
  static member bitAnd (l:IjsBox, r:IjsBox) =
    let l = TypeConverter.toNumber l
    let r = TypeConverter.toNumber r
    let l = TypeConverter.toInt32 l
    let r = TypeConverter.toInt32 r
    Utils.boxNumber (double (l &&& r))
    
  //----------------------------------------------------------------------------
  // |
  static member bitOr (l, r) = Dlr.callStaticT<Operators> "bitOr" [l; r]
  static member bitOr (l:IjsBox, r:IjsBox) =
    let l = TypeConverter.toNumber l
    let r = TypeConverter.toNumber r
    let l = TypeConverter.toInt32 l
    let r = TypeConverter.toInt32 r
    Utils.boxNumber (double (l ||| r))
    
  //----------------------------------------------------------------------------
  // ^
  static member bitXOr (l, r) = Dlr.callStaticT<Operators> "bitXOr" [l; r]
  static member bitXOr (l:IjsBox, r:IjsBox) =
    let l = TypeConverter.toNumber l
    let r = TypeConverter.toNumber r
    let l = TypeConverter.toInt32 l
    let r = TypeConverter.toInt32 r
    Utils.boxNumber (double (l ^^^ r))
    
  //----------------------------------------------------------------------------
  // <<
  static member bitLhs (l, r) = Dlr.callStaticT<Operators> "bitLhs" [l; r]
  static member bitLhs (l:IjsBox, r:IjsBox) =
    let l = TypeConverter.toNumber l
    let r = TypeConverter.toNumber r
    let l = TypeConverter.toInt32 l
    let r = TypeConverter.toUInt32 r &&& 0x1Fu
    Utils.boxNumber (double (l <<< int r))
    
  //----------------------------------------------------------------------------
  // >>
  static member bitRhs (l, r) = Dlr.callStaticT<Operators> "bitRhs" [l; r]
  static member bitRhs (l:IjsBox, r:IjsBox) =
    let l = TypeConverter.toNumber l
    let r = TypeConverter.toNumber r
    let l = TypeConverter.toInt32 l
    let r = TypeConverter.toUInt32 r &&& 0x1Fu
    Utils.boxNumber (double (l >>> int r))
    
  //----------------------------------------------------------------------------
  // >>>
  static member bitURhs (l, r) = Dlr.callStaticT<Operators> "bitURhs" [l; r]
  static member bitURhs (l:IjsBox, r:IjsBox) =
    let l = TypeConverter.toNumber l
    let r = TypeConverter.toNumber r
    let l = TypeConverter.toUInt32 l
    let r = TypeConverter.toUInt32 r &&& 0x1Fu
    Utils.boxNumber (double (l >>> int r))
    
  //----------------------------------------------------------------------------
  // &&
  static member and' (l, r) = Dlr.callStaticT<Operators> "and'" [l; r]
  static member and' (l:IjsBox, r:IjsBox) =
    if not (TypeConverter.toBoolean l) then l else r
    
  //----------------------------------------------------------------------------
  // ||
  static member or' (l, r) = Dlr.callStaticT<Operators> "or'" [l; r]
  static member or' (l:IjsBox, r:IjsBox) =
    if TypeConverter.toBoolean l then l else r
      
//------------------------------------------------------------------------------
// PropertyMap
//------------------------------------------------------------------------------
module PropertyMap =

  //----------------------------------------------------------------------------
  let getSubMap (map:PropertyMap) name = 
    if map.isDynamic then 
      let index = 
        if map.FreeIndexes.Count > 0 then map.FreeIndexes.Pop()
        else map.NextIndex <- map.NextIndex + 1; map.NextIndex - 1

      map.PropertyMap.Add(name, index)
      map

    else
      let mutable subMap = null
      
      if not(map.SubClasses.TryGetValue(name, &subMap)) then
        let properties = new MutableDict<string, int>(map.PropertyMap)
        properties.Add(name, properties.Count)
        subMap <- IronJS.PropertyMap(map.Env, properties)
        map.SubClasses.Add(name, subMap)

      subMap

  //----------------------------------------------------------------------------
  let rec buildSubMap (map:PropertyMap) names =
    Seq.fold (fun map name -> getSubMap map name) map names
        
  //----------------------------------------------------------------------------
  let makeDynamic (map:IronJS.PropertyMap) =
    if map.isDynamic then map
    else
      let newMap = PropertyMap(null)
      newMap.Id <- -1L
      newMap.NextIndex <- map.NextIndex
      newMap.FreeIndexes <- new MutableStack<int>()
      newMap.PropertyMap <- new MutableDict<string, int>(map.PropertyMap)
      newMap
        
  //----------------------------------------------------------------------------
  let delete (x:IronJS.PropertyMap, name) =
    let pc = if not x.isDynamic then makeDynamic x else x
    let mutable index = 0

    if pc.PropertyMap.TryGetValue(name, &index) then 
      pc.FreeIndexes.Push index
      pc.PropertyMap.Remove name |> ignore

    pc
      
  //----------------------------------------------------------------------------
  let getIndex (map:PropertyMap) name =
    map.PropertyMap.[name]
    
//------------------------------------------------------------------------------
// Function API
//------------------------------------------------------------------------------
type Function() =

  static let getPrototype(f:IjsFunc) =
    let prototype = (f :> IjsObj).Methods.GetProperty.Invoke(f, "prototype")
    match prototype.Tag with
    | TypeTags.Function
    | TypeTags.Object -> prototype.Object
    | _ -> f.Env.Object_prototype

  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  // GENERATED FUNCTION METHODS
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  
  //----------------------------------------------------------------------------
  static member call (f:IjsFunc,t) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,IjsBox>>(f)
    c.Invoke(f,t)
  
  //----------------------------------------------------------------------------
  static member call (f:IjsFunc,t,a0:'a0) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,IjsBox>>(f)
    c.Invoke(f,t,a0)
  
  //----------------------------------------------------------------------------
  static member call (f:IjsFunc,t,a0:'a0,a1:'a1) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,IjsBox>>(f)
    c.Invoke(f,t,a0,a1)
  
  #if CLR2
  #else
  //----------------------------------------------------------------------------
  static member call (f:IjsFunc,t,a0:'a0,a1:'a1,a2:'a2) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,IjsBox>>(f)
    c.Invoke(f,t,a0,a1,a2)
  
  //----------------------------------------------------------------------------
  static member call (f:IjsFunc,t,a0:'a0,a1:'a1,a2:'a2,a3:'a3) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,'a3,IjsBox>>(f)
    c.Invoke(f,t,a0,a1,a2,a3)
  
  //----------------------------------------------------------------------------
  static member call (f:IjsFunc,t,a0:'a0,a1:'a1,a2:'a2,a3:'a3,a4:'a4) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,'a3,'a4,IjsBox>>(f)
    c.Invoke(f,t,a0,a1,a2,a3,a4)
  
  //----------------------------------------------------------------------------
  static member call (f:IjsFunc,t,a0:'a0,a1:'a1,a2:'a2,a3:'a3,a4:'a4,a5:'a5) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,'a3,'a4,'a5,IjsBox>>(f)
    c.Invoke(f,t,a0,a1,a2,a3,a4,a5)
  
  //----------------------------------------------------------------------------
  static member call (f:IjsFunc,t,a0:'a0,a1:'a1,a2:'a2,a3:'a3,a4:'a4,a5:'a5,a6:'a6) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,'a3,'a4,'a5,'a6,IjsBox>>(f)
    c.Invoke(f,t,a0,a1,a2,a3,a4,a5,a6)
  
  //----------------------------------------------------------------------------
  static member call (f:IjsFunc,t,a0:'a0,a1:'a1,a2:'a2,a3:'a3,a4:'a4,a5:'a5,a6:'a6,a7:'a7) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,'a3,'a4,'a5,'a6,'a7,IjsBox>>(f)
    c.Invoke(f,t,a0,a1,a2,a3,a4,a5,a6,a7)
  #endif

  //----------------------------------------------------------------------------
  static member construct (f:IjsFunc,t:IjsObj) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,IjsBox>>(f)

    match f.ConstructorMode with
    | ConstructorModes.Host -> c.Invoke(f,null)
    | ConstructorModes.User -> 
      let o = Environment.createObject f.Env
      o.Prototype <- getPrototype f
      c.Invoke(f,o) |> ignore
      Utils.boxObject o

    | _ -> Errors.runtime "Can't call [[Construct]] on non-constructor"

  //----------------------------------------------------------------------------
  static member construct (f:IjsFunc,t:IjsObj,a0:'a0) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,IjsBox>>(f)

    match f.ConstructorMode with
    | ConstructorModes.Host -> c.Invoke(f,null,a0)
    | ConstructorModes.User -> 
      let o = Environment.createObject f.Env
      o.Prototype <- getPrototype f
      c.Invoke(f,o,a0) |> ignore
      Utils.boxObject o

    | _ -> Errors.runtime "Can't call [[Construct]] on non-constructor"

  //----------------------------------------------------------------------------
  static member construct (f:IjsFunc,t:IjsObj,a0:'a0,a1:'a1) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,IjsBox>>(f)

    match f.ConstructorMode with
    | ConstructorModes.Host -> c.Invoke(f,null,a0,a1)
    | ConstructorModes.User -> 
      let o = Environment.createObject f.Env
      o.Prototype <- getPrototype f
      c.Invoke(f,o,a0,a1) |> ignore
      Utils.boxObject o

    | _ -> Errors.runtime "Can't call [[Construct]] on non-constructor"
    
  #if CLR2
  #else
  //----------------------------------------------------------------------------
  static member construct (f:IjsFunc,t:IjsObj,a0:'a0,a1:'a1,a2:'a2) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,IjsBox>>(f)

    match f.ConstructorMode with
    | ConstructorModes.Host -> c.Invoke(f,null,a0,a1,a2)
    | ConstructorModes.User -> 
      let o = Environment.createObject f.Env
      o.Prototype <- getPrototype f
      c.Invoke(f,o,a0,a1,a2) |> ignore
      Utils.boxObject o

    | _ -> Errors.runtime "Can't call [[Construct]] on non-constructor"

  //----------------------------------------------------------------------------
  static member construct (f:IjsFunc,t:IjsObj,a0:'a0,a1:'a1,a2:'a2,a3:'a3) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,'a3,IjsBox>>(f)

    match f.ConstructorMode with
    | ConstructorModes.Host -> c.Invoke(f,null,a0,a1,a2,a3)
    | ConstructorModes.User -> 
      let o = Environment.createObject f.Env
      o.Prototype <- getPrototype f
      c.Invoke(f,o,a0,a1,a2,a3) |> ignore
      Utils.boxObject o

    | _ -> Errors.runtime "Can't call [[Construct]] on non-constructor"

  //----------------------------------------------------------------------------
  static member construct (f:IjsFunc,t:IjsObj,a0:'a0,a1:'a1,a2:'a2,a3:'a3,a4:'a4) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,'a3,'a4,IjsBox>>(f)

    match f.ConstructorMode with
    | ConstructorModes.Host -> c.Invoke(f,null,a0,a1,a2,a3,a4)
    | ConstructorModes.User -> 
      let o = Environment.createObject f.Env
      o.Prototype <- getPrototype f
      c.Invoke(f,o,a0,a1,a2,a3,a4) |> ignore
      Utils.boxObject o

    | _ -> Errors.runtime "Can't call [[Construct]] on non-constructor"

  //----------------------------------------------------------------------------
  static member construct (f:IjsFunc,t:IjsObj,a0:'a0,a1:'a1,a2:'a2,a3:'a3,a4:'a4,a5:'a5) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,'a3,'a4,'a5,IjsBox>>(f)

    match f.ConstructorMode with
    | ConstructorModes.Host -> c.Invoke(f,null,a0,a1,a2,a3,a4,a5)
    | ConstructorModes.User -> 
      let o = Environment.createObject f.Env
      o.Prototype <- getPrototype f
      c.Invoke(f,o,a0,a1,a2,a3,a4,a5) |> ignore
      Utils.boxObject o

    | _ -> Errors.runtime "Can't call [[Construct]] on non-constructor"

  //----------------------------------------------------------------------------
  static member construct (f:IjsFunc,t:IjsObj,a0:'a0,a1:'a1,a2:'a2,a3:'a3,a4:'a4,a5:'a5,a6:'a6) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,'a3,'a4,'a5,'a6,IjsBox>>(f)

    match f.ConstructorMode with
    | ConstructorModes.Host -> c.Invoke(f,null,a0,a1,a2,a3,a4,a5,a6)
    | ConstructorModes.User -> 
      let o = Environment.createObject f.Env
      o.Prototype <- getPrototype f
      c.Invoke(f,o,a0,a1,a2,a3,a4,a5,a6) |> ignore
      Utils.boxObject o

    | _ -> Errors.runtime "Can't call [[Construct]] on non-constructor"

  //----------------------------------------------------------------------------
  static member construct (f:IjsFunc,t:IjsObj,a0:'a0,a1:'a1,a2:'a2,a3:'a3,a4:'a4,a5:'a5,a6:'a6,a7:'a7) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,'a3,'a4,'a5,'a6,'a7,IjsBox>>(f)

    match f.ConstructorMode with
    | ConstructorModes.Host -> c.Invoke(f,null,a0,a1,a2,a3,a4,a5,a6,a7)
    | ConstructorModes.User -> 
      let o = Environment.createObject f.Env
      o.Prototype <- getPrototype f
      c.Invoke(f,o,a0,a1,a2,a3,a4,a5,a6,a7) |> ignore
      Utils.boxObject o

    | _ -> Errors.runtime "Can't call [[Construct]] on non-constructor"
  #endif


  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  // GENERATED FUNCTION METHODS
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------

//------------------------------------------------------------------------------
// HostFunction API
//------------------------------------------------------------------------------
module HostFunction =

  open Extensions

  [<ReferenceEquality>]
  type DispatchTarget<'a when 'a :> Delegate> = {
    Delegate : ClrType
    Function : IjsHostFunc<'a>
    Invoke: Dlr.Expr -> Dlr.Expr seq -> Dlr.Expr
  }

  //----------------------------------------------------------------------------
  let marshalArgs (args:Dlr.ExprParam array) (env:Dlr.Expr) i t =
    if i < args.Length 
      then TypeConverter.convertTo env args.[i] t
      else Dlr.default' t
      
  //----------------------------------------------------------------------------
  let marshalBoxParams (f:IjsHostFunc<_>) args m =
    args
    |> Seq.skip f.ArgTypes.Length
    |> Seq.map Expr.boxValue
    |> fun x -> Seq.append m [Dlr.newArrayItemsT<IjsBox> x]
    
  //----------------------------------------------------------------------------
  let marshalObjectParams (f:IjsHostFunc<_>) (args:Dlr.ExprParam array) m =
    args
    |> Seq.skip f.ArgTypes.Length
    |> Seq.map TypeConverter.toClrObject
    |> fun x -> Seq.append m [Dlr.newArrayItemsT<ClrObject> x]
    
  //----------------------------------------------------------------------------
  let createParam i t = Dlr.param (sprintf "a%i" i) t
  
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
      | ParamsModes.BoxParams when paramsExist -> 
        marshalBoxParams f passedArgs marshalled

      | ParamsModes.ObjectParams when paramsExist -> 
        marshalObjectParams f passedArgs marshalled

      | _ -> marshalled

    let invoke = target.Invoke func marshalled
    let body = 
      if Utils.isBox f.ReturnType then invoke
      elif Utils.isVoid f.ReturnType then Expr.voidAsUndefined invoke
      else
        Dlr.blockTmpT<Box> (fun tmp ->
          [
            (Expr.setBoxTypeOf tmp invoke)
            (Expr.setBoxValue tmp invoke)
            (tmp :> Dlr.Expr)
          ] |> Seq.ofList
        )
            
    let lambda = Dlr.lambda target.Delegate args body
    Debug.printExpr lambda
    lambda.Compile()

  //----------------------------------------------------------------------------
  let generateInvoke<'a when 'a :> Delegate> f args =
    let casted = Dlr.castT<IjsHostFunc<'a>> f
    Dlr.invoke (Dlr.field casted "Delegate") args
  
  //----------------------------------------------------------------------------
  let compile<'a when 'a :> Delegate> (x:IjsFunc) (delegate':System.Type) =
    compileDispatcher {
      Delegate = delegate'
      Function = x :?> IjsHostFunc<'a>
      Invoke = generateInvoke<'a>
    }
    
  //----------------------------------------------------------------------------
  let create (env:IjsEnv) (delegate':'a) =
    let h = IjsHostFunc<'a>(env, delegate')
    let f = h :> IjsFunc
    let o = f :> IjsObj

    o.Methods <- env.Object_methods
    o.Methods.PutValProperty.Invoke(f, "length", double h.jsArgsLength)
    Environment.addCompiler env f compile<'a>

    f

//------------------------------------------------------------------------------
module Object =

  //----------------------------------------------------------------------------
  let defaultvalue (o:IjsObj) (hint:byte) =
    let hint = 
      if hint = DefaultValue.None 
        then DefaultValue.Number 
        else hint

    let valueOf = o.Methods.GetProperty.Invoke(o, "valueOf")
    let toString = o.Methods.GetProperty.Invoke(o, "toString")

    match hint with
    | DefaultValue.Number ->
      match valueOf.Tag with
      | TypeTags.Function ->
        let mutable v = Function.call(valueOf.Func, o)
        if Utils.isPrimitive v then v
        else
          match toString.Tag with
          | TypeTags.Function ->
            let mutable v = Function.call(toString.Func, o)
            if Utils.isPrimitive v then v else Errors.runtime "[[TypeError]]"
          | _ -> Errors.runtime "[[TypeError]]"
      | _ -> Errors.runtime "[[TypeError]]"

    | DefaultValue.String ->
      match toString.Tag with
      | TypeTags.Function ->
        let mutable v = Function.call(toString.Func, o)
        if Utils.isPrimitive v then v
        else 
          match toString.Tag with
          | TypeTags.Function ->
            let mutable v = Function.call(valueOf.Func, o)
            if Utils.isPrimitive v then v else Errors.runtime "[[TypeError]]"
          | _ -> Errors.runtime "[[TypeError]]"
      | _ -> Errors.runtime "[[TypeError]]"

    | _ -> Errors.runtime "Invalid hint"

  let defaultValue' = Default defaultvalue
    
  //----------------------------------------------------------------------------
  module Property = 
  
    //--------------------------------------------------------------------------
    let requiredStorage (o:IjsObj) =
      o.PropertyMap.PropertyMap.Count
      
    //--------------------------------------------------------------------------
    let isFull (o:IjsObj) =
      requiredStorage o >= o.PropertyDescriptors.Length
      
    //--------------------------------------------------------------------------
    let getIndex (o:IjsObj) (name:string) =
      o.PropertyMap.PropertyMap.TryGetValue name
      
    //--------------------------------------------------------------------------
    let setMap (o:IjsObj) pc =
      o.PropertyMap <- pc

    //--------------------------------------------------------------------------
    let makeDynamic (o:IjsObj) =
      if o.PropertyMapId >= 0L then
        o.PropertyMap <- PropertyMap.makeDynamic o.PropertyMap
      
    //--------------------------------------------------------------------------
    let expandStorage (o:IjsObj) =
      let values = o.PropertyDescriptors
      let required = requiredStorage o
      let newValues = Array.zeroCreate (required * 2)

      if values.Length > 0 then 
        Array.Copy(values, newValues, values.Length)
      
      o.PropertyDescriptors <- newValues
      
    //--------------------------------------------------------------------------
    let ensureIndex (o:IjsObj) (name:string) =
      match getIndex o name with
      | true, index -> index
      | _ -> 
        o.PropertyMap <- PropertyMap.getSubMap o.PropertyMap name
        if isFull o then expandStorage o
        o.PropertyMap.PropertyMap.[name]
        
    //--------------------------------------------------------------------------
    let find (o:IjsObj) name =
      let rec find o name =
        if Utils.isNull o then (null, -1)
        else
          match getIndex o name with
          | true, index ->  o, index
          | _ -> find o.Prototype name

      find o name
      
    //--------------------------------------------------------------------------
    #if DEBUG
    let putBox (o:IjsObj) (name:IjsStr) (val':IjsBox) =
    #else
    let inline putBox (o:IjsObj) (name:IjsStr) (val':IjsBox) =
    #endif
      let index = ensureIndex o name
      o.PropertyDescriptors.[index].Box <- val'
      o.PropertyDescriptors.[index].HasValue <- true

    //--------------------------------------------------------------------------
    #if DEBUG
    let putRef (o:IjsObj) (name:IjsStr) (val':ClrObject) (tc:TypeTag) =
    #else
    let inline putRef (o:IjsObj) (name:IjsStr) (val':ClrObject) (tc:TypeTag) =
    #endif
      let index = ensureIndex o name
      o.PropertyDescriptors.[index].Box.Clr <- val'
      o.PropertyDescriptors.[index].Box.Tag <- tc
      
    //--------------------------------------------------------------------------
    #if DEBUG
    let putVal (o:IjsObj) (name:IjsStr) (val':IjsNum) =
    #else
    let inline putVal (o:IjsObj) (name:IjsStr) (val':IjsNum) =
    #endif
      let index = ensureIndex o name
      o.PropertyDescriptors.[index].Box.Number <- val'
      o.PropertyDescriptors.[index].HasValue <- true

    //--------------------------------------------------------------------------
    #if DEBUG
    let get (o:IjsObj) (name:IjsStr) =
    #else
    let inline get (o:IjsObj) (name:IjsStr) =
    #endif
      match find o name with
      | _, -1 -> Utils.BoxedConstants.undefined
      | pair -> (fst pair).PropertyDescriptors.[snd pair].Box

    //--------------------------------------------------------------------------
    let has (o:IjsObj) (name:IjsStr) =
      find o name |> snd > -1

    //--------------------------------------------------------------------------
    let delete (o:IjsObj) (name:IjsStr) =
      match getIndex o name with
      | true, index -> 
        setMap o (PropertyMap.delete(o.PropertyMap, name))

        let attrs = o.PropertyDescriptors.[index].Attributes
        let canDelete = Utils.Descriptor.isDeletable attrs

        if canDelete then
          o.PropertyDescriptors.[index].HasValue <- false
          o.PropertyDescriptors.[index].Box.Clr <- null
          o.PropertyDescriptors.[index].Box.Number <- 0.0

        canDelete

      | _ -> true
      
    //--------------------------------------------------------------------------
    module Delegates =
      let putBox = PutBoxProperty putBox
      let putVal = PutValProperty putVal
      let putRef = PutRefProperty putRef
      let get = GetProperty get
      let has = HasProperty has
      let delete = DeleteProperty delete
    
  //----------------------------------------------------------------------------
  module Index =
  
    open Extensions
    open Utils.Patterns
  
    //--------------------------------------------------------------------------
    let initSparse (o:IjsObj) =
      o.IndexSparse <- new MutableSorted<uint32, Box>()

      for i = 0 to int (o.IndexLength-1u) do
        if Utils.Descriptor.hasValue o.IndexDense.[i] then
          o.IndexSparse.Add(uint32 i, o.IndexDense.[i].Box)

      o.IndexDense <- null
      
    //--------------------------------------------------------------------------
    let expandStorage (o:IjsObj) i =
      if o.IndexSparse = null || o.IndexDense.Length <= i then
        let size = if i >= 1073741823 then 2147483647 else ((i+1) * 2)
        let values = o.IndexDense
        let newValues = Array.zeroCreate size

        if values <> null && values.Length > 0 then
          Array.Copy(values, newValues, values.Length)

        o.IndexDense <- newValues
        
    //--------------------------------------------------------------------------
    let updateLength (o:IjsObj) (i:uint32) =
      if i > o.IndexLength then
        o.IndexLength <- i+1u
        if o.Class = Classes.Array then
          Property.putVal o "length" (double i)

    //--------------------------------------------------------------------------
    let find (o:IjsObj) (i:uint32) =
      let rec find o (i:uint32) =
        if Utils.isNull o then (null, 0u, false)
        else 
          if Utils.Object.isDense o then
            let ii = int i
            if ii < o.IndexDense.Length then
              if Utils.Descriptor.hasValue o.IndexDense.[ii] 
                then (o, i, true)
                else find o.Prototype i
            else find o.Prototype i
          else
            if o.IndexSparse.ContainsKey i 
              then (o, i, false)
              else find o.Prototype i

      find o i
      
    //--------------------------------------------------------------------------
    let hasIndex (o:IjsObj) (i:uint32) =
      if Utils.Object.isDense o then
        let ii = int i
        if ii < o.IndexDense.Length 
          then Utils.Descriptor.hasValue o.IndexDense.[ii]
          else false
      else
        o.IndexSparse.ContainsKey i

    //--------------------------------------------------------------------------
    #if DEBUG
    let putBox (o:IjsObj) (i:uint32) (v:IjsBox) =
    #else
    let inline putBox (o:IjsObj) (i:uint32) (v:IjsBox) =
    #endif
      if i > Array.MaxIndex then initSparse o
      if Utils.Object.isDense o then
        if i > 255u && i/2u > o.IndexLength then
          initSparse o
          o.IndexSparse.[i] <- v

        else
          let i = int i
          if i >= o.IndexDense.Length then expandStorage o i
          o.IndexDense.[i].Box <- v
          o.IndexDense.[i].HasValue <- true

      else
        o.IndexSparse.[i] <- v

      updateLength o i


    //--------------------------------------------------------------------------
    #if DEBUG
    let putVal (o:IjsObj) (i:uint32) (v:IjsNum) =
    #else
    let inline putVal (o:IjsObj) (i:uint32) (v:IjsNum) =
    #endif
      if i > Array.MaxIndex then initSparse o
      if Utils.Object.isDense o then
        if i > 255u && i/2u > o.IndexLength then
          initSparse o
          o.IndexSparse.[i] <- Utils.boxVal v

        else
          let i = int i
          if i >= o.IndexDense.Length then expandStorage o i
          o.IndexDense.[i].Box.Number <- v
          o.IndexDense.[i].HasValue <- true

      else
        o.IndexSparse.[i] <- Utils.boxVal v

      updateLength o i

    //--------------------------------------------------------------------------
    #if DEBUG
    let putRef (o:IjsObj) (i:uint32) (v:ClrObject) (tc:TypeTag) =
    #else
    let inline putRef (o:IjsObj) (i:uint32) (v:ClrObject) (tc:TypeTag) =
    #endif
      if i > Array.MaxIndex then initSparse o
      if Utils.Object.isDense o then
        if i > 255u && i/2u > o.IndexLength then
          initSparse o
          o.IndexSparse.[i] <- Utils.boxRef v tc

        else
          let i = int i
          if i >= o.IndexDense.Length then expandStorage o i
          o.IndexDense.[i].Box.Clr <- v
          o.IndexDense.[i].Box.Tag <- tc

      else
        o.IndexSparse.[i] <- Utils.boxRef v tc

      updateLength o i

    //--------------------------------------------------------------------------
    #if DEBUG
    let get (o:IjsObj) (i:uint32) =
    #else
    let inline get (o:IjsObj) (i:uint32) =
    #endif
      match find o i with
      | null, _, _ -> Utils.BoxedConstants.undefined
      | o, index, isDense ->
        if isDense 
          then o.IndexDense.[int index].Box
          else o.IndexSparse.[index]
          
    //--------------------------------------------------------------------------
    let has (o:IjsObj) (i:uint32) =
      match find o i with
      | null, _, _ -> false
      | _ -> true

    //--------------------------------------------------------------------------
    let delete (o:IjsObj) (i:uint32) =
      match find o i with
      | null, _, _ -> true
      | o2, index, isDense ->
        if Utils.refEquals o o2 then
          if isDense then 
            o.IndexDense.[int i].Box <- Box()
            o.IndexDense.[int i].HasValue <- false
          else 
            o.IndexSparse.Remove i |> ignore

          true
        else false
        
    //--------------------------------------------------------------------------
    module Delegates =
      let putBox = PutBoxIndex putBox
      let putVal = PutValIndex putVal
      let putRef = PutRefIndex putRef
      let get = GetIndex get
      let has = HasIndex has
      let delete = DeleteIndex delete
    
    //--------------------------------------------------------------------------
    type Converters =
    
      //------------------------------------------------------------------------
      static member put (o:IjsObj, index:IjsBox, value:IjsBox) =
        match index with
        | NumberAndIndex i 
        | StringAndIndex i -> o.put(i, value)
        | Tagged tc -> o.put(TypeConverter.toString index, value)
        | _ -> failwith "Que?"
      
      static member put (o:IjsObj, index:IjsBool, value:IjsBox) =
        o.put(TypeConverter.toString index, value)
      
      static member put (o:IjsObj, index:IjsNum, value:IjsBox) =
        match index with
        | NumberIndex i -> o.put(i, value)
        | _ -> o.put(TypeConverter.toString index, value)
        
      static member put (o:IjsObj, index:ClrObject, value:IjsBox) =
        match TypeConverter.toString index with
        | StringIndex i -> o.put(i, value)
        | index -> o.put(index, value)

      static member put (o:IjsObj, index:Undefined, value:IjsBox) =
        o.put("undefined", value)
      
      static member put (o:IjsObj, index:IjsStr, value:IjsBox) =
        match index with
        | StringIndex i -> o.put(i, value)
        | _ -> o.put(TypeConverter.toString index, value)

      static member put (o:IjsObj, index:IjsObj, value:IjsBox) =
        match TypeConverter.toString index with
        | StringIndex i -> o.put(i, value)
        | index -> o.put(index, value)
        
      //------------------------------------------------------------------------
      static member put (o:IjsObj, index:IjsBox, value:IjsVal) =
        match index with
        | NumberAndIndex i
        | StringAndIndex i -> o.put(i, value)
        | Tagged tc -> o.put(TypeConverter.toString index, value)
        | _ -> failwith "Que?"
      
      static member put (o:IjsObj, index:IjsBool, value:IjsVal) =
        o.put(TypeConverter.toString index, value)
      
      static member put (o:IjsObj, index:IjsNum, value:IjsVal) =
        match index with
        | NumberIndex i -> o.put(i, value)
        | _ -> o.put(TypeConverter.toString index, value)
        
      static member put (o:IjsObj, index:ClrObject, value:IjsVal) =
        match TypeConverter.toString index with
        | StringIndex i -> o.put(i, value)
        | index -> o.put(index, value)

      static member put (o:IjsObj, index:Undefined, value:IjsVal) =
        o.put("undefined", value)
      
      static member put (o:IjsObj, index:IjsStr, value:IjsVal) =
        match index with
        | StringIndex i -> o.put(i, value)
        | _ -> o.put(TypeConverter.toString index, value)

      static member put (o:IjsObj, index:IjsObj, value:IjsVal) =
        match TypeConverter.toString index with
        | StringIndex i -> o.put(i, value)
        | index -> o.put(index, value)
        
      //------------------------------------------------------------------------
      static member put (o:IjsObj, index:IjsBox, value:IjsRef, tc:TypeTag) =
        match index with
        | NumberAndIndex i
        | StringAndIndex i -> o.put(i, value, tc)
        | Tagged tc -> o.put(TypeConverter.toString index, value)
        | _ -> failwith "Que?"
      
      static member put (o:IjsObj, index:IjsBool, value:IjsRef, tc:TypeTag) =
        o.put(TypeConverter.toString index, value, tc)
      
      static member put (o:IjsObj, index:IjsNum, value:IjsRef, tc:TypeTag) =
        match index with
        | NumberIndex i -> o.put(i, value)
        | _ -> o.put(TypeConverter.toString index, value, tc)
        
      static member put (o:IjsObj, index:ClrObject, value:IjsRef, tc:TypeTag) =
        match TypeConverter.toString index with
        | StringIndex i -> o.put(i, value, tc)
        | index -> o.put(index, value, tc)

      static member put (o:IjsObj, index:Undefined, value:IjsRef, tc:TypeTag) =
        o.put("undefined", value, tc)
      
      static member put (o:IjsObj, index:IjsStr, value:IjsRef, tc:TypeTag) =
        match index with
        | StringIndex i -> o.put(i, value, tc)
        | _ -> o.put(TypeConverter.toString index, value, tc)

      static member put (o:IjsObj, index:IjsObj, value:IjsRef, tc:TypeTag) =
        match TypeConverter.toString index with
        | StringIndex i -> o.put(i, value, tc)
        | index -> o.put(index, value, tc)

      //------------------------------------------------------------------------
      static member get (o:IjsObj, index:IjsBox) =
        match index with
        | NumberAndIndex i
        | StringAndIndex i -> o.get i
        | Tagged tc -> o.get(TypeConverter.toString index)
        | _ -> failwith "Que?"
      
      static member get (o:IjsObj, index:IjsBool) =
        o.get(TypeConverter.toString index)
      
      static member get (o:IjsObj, index:IjsNum) =
        match index with
        | NumberIndex i -> o.get i
        | _ -> o.get(TypeConverter.toString index)
        
      static member get (o:IjsObj, index:ClrObject) =
        match TypeConverter.toString index with
        | StringIndex i -> o.get i
        | index -> o.get(TypeConverter.toString index)

      static member get (o:IjsObj, index:Undefined) =
        o.get("undefined")
      
      static member get (o:IjsObj, index:IjsStr) =
        match index with
        | StringIndex i -> o.get i
        | _ -> o.get index

      static member get (o:IjsObj, index:IjsObj) =
        match TypeConverter.toString index with
        | StringIndex i -> o.get i
        | index -> o.get index

      //------------------------------------------------------------------------
      static member has (o:IjsObj, index:IjsBox) =
        match index with
        | NumberAndIndex i
        | StringAndIndex i -> o.has i
        | Tagged tc -> o.has(TypeConverter.toString index)
        | _ -> failwith "Que?"
      
      static member has (o:IjsObj, index:IjsBool) =
        o.has(TypeConverter.toString index)
      
      static member has (o:IjsObj, index:IjsNum) =
        match index with
        | NumberIndex i -> o.has i
        | _ -> o.has(TypeConverter.toString index)
        
      static member has (o:IjsObj, index:ClrObject) =
        match TypeConverter.toString index with
        | StringIndex i -> o.has i
        | index -> o.has(TypeConverter.toString index)

      static member has (o:IjsObj, index:Undefined) =
        o.has("undefined")
      
      static member has (o:IjsObj, index:IjsStr) =
        match index with
        | StringIndex i -> o.has i
        | _ -> o.has index

      static member has (o:IjsObj, index:IjsObj) =
        Converters.has(o, TypeConverter.toPrimitive index)

module Arguments =

  module Index =

    //--------------------------------------------------------------------------
    let putBox (o:IjsObj) (i:uint32) (v:IjsBox) =
      let a = o :?> Arguments
      let ii = int i

      if a.LinkIntact && ii < a.LinkMap.Length then
        match a.LinkMap.[ii] with
        | ArgumentsLinkArray.Locals, index -> a.Locals.[index] <- v
        | ArgumentsLinkArray.ClosedOver, index -> a.ClosedOver.[index] <- v
        | _ -> failwith "Que?"

      Object.Index.putBox o i v
  
    //--------------------------------------------------------------------------
    let putVal (o:IjsObj) (i:uint32) (v:IjsNum) =
      let a = o :?> Arguments
      let ii = int i

      if a.LinkIntact && ii < a.LinkMap.Length then
        match a.LinkMap.[ii] with
        | ArgumentsLinkArray.Locals, index -> a.Locals.[index].Number <- v
        | ArgumentsLinkArray.ClosedOver, index -> 
          a.ClosedOver.[index].Number <- v
        | _ -> failwith "Que?"

      Object.Index.putVal o i v

    //--------------------------------------------------------------------------
    let putRef (o:IjsObj) (i:uint32) (v:IjsRef) (tag:TypeTag) =
      let a = o :?> Arguments
      let ii = int i

      if a.LinkIntact && ii < a.LinkMap.Length then
        match a.LinkMap.[ii] with
        | ArgumentsLinkArray.Locals, index -> 
          a.Locals.[index].Clr <- v
          a.Locals.[index].Tag <- tag

        | ArgumentsLinkArray.ClosedOver, index -> 
          a.ClosedOver.[index].Clr <- v
          a.ClosedOver.[index].Tag <- tag

        | _ -> failwith "Que?"

      Object.Index.putRef o i v tag
    
    //--------------------------------------------------------------------------
    let get (o:IjsObj) (i:uint32) =
      let a = o :?> Arguments
      let ii = int i

      if a.LinkIntact && ii < a.LinkMap.Length then
        match a.LinkMap.[ii] with
        | ArgumentsLinkArray.Locals, index -> a.Locals.[index]
        | ArgumentsLinkArray.ClosedOver, index -> a.ClosedOver.[index]
        | _ -> failwith "Que?"

      else
        Object.Index.get o i
        
    //--------------------------------------------------------------------------
    let has (o:IjsObj) (i:uint32) =
      let a = o :?> Arguments
      let ii = int i

      if a.LinkIntact && ii < a.LinkMap.Length 
        then true
        else Object.Index.has o i
        
    //--------------------------------------------------------------------------
    let delete (o:IjsObj) (i:uint32) =
      let a = o :?> Arguments
      let ii = int i

      if a.LinkIntact && ii < a.LinkMap.Length then
        a.copyLinkedValues()
        a.LinkIntact <- false
        a.Locals <- null
        a.ClosedOver <- null

      Object.Index.delete o i
        
    //--------------------------------------------------------------------------
    module Delegates =
      let putBox = PutBoxIndex putBox
      let putVal = PutValIndex putVal
      let putRef = PutRefIndex putRef
      let get = GetIndex get
      let has = HasIndex has
      let delete = DeleteIndex delete
      
//------------------------------------------------------------------------------
module DynamicScope =
  
  //----------------------------------------------------------------------------
  let findObject name (dc:DynamicScope) stop =
    let rec find (dc:DynamicScope) =
      match dc with
      | [] -> None
      | (level, o)::xs ->
        if level >= stop then
          let mutable h = null
          let mutable i = 0
          if o.Methods.HasProperty.Invoke(o, name)
            then Some o
            else find xs
        else
          None

    find dc
      
  //----------------------------------------------------------------------------
  let findVariable name (dc:DynamicScope) stop = 
    match findObject name dc stop with
    | Some o -> Some(o.Methods.GetProperty.Invoke(o, name))
    | _ -> None
      
  //----------------------------------------------------------------------------
  let set (name) dc stop (g:IjsObj) (s:Scope) i =
    match findObject name dc stop with
    | Some o -> o.Methods.GetProperty.Invoke(o, name)
    | _ -> if s = null then g.Methods.GetProperty.Invoke(g, name) else s.[i]
      
  //----------------------------------------------------------------------------
  let get name (v:IjsBox) dc stop (g:IjsObj) (s:Scope) i =
    match findObject name dc stop with
    | Some o -> o.Methods.PutBoxProperty.Invoke(o, name, v)
    | _ -> 
      if s = null 
        then g.Methods.PutBoxProperty.Invoke(g, name, v) 
        else s.[i] <- v
          
  //----------------------------------------------------------------------------
  let call<'a when 'a :> Delegate> name args dc stop g (s:Scope) i =

    let this, func = 
      match findObject name dc stop with
      | Some o -> o, (o.Methods.GetProperty.Invoke(o, name))
      | _ -> g, if s=null then g.Methods.GetProperty.Invoke(g, name) else s.[i]

    if func.Tag >= TypeTags.Function then
      let func = func.Func
      let internalArgs = [|func :> obj; this :> obj|]
      let compiled = func.Compiler.compileAs<'a> func
      Utils.box (compiled.DynamicInvoke(Array.append internalArgs args))

    else
      Errors.runtime "Can only call javascript function dynamically"
        
  //----------------------------------------------------------------------------
  let delete (dc:DynamicScope) (g:IjsObj) name =
    match findObject name dc -1 with
    | Some o -> o.Methods.DeleteProperty.Invoke(o, name)
    | _ -> g.Methods.DeleteProperty.Invoke(g, name)

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