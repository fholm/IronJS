namespace IronJS.Api

open System
open IronJS
open IronJS.Aliases

//------------------------------------------------------------------------------
// Static class containing all type conversions
//------------------------------------------------------------------------------
type TypeConverter =

  //----------------------------------------------------------------------------
  static member toBox(b:Box byref) = b
  static member toBox(d:double) = Utils.boxDouble d
  static member toBox(b:bool) = Utils.boxBool b
  static member toBox(s:string) = Utils.boxString s
  static member toBox(o:IjsObj) = Utils.boxObject o
  static member toBox(f:IjsFunc) = Utils.boxFunction f
  static member toBox(c:HostObject) = Utils.boxClr c
  static member toBox(expr:Dlr.Expr) = 
    Dlr.callStaticT<TypeConverter> "toBox" [expr]
    
  //----------------------------------------------------------------------------
  static member toHostObject(d:double) = box d
  static member toHostObject(b:bool) = box b
  static member toHostObject(s:string) = box s
  static member toHostObject(o:IjsObj) = box o
  static member toHostObject(f:IjsFunc) = box f
  static member toHostObject(c:HostObject) = c
  static member toHostObject(b:Box byref) =
    match b.Type with
    | TypeCodes.Empty -> null
    | TypeCodes.Undefined -> null
    | TypeCodes.String -> b.String :> HostObject
    | TypeCodes.Bool -> b.Bool :> HostObject
    | TypeCodes.Number -> b.Double :> HostObject
    | TypeCodes.Clr -> b.Clr
    | TypeCodes.Object -> b.Object :> HostObject
    | TypeCodes.Function -> b.Func :> HostObject
    | _ -> Errors.Generic.invalidTypeCode b.Type

  static member toHostObject (expr:Dlr.Expr) =
    Dlr.callStaticT<TypeConverter> "toHostObject" [expr]

  //----------------------------------------------------------------------------
  static member toString (b:bool) = if b then "true" else "false"
  static member toString (s:string) = s
  static member toString (u:Undefined) = "undefined"
  static member toString (b:Box byref) =
    if Utils.Box.isNumber b.Tag then TypeConverter.toString(b.Double)
    else
      match b.Type with
      | TypeCodes.Undefined -> "undefined"
      | TypeCodes.String -> b.String
      | TypeCodes.Bool -> TypeConverter.toString b.Bool
      | TypeCodes.Clr -> TypeConverter.toString b.Clr
      | TypeCodes.Object -> TypeConverter.toString b.Object
      | TypeCodes.Function -> TypeConverter.toString (b.Func :> IjsObj)
      | _ -> Errors.Generic.invalidTypeCode b.Type

  static member toString (o:IjsObj) = 
    let mutable v = Object.defaultValue(o, DefaultValue.String)
    TypeConverter.toString(&v)

  static member toString (d:double) = 
    if System.Double.IsInfinity d then "Infinity" else d.ToString()

  static member toString (c:HostObject) = 
    if c = null then "null" else c.ToString()

  static member toString (expr:Dlr.Expr) =
    Dlr.callStaticT<TypeConverter> "toString" [expr]
      
  //----------------------------------------------------------------------------
  static member toPrimitive (b:bool, _:byte) = Utils.boxBool b
  static member toPrimitive (d:double, _:byte) = Utils.boxDouble d
  static member toPrimitive (s:string, _:byte) = Utils.boxString s
  static member toPrimitive (u:Undefined, _:byte) = Utils.boxUndefined u
  static member toPrimitive (o:IjsObj, h:byte) = Object.defaultValue(o, h)
  static member toPrimitive (o:IjsObj) = Object.defaultValue(o)
  static member toPrimitive (b:Box byref, h:byte) =
    match b.Type with
    | TypeCodes.Bool
    | TypeCodes.Number
    | TypeCodes.String
    | TypeCodes.Empty
    | TypeCodes.Undefined -> b
    | TypeCodes.Clr -> TypeConverter.toPrimitive(b.Clr, h)
    | TypeCodes.Object
    | TypeCodes.Function -> Object.defaultValue(b.Object, h)
    | _ -> Errors.Generic.invalidTypeCode b.Type
  
  static member toPrimitive (c:HostObject, _:byte) = 
    Utils.boxClr (if c = null then null else c.ToString())

  static member toPrimitive (expr:Dlr.Expr) =
    Dlr.callStaticT<TypeConverter> "toPrimitive" [expr]
      
  //----------------------------------------------------------------------------
  static member toBoolean (b:bool) = b
  static member toBoolean (d:double) = d > 0.0 || d < 0.0
  static member toBoolean (c:HostObject) = if c = null then false else true
  static member toBoolean (s:string) = s.Length > 0
  static member toBoolean (u:Undefined) = false
  static member toBoolean (o:IjsObj) = true
  static member toBoolean (b:Box byref) =
    match b.Type with 
    | TypeCodes.Bool -> b.Bool
    | TypeCodes.Empty
    | TypeCodes.Undefined -> false
    | TypeCodes.Number -> TypeConverter.toBoolean b.Double
    | TypeCodes.String -> b.String.Length > 0
    | TypeCodes.Clr -> TypeConverter.toBoolean b.Clr
    | TypeCodes.Object 
    | TypeCodes.Function -> true
    | _ -> Errors.Generic.invalidTypeCode b.Type
    
  static member toBoolean (expr:Dlr.Expr) =
    Dlr.callStaticT<TypeConverter> "toBoolean" [expr]

  //----------------------------------------------------------------------------
  static member toNumber (b:bool) : double = if b then 1.0 else 0.0
  static member toNumber (d:double) = d
  static member toNumber (c:HostObject) = if c = null then 0.0 else 1.0
  static member toNumber (u:Undefined) = Number.NaN
  static member toNumber (o:IjsObj) : Number = 
    TypeConverter.toNumber(Object.defaultValue(o, DefaultValue.Number))

  static member toNumber (b:Box byref) =
    match b.Type with
    | TypeCodes.Number -> b.Double
    | TypeCodes.Bool -> if b.Bool then 1.0 else 0.0
    | TypeCodes.String -> TypeConverter.toNumber(b.String)
    | TypeCodes.Empty
    | TypeCodes.Undefined -> NaN
    | TypeCodes.Clr -> TypeConverter.toNumber b.Clr
    | TypeCodes.Object 
    | TypeCodes.Function -> TypeConverter.toNumber(b.Object)
    | _ -> Errors.Generic.invalidTypeCode b.Type

  static member toNumber (s:string) = 
    let mutable d = 0.0
    if Double.TryParse(s, anyNumber, invariantCulture, &d) 
      then d
      else NaN

  static member toNumber (expr:Dlr.Expr) = 
    Dlr.callStaticT<TypeConverter> "toNumber" [expr]
        
  //----------------------------------------------------------------------------
  static member toObject (env:IjsEnv, o:IjsObj) = o
  static member toObject (env:IjsEnv, b:Box byref) =
    match b.Type with
    | TypeCodes.Function
    | TypeCodes.Object -> b.Object
    | TypeCodes.Empty
    | TypeCodes.Undefined
    | TypeCodes.Clr -> Errors.Generic.notImplemented()
    | TypeCodes.String -> Environment.createObject(env, b.String)
    | TypeCodes.Number -> Environment.createObject(env, b.Double)
    | TypeCodes.Bool -> Environment.createObject(env, b.Bool)
    | _ -> Errors.Generic.invalidTypeCode b.Type

  static member toObject (env:IjsEnv, b:Box) =
    match b.Type with
    | TypeCodes.Function
    | TypeCodes.Object -> b.Object
    | TypeCodes.Empty
    | TypeCodes.Undefined
    | TypeCodes.Clr -> Errors.Generic.notImplemented()
    | TypeCodes.String -> Environment.createObject(env, b.String)
    | TypeCodes.Number -> Environment.createObject(env, b.Double)
    | TypeCodes.Bool -> Environment.createObject(env, b.Bool)
    | _ -> Errors.Generic.invalidTypeCode b.Type

  static member toObject (env:Dlr.Expr, expr:Dlr.Expr) =
    Dlr.callStaticT<TypeConverter> "toObject" [env; expr]
      
  //----------------------------------------------------------------------------
  static member toInt32 (d:double) = int d
  static member toUInt32 (d:double) = uint32 d
  static member toUInt16 (d:double) = uint16 d
  static member toInteger (d:double) : double = 
    if d = NaN
      then 0.0
      elif d = 0.0 || d = NegInf || d = PosInf
        then d
        else double (Math.Sign(d)) * Math.Floor(Math.Abs(d))
                
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
      elif t = typeof<HostObject> then TypeConverter.toHostObject expr
      else Errors.Generic.noConversion expr.Type t

  static member convertToT<'a> env expr = 
    TypeConverter.convertTo env expr typeof<'a>
    
//------------------------------------------------------------------------------
// Operators
//------------------------------------------------------------------------------
and Operators =

  //----------------------------------------------------------------------------
  // Unary
  //----------------------------------------------------------------------------

  //----------------------------------------------------------------------------
  // typeof
  static member typeOf (o:Box byref) = TypeCodes.Names.[o.Type]
  static member typeOf expr = Dlr.callStaticT<Operators> "typeOf" [expr]
  
  //----------------------------------------------------------------------------
  // !
  static member not (o) = Dlr.callStaticT<Operators> "not" [o]
  static member not (o:Box byref) =
    not (TypeConverter.toBoolean &o)
    
  //----------------------------------------------------------------------------
  // ~
  static member bitCmpl (o) = Dlr.callStaticT<Operators> "bitCmpl" [o]
  static member bitCmpl (o:Box byref) =
    let o = TypeConverter.toNumber &o
    let o = TypeConverter.toInt32 o
    Utils.boxDouble (double (~~~ o))

  //----------------------------------------------------------------------------
  // Binary
  //----------------------------------------------------------------------------
    
  //----------------------------------------------------------------------------
  // <
  static member lt (l, r) = Dlr.callStaticT<Operators> "lt" [l; r]
  static member lt (l:Box byref, r:Box byref) =
    if l.Type = TypeCodes.Number && r.Type = TypeCodes.Number 
      then l.Double < r.Double
      elif l.Type = TypeCodes.String && r.Type = TypeCodes.String
        then l.String < r.String
        else TypeConverter.toNumber l < TypeConverter.toNumber r
        
  //----------------------------------------------------------------------------
  // <=
  static member ltEq (l, r) = Dlr.callStaticT<Operators> "ltEq" [l; r]
  static member ltEq (l:Box byref, r:Box byref) =
    if l.Type = TypeCodes.Number && r.Type = TypeCodes.Number 
      then l.Double <= r.Double
      elif l.Type = TypeCodes.String && r.Type = TypeCodes.String
        then l.String <= r.String
        else TypeConverter.toNumber l <= TypeConverter.toNumber r
        
  //----------------------------------------------------------------------------
  // >
  static member gt (l, r) = Dlr.callStaticT<Operators> "gt" [l; r]
  static member gt (l:Box byref, r:Box byref) =
    if l.Type = TypeCodes.Number && r.Type = TypeCodes.Number 
      then l.Double > r.Double
      elif l.Type = TypeCodes.String && r.Type = TypeCodes.String
        then l.String > r.String
        else TypeConverter.toNumber l > TypeConverter.toNumber r
        
  //----------------------------------------------------------------------------
  // >=
  static member gtEq (l, r) = Dlr.callStaticT<Operators> "gtEq" [l; r]
  static member gtEq (l:Box byref, r:Box byref) =
    if l.Type = TypeCodes.Number && r.Type = TypeCodes.Number 
      then l.Double >= r.Double
      elif l.Type = TypeCodes.String && r.Type = TypeCodes.String
        then l.String >= r.String
        else TypeConverter.toNumber l >= TypeConverter.toNumber r
        
  //----------------------------------------------------------------------------
  // ==
  static member eq (l, r) = Dlr.callStaticT<Operators> "eq" [l; r]
  static member eq (l:Box byref, r:Box byref) = 
    if l.Type = r.Type then
      match l.Type with
      | TypeCodes.Empty
      | TypeCodes.Undefined -> true
      | TypeCodes.Number -> l.Double = r.Double
      | TypeCodes.String -> l.String = r.String
      | TypeCodes.Bool -> l.Bool = r.Bool
      | TypeCodes.Clr
      | TypeCodes.Function
      | TypeCodes.Object -> Object.ReferenceEquals(l.Clr, r.Clr)
      | _ -> false

    else
      if l.Type = TypeCodes.Clr 
        && l.Clr = null 
        && (r.Type = TypeCodes.Undefined 
            || r.Type = TypeCodes.Empty) then true
      
      elif r.Type = TypeCodes.Clr 
        && r.Clr = null 
        && (l.Type = TypeCodes.Undefined 
            || l.Type = TypeCodes.Empty) then true

      elif l.Type = TypeCodes.Number && r.Type = TypeCodes.String then
        l.Double = TypeConverter.toNumber r.String
        
      elif r.Type = TypeCodes.String && r.Type = TypeCodes.Number then
        TypeConverter.toNumber l.String = r.Double

      elif l.Type = TypeCodes.Bool then
        let mutable l = Utils.boxDouble(TypeConverter.toNumber &l)
        Operators.eq(&l, &r)

      elif r.Type = TypeCodes.Bool then
        let mutable r = Utils.boxDouble(TypeConverter.toNumber &r)
        Operators.eq(&l, &r)

      elif r.Type >= TypeCodes.Object then
        match l.Type with
        | TypeCodes.Number
        | TypeCodes.String -> 
          let mutable r = TypeConverter.toPrimitive(r.Object)
          Operators.eq(&l, &r)
        | _ -> false

      elif l.Type >= TypeCodes.Object then
        match r.Type with
        | TypeCodes.Number
        | TypeCodes.String -> 
          let mutable l = TypeConverter.toPrimitive(l.Object)
          Operators.eq(&l, &r)
        | _ -> false

      else
        false
        
  //----------------------------------------------------------------------------
  // !=
  static member notEq (l, r) = Dlr.callStaticT<Operators> "notEq" [l; r]
  static member notEq (l:Box byref, r:Box byref) = not (Operators.eq(&l, &r))
  
  //----------------------------------------------------------------------------
  // ===
  static member same (l, r) = Dlr.callStaticT<Operators> "same" [l; r]
  static member same (l:Box byref, r:Box byref) = 
    if l.Type = r.Type then
      match l.Type with
      | TypeCodes.Empty
      | TypeCodes.Undefined -> true
      | TypeCodes.Number -> l.Double = r.Double
      | TypeCodes.String -> l.String = r.String
      | TypeCodes.Bool -> l.Bool = r.Bool
      | TypeCodes.Clr
      | TypeCodes.Function
      | TypeCodes.Object -> Object.ReferenceEquals(l.Clr, r.Clr)
      | _ -> false

    else
      false
      
  //----------------------------------------------------------------------------
  // !==
  static member notSame (l, r) = Dlr.callStaticT<Operators> "notSame" [l; r]
  static member notSame (l:Box byref, r:Box byref) =
    not (Operators.same(&l, &r))
    
  //----------------------------------------------------------------------------
  // +
  static member add (l, r) = Dlr.callStaticT<Operators> "add" [l; r]
  static member add (l:Box byref, r:Box byref) = 
    if l.Type = TypeCodes.Number && r.Type = TypeCodes.Number then
      Utils.boxDouble (l.Double + r.Double)

    elif l.Type = TypeCodes.String || r.Type = TypeCodes.String then
      Utils.boxString (TypeConverter.toString(&l) + TypeConverter.toString(&r))

    else
      Utils.boxDouble (TypeConverter.toNumber(&l) + TypeConverter.toNumber(&r))
      
  //----------------------------------------------------------------------------
  // -
  static member sub (l, r) = Dlr.callStaticT<Operators> "sub" [l; r]
  static member sub (l:Box byref, r:Box byref) =
    if l.Type = TypeCodes.Number && r.Type = TypeCodes.Number then
      Utils.boxDouble (l.Double - r.Double)

    else
      Utils.boxDouble (TypeConverter.toNumber(&l) - TypeConverter.toNumber(&r))
      
  //----------------------------------------------------------------------------
  // /
  static member div (l, r) = Dlr.callStaticT<Operators> "div" [l; r]
  static member div (l:Box byref, r:Box byref) =
    if l.Type = TypeCodes.Number && r.Type = TypeCodes.Number then
      Utils.boxDouble (l.Double / r.Double)

    else
      Utils.boxDouble (TypeConverter.toNumber(&l) / TypeConverter.toNumber(&r))
      
  //----------------------------------------------------------------------------
  // *
  static member mul (l, r) = Dlr.callStaticT<Operators> "mul" [l; r]
  static member mul (l:Box byref, r:Box byref) =
    if l.Type = TypeCodes.Number && r.Type = TypeCodes.Number then
      Utils.boxDouble (l.Double * r.Double)

    else
      Utils.boxDouble (TypeConverter.toNumber(&l) * TypeConverter.toNumber(&r))
      
  //----------------------------------------------------------------------------
  // %
  static member mod' (l, r) = Dlr.callStaticT<Operators> "mod'" [l; r]
  static member mod' (l:Box byref, r:Box byref) =
    if l.Type = TypeCodes.Number && r.Type = TypeCodes.Number then
      Utils.boxDouble (l.Double % r.Double)

    else
      Utils.boxDouble (TypeConverter.toNumber &l % TypeConverter.toNumber &r)
      
  //----------------------------------------------------------------------------
  // + (unary)
  static member plus (l, r) = Dlr.callStaticT<Operators> "plus" [l; r]
  static member plus (o:Box byref) =
    Utils.boxDouble (TypeConverter.toNumber &o)
    
  //----------------------------------------------------------------------------
  // - (unary)
  static member minus (l, r) = Dlr.callStaticT<Operators> "minus" [l; r]
  static member minus (o:Box byref) =
    Utils.boxDouble ((TypeConverter.toNumber &o) * -1.0)
    
  //----------------------------------------------------------------------------
  // &
  static member bitAnd (l, r) = Dlr.callStaticT<Operators> "bitAnd" [l; r]
  static member bitAnd (l:Box byref, r:Box byref) =
    let l = TypeConverter.toNumber &l
    let r = TypeConverter.toNumber &r
    let l = TypeConverter.toInt32 l
    let r = TypeConverter.toInt32 r
    Utils.boxDouble (double (l &&& r))
    
  //----------------------------------------------------------------------------
  // |
  static member bitOr (l, r) = Dlr.callStaticT<Operators> "bitOr" [l; r]
  static member bitOr (l:Box byref, r:Box byref) =
    let l = TypeConverter.toNumber &l
    let r = TypeConverter.toNumber &r
    let l = TypeConverter.toInt32 l
    let r = TypeConverter.toInt32 r
    Utils.boxDouble (double (l ||| r))
    
  //----------------------------------------------------------------------------
  // ^
  static member bitXOr (l, r) = Dlr.callStaticT<Operators> "bitXOr" [l; r]
  static member bitXOr (l:Box byref, r:Box byref) =
    let l = TypeConverter.toNumber &l
    let r = TypeConverter.toNumber &r
    let l = TypeConverter.toInt32 l
    let r = TypeConverter.toInt32 r
    Utils.boxDouble (double (l ^^^ r))
    
  //----------------------------------------------------------------------------
  // <<
  static member bitLhs (l, r) = Dlr.callStaticT<Operators> "bitLhs" [l; r]
  static member bitLhs (l:Box byref, r:Box byref) =
    let l = TypeConverter.toNumber &l
    let r = TypeConverter.toNumber &r
    let l = TypeConverter.toInt32 l
    let r = TypeConverter.toUInt32 r &&& 0x1Fu
    Utils.boxDouble (double (l <<< int r))
    
  //----------------------------------------------------------------------------
  // >>
  static member bitRhs (l, r) = Dlr.callStaticT<Operators> "bitRhs" [l; r]
  static member bitRhs (l:Box byref, r:Box byref) =
    let l = TypeConverter.toNumber &l
    let r = TypeConverter.toNumber &r
    let l = TypeConverter.toInt32 l
    let r = TypeConverter.toUInt32 r &&& 0x1Fu
    Utils.boxDouble (double (l >>> int r))
    
  //----------------------------------------------------------------------------
  // >>>
  static member bitURhs (l, r) = Dlr.callStaticT<Operators> "bitURhs" [l; r]
  static member bitURhs (l:Box byref, r:Box byref) =
    let l = TypeConverter.toNumber &l
    let r = TypeConverter.toNumber &r
    let l = TypeConverter.toUInt32 l
    let r = TypeConverter.toUInt32 r &&& 0x1Fu
    Utils.boxDouble (double (l >>> int r))
    
  //----------------------------------------------------------------------------
  // &&
  static member and' (l, r) = Dlr.callStaticT<Operators> "and'" [l; r]
  static member and' (l:Box byref, r:Box byref) =
    if not (TypeConverter.toBoolean &l) then l else r
    
  //----------------------------------------------------------------------------
  // ||
  static member or' (l, r) = Dlr.callStaticT<Operators> "or'" [l; r]
  static member or' (l:Box byref, r:Box byref) =
    if TypeConverter.toBoolean &l then l else r
      


//------------------------------------------------------------------------------
// PropertyClass API
//------------------------------------------------------------------------------
and PropertyClass =
        
  //----------------------------------------------------------------------------
  static member subClass (x:IronJS.PropertyClass, name) = 
    if x.isDynamic then 
      let index = 
        if x.FreeIndexes.Count > 0 then x.FreeIndexes.Pop()
        else x.NextIndex <- x.NextIndex + 1; x.NextIndex - 1

      x.PropertyMap.Add(name, index)
      x

    else
      let mutable subClass = null
      
      if not(x.SubClasses.TryGetValue(name, &subClass)) then
        let newMap = new MutableDict<string, int>(x.PropertyMap)
        newMap.Add(name, newMap.Count)
        subClass <- IronJS.PropertyClass(x.Env, newMap)
        x.SubClasses.Add(name, subClass)

      subClass

  //----------------------------------------------------------------------------
  static member subClass (x:IronJS.PropertyClass, names:string seq) =
    Seq.fold (fun c (n:string) -> PropertyClass.subClass(c, n)) x names
        
  //----------------------------------------------------------------------------
  static member makeDynamic (x:IronJS.PropertyClass) =
    if x.isDynamic then x
    else
      let pc = new IronJS.PropertyClass(null)
      pc.Id <- -1L
      pc.NextIndex <- x.NextIndex
      pc.FreeIndexes <- new MutableStack<int>()
      pc.PropertyMap <- new MutableDict<string, int>(x.PropertyMap)
      pc
        
  //----------------------------------------------------------------------------
  static member delete (x:IronJS.PropertyClass, name) =
    let pc = if not x.isDynamic then PropertyClass.makeDynamic x else x
    let mutable index = 0

    if pc.PropertyMap.TryGetValue(name, &index) then 
      pc.FreeIndexes.Push index

    pc
      
  //----------------------------------------------------------------------------
  static member getIndex (x:IronJS.PropertyClass, name) =
    x.PropertyMap.[name]
    
//------------------------------------------------------------------------------
// Environment API
//------------------------------------------------------------------------------
and Environment =

  //----------------------------------------------------------------------------
  static member addCompiler (x:IjsEnv, funId, compiler) =
    if not (x.Compilers.ContainsKey funId) then
      x.Compilers.Add(funId, CachedCompiler compiler)

  //----------------------------------------------------------------------------
  static member addCompiler (x:IjsEnv, f:IjsFunc, compiler) =
    if not (x.Compilers.ContainsKey f.FunctionId) then
      f.Compiler <- CachedCompiler compiler
      x.Compilers.Add(f.FunctionId, f.Compiler)
  
  //----------------------------------------------------------------------------
  static member hasCompiler (x:IjsEnv, funcId) =
    x.Compilers.ContainsKey funcId

  //----------------------------------------------------------------------------
  static member createObject (x:IjsEnv, pc) =
    IjsObj(pc, x.Object_prototype, Classes.Object, 0u)
    
  //----------------------------------------------------------------------------
  static member createObject (x:IjsEnv) =
    IjsObj(x.Base_Class, x.Object_prototype, Classes.Object, 0u)
    
  //----------------------------------------------------------------------------
  static member createObject (x:IjsEnv, s:IjsStr) =
    let o = IjsObj(x.String_Class, x.String_prototype, Classes.String, 0u)
    Object.putProperty(o, "length", double s.Length) |> ignore
    o.Value.Box.Type <- TypeCodes.String
    o.Value.Box.String <-s
    o
    
  //----------------------------------------------------------------------------
  static member createObject (x:IjsEnv, n:IjsNum) =
    let o = IjsObj(x.Number_Class, x.Number_prototype, Classes.Number, 0u)
    o.Value.Box.Double <- n
    o
    
  //----------------------------------------------------------------------------
  static member createObject (x:IjsEnv, b:IjsBool) =
    let o = IjsObj(x.Boolean_Class, x.Boolean_prototype, Classes.Boolean, 0u)
    o.Value.Box.Type <- TypeCodes.Bool
    o.Value.Box.Bool <- b
    o
    
//------------------------------------------------------------------------------
// Function API
//------------------------------------------------------------------------------
and Function() =

  static let getPrototype(f:IjsFunc) =
    let prototype = Object.getProperty(f, "prototype")
    match prototype.Type with
    | TypeCodes.Function
    | TypeCodes.Object -> prototype.Object
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

  //----------------------------------------------------------------------------
  static member construct (f:IjsFunc,t:IjsObj) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,IjsBox>>(f)

    match f.ConstructorMode with
    | ConstructorModes.Host -> c.Invoke(f,null)
    | ConstructorModes.User -> 
      let o = Environment.createObject(f.Env)
      o.Prototype <- getPrototype f
      c.Invoke(f,o)

    | _ -> Errors.runtime "Can't call [[Construct]] on non-constructor"

  //----------------------------------------------------------------------------
  static member construct (f:IjsFunc,t:IjsObj,a0:'a0) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,IjsBox>>(f)

    match f.ConstructorMode with
    | ConstructorModes.Host -> c.Invoke(f,null,a0)
    | ConstructorModes.User -> 
      let o = Environment.createObject(f.Env)
      o.Prototype <- getPrototype f
      c.Invoke(f,o,a0)

    | _ -> Errors.runtime "Can't call [[Construct]] on non-constructor"

  //----------------------------------------------------------------------------
  static member construct (f:IjsFunc,t:IjsObj,a0:'a0,a1:'a1) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,IjsBox>>(f)

    match f.ConstructorMode with
    | ConstructorModes.Host -> c.Invoke(f,null,a0,a1)
    | ConstructorModes.User -> 
      let o = Environment.createObject(f.Env)
      o.Prototype <- getPrototype f
      c.Invoke(f,o,a0,a1)

    | _ -> Errors.runtime "Can't call [[Construct]] on non-constructor"

  //----------------------------------------------------------------------------
  static member construct (f:IjsFunc,t:IjsObj,a0:'a0,a1:'a1,a2:'a2) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,IjsBox>>(f)

    match f.ConstructorMode with
    | ConstructorModes.Host -> c.Invoke(f,null,a0,a1,a2)
    | ConstructorModes.User -> 
      let o = Environment.createObject(f.Env)
      o.Prototype <- getPrototype f
      c.Invoke(f,o,a0,a1,a2)

    | _ -> Errors.runtime "Can't call [[Construct]] on non-constructor"

  //----------------------------------------------------------------------------
  static member construct (f:IjsFunc,t:IjsObj,a0:'a0,a1:'a1,a2:'a2,a3:'a3) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,'a3,IjsBox>>(f)

    match f.ConstructorMode with
    | ConstructorModes.Host -> c.Invoke(f,null,a0,a1,a2,a3)
    | ConstructorModes.User -> 
      let o = Environment.createObject(f.Env)
      o.Prototype <- getPrototype f
      c.Invoke(f,o,a0,a1,a2,a3)

    | _ -> Errors.runtime "Can't call [[Construct]] on non-constructor"

  //----------------------------------------------------------------------------
  static member construct (f:IjsFunc,t:IjsObj,a0:'a0,a1:'a1,a2:'a2,a3:'a3,a4:'a4) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,'a3,'a4,IjsBox>>(f)

    match f.ConstructorMode with
    | ConstructorModes.Host -> c.Invoke(f,null,a0,a1,a2,a3,a4)
    | ConstructorModes.User -> 
      let o = Environment.createObject(f.Env)
      o.Prototype <- getPrototype f
      c.Invoke(f,o,a0,a1,a2,a3,a4)

    | _ -> Errors.runtime "Can't call [[Construct]] on non-constructor"

  //----------------------------------------------------------------------------
  static member construct (f:IjsFunc,t:IjsObj,a0:'a0,a1:'a1,a2:'a2,a3:'a3,a4:'a4,a5:'a5) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,'a3,'a4,'a5,IjsBox>>(f)

    match f.ConstructorMode with
    | ConstructorModes.Host -> c.Invoke(f,null,a0,a1,a2,a3,a4,a5)
    | ConstructorModes.User -> 
      let o = Environment.createObject(f.Env)
      o.Prototype <- getPrototype f
      c.Invoke(f,o,a0,a1,a2,a3,a4,a5)

    | _ -> Errors.runtime "Can't call [[Construct]] on non-constructor"

  //----------------------------------------------------------------------------
  static member construct (f:IjsFunc,t:IjsObj,a0:'a0,a1:'a1,a2:'a2,a3:'a3,a4:'a4,a5:'a5,a6:'a6) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,'a3,'a4,'a5,'a6,IjsBox>>(f)

    match f.ConstructorMode with
    | ConstructorModes.Host -> c.Invoke(f,null,a0,a1,a2,a3,a4,a5,a6)
    | ConstructorModes.User -> 
      let o = Environment.createObject(f.Env)
      o.Prototype <- getPrototype f
      c.Invoke(f,o,a0,a1,a2,a3,a4,a5,a6)

    | _ -> Errors.runtime "Can't call [[Construct]] on non-constructor"

  //----------------------------------------------------------------------------
  static member construct (f:IjsFunc,t:IjsObj,a0:'a0,a1:'a1,a2:'a2,a3:'a3,a4:'a4,a5:'a5,a6:'a6,a7:'a7) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,'a3,'a4,'a5,'a6,'a7,IjsBox>>(f)

    match f.ConstructorMode with
    | ConstructorModes.Host -> c.Invoke(f,null,a0,a1,a2,a3,a4,a5,a6,a7)
    | ConstructorModes.User -> 
      let o = Environment.createObject(f.Env)
      o.Prototype <- getPrototype f
      c.Invoke(f,o,a0,a1,a2,a3,a4,a5,a6,a7)

    | _ -> Errors.runtime "Can't call [[Construct]] on non-constructor"


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
// DispatchTarget
//------------------------------------------------------------------------------
and [<ReferenceEquality>] DispatchTarget = {
  Delegate : HostType
  Function : IjsHostFunc
  Invoke: Dlr.Expr -> Dlr.Expr seq -> Dlr.Expr
}

//------------------------------------------------------------------------------
// HostFunction API
//------------------------------------------------------------------------------
and HostFunction() =

  //----------------------------------------------------------------------------
  static let marshalArgs (passedArgs:Dlr.ExprParam array) (env:Dlr.Expr) i t =
    if i < passedArgs.Length 
      then TypeConverter.convertTo env passedArgs.[i] t
      else Dlr.default' t
      
  //----------------------------------------------------------------------------
  static let marshalBoxParams 
    (f:IjsHostFunc) (passed:Dlr.ExprParam array) (marshalled:Dlr.Expr seq) =
    passed
    |> Seq.skip f.ArgTypes.Length
    |> Seq.map Expr.boxValue
    |> fun x -> Seq.append marshalled [Dlr.newArrayItemsT<IjsBox> x]
    
  //----------------------------------------------------------------------------
  static let marshalObjectParams 
    (f:IjsHostFunc) (passed:Dlr.ExprParam array) (marshalled:Dlr.Expr seq) =
    passed
    |> Seq.skip f.ArgTypes.Length
    |> Seq.map TypeConverter.toHostObject
    |> fun x -> Seq.append marshalled [Dlr.newArrayItemsT<HostObject> x]
    
  //----------------------------------------------------------------------------
  static let createParam i t = Dlr.param (sprintf "a%i" i) t
  
  //----------------------------------------------------------------------------
  static member compileDispatcher (target:DispatchTarget) = 
    let f = target.Function

    let argTypes = Reflection.getDelegateArgTypes target.Delegate
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

    

//------------------------------------------------------------------------------
// DelegateFunction API
//------------------------------------------------------------------------------
and DelegateFunction<'a when 'a :> Delegate>() =

  //----------------------------------------------------------------------------
  static let generateInvoke f args =
    let casted = Dlr.castT<IjsDelFunc<'a>> f
    Dlr.invoke (Dlr.field casted "Delegate") args
    
  //----------------------------------------------------------------------------
  static member create (env:IjsEnv, delegate':'a) =
    let h = IjsDelFunc<'a>(env, delegate') :> IjsHostFunc
    let f = h :> IjsFunc

    Object.putLength(f, double h.jsArgsLength) |> ignore
    Environment.addCompiler(env, f, DelegateFunction<'a>.compile)

    f
  
  //----------------------------------------------------------------------------
  static member compile (x:IjsFunc) (delegate':System.Type) =
    HostFunction.compileDispatcher {
      Delegate = delegate'
      Function = x :?> IjsHostFunc
      Invoke = generateInvoke
    }

    

//------------------------------------------------------------------------------
// ClrFunction API
//------------------------------------------------------------------------------
and ClrFunction() =
  
  //----------------------------------------------------------------------------
  static let generateInvoke (x:IjsClrFunc) _ (args:Dlr.Expr seq) =
    Dlr.Expr.Call(null, x.Method, args) :> Dlr.Expr

  //----------------------------------------------------------------------------
  static member compile (x:IjsFunc) (delegate':System.Type) =
    HostFunction.compileDispatcher {
      Delegate = delegate'
      Function = x :?> IjsHostFunc
      Invoke = generateInvoke (x :?> IjsClrFunc)
    }

  //----------------------------------------------------------------------------
  static member create (env:IjsEnv, method') =
    let h = IjsClrFunc(env, method') :> IjsHostFunc
    let f = h :> IjsFunc
    
    Object.putLength(f, double h.jsArgsLength) |> ignore
    Environment.addCompiler(env, f, ClrFunction.compile)

    f

        
//------------------------------------------------------------------------------
// GetPropertyCache API
//------------------------------------------------------------------------------
and GetPropertyCache =
  static member update (x:IronJS.GetPropertyCache, o:IjsObj) =
    match o.PropertyClassId with
    | -1L -> Object.getProperty(o, x.PropertyName)
    | _   -> 
      let mutable h = null
      let mutable i = -1

      if Object.hasProperty(o, x.PropertyName, &h, &i) then
        if Object.ReferenceEquals(o, h) then
          x.PropertyIndex   <- i
          x.PropertyClassId <- h.PropertyClassId
        else
          () //Build prototype crawler

        h.PropertyValues.[i]

      else
        Utils.boxedUndefined
        
//------------------------------------------------------------------------------
// InvokeCache API
//------------------------------------------------------------------------------
and InvokeCache =
  static member update (x:IronJS.InvokeCache<'a>, f:IjsFunc) =
    x.FunctionId <- f.FunctionId
    x.Cached <- f.Compiler.compileAs<'a> f
    x.Cached
      
  static member create (t:HostType) =
    typedefof<InvokeCache<_>>
      .MakeGenericType([|t|])
      .GetConstructor([||])
      .Invoke([||])
      
//------------------------------------------------------------------------------
// Object API
//------------------------------------------------------------------------------
and Object() =

  //----------------------------------------------------------------------------
  static member setPropertyClass (x:IjsObj, pc) =
    x.PropertyClass <- pc
    x.PropertyClassId <- pc.Id

  //----------------------------------------------------------------------------
  // 8.6.2.6 - [[DefaultValue]]
  static member defaultValue (x:IjsObj) : Box =
    match x.Class with
    | Classes.Date -> Object.defaultValue(x, DefaultValue.String)
    | _ -> Object.defaultValue(x, DefaultValue.Number)
    
  //----------------------------------------------------------------------------
  // 8.6.2.6 - [[DefaultValue]]
  static member defaultValue (x:IjsObj, hint:byte) : Box =
    let valueOf = Object.getProperty(x, "valueOf")
    let toString = Object.getProperty(x, "toString")

    match hint with
    | DefaultValue.Number ->
      match valueOf.Type with
      | TypeCodes.Function -> 
        let mutable v = Function.call(valueOf.Func, x)
        if Utils.isPrimitive &v then v
        else
          match toString.Type with
          | TypeCodes.Function ->
            let mutable v = Function.call(toString.Func, x)
            if Utils.isPrimitive &v then v else Errors.runtime "[[TypeError]]"
          | _ -> Errors.runtime "[[TypeError]]"
      | _ -> Errors.runtime "[[TypeError]]"

    | DefaultValue.String ->
      match toString.Type with
      | TypeCodes.Function ->
        let mutable v = Function.call(toString.Func, x)
        if Utils.isPrimitive &v then v
        else 
          match toString.Type with
          | TypeCodes.Function ->
            let mutable v = Function.call(valueOf.Func, x)
            if Utils.isPrimitive &v then v else Errors.runtime "[[TypeError]]"
          | _ -> Errors.runtime "[[TypeError]]"
      | _ -> Errors.runtime "[[TypeError]]"

    | _ -> Errors.runtime "Invalid hint"
    
  //----------------------------------------------------------------------------
  // Methods that deal with property access
  //----------------------------------------------------------------------------

  //----------------------------------------------------------------------------
  //Makes the Object and its PropertyClass dynamic
  static member makeDynamic (x:IjsObj) =
    x.PropertyClass <- PropertyClass.makeDynamic x.PropertyClass
    x.PropertyClassId <- x.PropertyClass.Id

  //----------------------------------------------------------------------------
  //Expands PropertyValue array size
  static member expandPropertyStorage (x:IjsObj) =
    let newPropertyValues = 
      Array.zeroCreate (if x.count = 0 then 2 else x.count*2)

    if x.count > 0 then 
      Array.Copy(x.PropertyValues, newPropertyValues, x.PropertyValues.Length)

    x.PropertyValues <- newPropertyValues


  static member expandPropertyStorage2 (x:IjsObj) =
    let newPropertyValues = 
      Array.zeroCreate (if x.count = 0 then 2 else x.count*2)

    if x.PropertyValues2.Length > 0 then 
      Array.Copy(x.PropertyValues2, newPropertyValues, x.PropertyValues2.Length)

    x.PropertyValues <- newPropertyValues
    

  //----------------------------------------------------------------------------
  //Creates a property index for 'name' if one doesn't exist
  static member createPropertyIndex (x:IjsObj, name:string) =
    let mutable i = -1
    if not (Object.getOwnPropertyIndex(x, name, &i)) then
      x.PropertyClass <- PropertyClass.subClass(x.PropertyClass, name)
      x.PropertyClassId <- x.PropertyClass.Id
      i <- x.PropertyClass.PropertyMap.[name]
      if x.isFull then Object.expandPropertyStorage x
    i
      
  //----------------------------------------------------------------------------
  //Checks for a property, including Prototype chain
  static member hasProperty (x, name, obj:IjsObj byref, index:int byref) =
    obj <- x
    let mutable continue' = true

    while continue' && not (Object.ReferenceEquals(obj, null)) do
      if obj.PropertyClass.PropertyMap.TryGetValue(name, &index)
        then continue'  <- false  //Found
        else obj        <- obj.Prototype //Try next in chain

    not continue'
      
  //----------------------------------------------------------------------------
  //Gets the index for a property named 'name'
  static member getOwnPropertyIndex (x:IjsObj, name:string, out:int byref) =
    x.PropertyClass.PropertyMap.TryGetValue(name, &out)
      
  //----------------------------------------------------------------------------
  //Gets all property names for the current object
  static member getOwnPropertyNames (x:IjsObj) =
    seq {for x in x.PropertyClass.PropertyMap.Keys -> x}
    
  //----------------------------------------------------------------------------
  static member getOwnPropertyAttributes (x:IjsObj, i:int) =
    if not (Utils.refEquals x.PropertyAttributes null) 
     && i < x.PropertyAttributes.Length then 
      x.PropertyAttributes.[i]
    else
      0s
      
  //----------------------------------------------------------------------------
  //Checks for a property, including Prototype chain
  static member hasProperty (x:IjsObj, name:string) : IjsBool =
    let mutable o = null
    let mutable i = -1
    Object.hasProperty(x, name, &o, &i)
      
  //----------------------------------------------------------------------------
  //Gets a property value, including Prototype chain
  static member getProperty (x:IjsObj, name:string) : IjsBox =
    let mutable h = null
    let mutable i = -1
    if Object.hasProperty(x, name, &h, &i) 
      then h.PropertyValues.[i]
      else Utils.boxedUndefined
      
  //----------------------------------------------------------------------------
  //Deletes a property on the object, making it dynamic in the process
  static member deleteOwnProperty (x:IjsObj, name:string) =
    let mutable i = -1
    if Object.getOwnPropertyIndex(x, name, &i) then
      let attrs = Object.getOwnPropertyAttributes(x, i)
      if attrs &&& PropertyAttrs.DontDelete = 0s then
        x.PropertyClass <- PropertyClass.delete(x.PropertyClass, name)
        x.PropertyClassId <- x.PropertyClass.Id

        x.PropertyValues.[i].Clr <- null
        x.PropertyValues.[i].Type <- TypeCodes.Empty
        x.PropertyValues.[i].Double <- 0.0
          
        true //We managed to delete the property

      else false
    else true
    
  //----------------------------------------------------------------------------
  // Special method that updates an arrays length property if needed
  static member updateLength (x:IjsObj, value:IjsNum) =
    if x.Class = Classes.Array then
      let newLength = TypeConverter.toUInt32 value

      if (double newLength) = value then
        if Utils.isDense x then
          while x.IndexLength > newLength do
            Object.deleteIndex(x, x.IndexLength - 1u) |> ignore
            x.IndexLength <- x.IndexLength - 1u

        else
          for k in List.ofSeq (x.IndexSparse.Keys) do
            if k >= newLength then 
              x.IndexSparse.Remove k |> ignore
          x.IndexLength <- newLength

      else
        failwith "RangeError"

  //----------------------------------------------------------------------------
  static member putProperty(x:IjsObj, n:string, value:obj, attrs:int16) =
    let i = Object.createPropertyIndex(x, n)
    let tc = Utils.obj2tc value

    match tc with
    | TypeCodes.Bool -> 
      x.PropertyValues.[i].Bool <- unbox value
      x.PropertyValues.[i].Type <- tc

    | TypeCodes.Number -> 
      x.PropertyValues.[i].Double <- unbox value

    | _ -> 
      x.PropertyValues.[i].Clr <- value
      x.PropertyValues.[i].Type <- tc
      
  //-------------------------------------------------------------------------
  //Expands IndexValues array size
  static member expandIndexStorage (x:IjsObj, index) =
    if x.IndexValues = null || x.IndexValues.Length <= index then
      let size = if index >= 1073741823 then 2147483647 else ((index+1) * 2)
      let newIndexValues = Array.zeroCreate size
      
      if x.IndexValues <> null && x.IndexValues.Length > 0 then
        Array.Copy(x.IndexValues, newIndexValues, x.IndexValues.Length)

      x.IndexValues <- newIndexValues
        
  //-------------------------------------------------------------------------
  //Changes the index storage to be more efficient for sparse indexes
  static member initSparse (x:IjsObj) =
    if Utils.isDense x then
      x.IndexSparse <- new MutableSorted<uint32, Box>()

      for i = 0 to (int (x.IndexLength-1u)) do
        if x.IndexValues.[i].Type <> TypeCodes.Empty then
          x.IndexSparse.Add(uint32 i, x.IndexValues.[i])

      x.IndexValues <- null

  //----------------------------------------------------------------------------
  // Box get/has/delete index
  //----------------------------------------------------------------------------
  static member getIndex (x:IjsObj, index:Box byref) =
    match index.Type with
    | TypeCodes.Number -> Object.getIndex(x, index.Double)
    | TypeCodes.String -> Object.getIndex(x, index.String)
    | TypeCodes.Bool -> Object.getProperty(x, TypeConverter.toString index.Bool)
    | TypeCodes.Clr -> Object.getProperty(x, TypeConverter.toString index.Clr)
    | TypeCodes.Object
    | TypeCodes.Function ->
      let mutable v = Object.defaultValue(index.Object)
      Object.getIndex(x, &v)

    | _ -> failwith "Que?"

  static member hasIndex (x:IjsObj, index:Box byref) =
    match index.Type with
    | TypeCodes.Number -> Object.hasIndex(x, index.Double)
    | TypeCodes.String -> Object.hasIndex(x, index.String)
    | TypeCodes.Bool -> Object.hasProperty(x, TypeConverter.toString index.Bool)
    | TypeCodes.Clr -> Object.hasProperty(x, TypeConverter.toString index.Clr)
    | TypeCodes.Object
    | TypeCodes.Function ->
      let mutable v = Object.defaultValue(index.Object)
      Object.hasIndex(x, &v)

    | _ -> failwith "Que?"

  static member deleteIndex (x:IjsObj, index:Box byref) =
    match index.Type with
    | TypeCodes.Number -> Object.deleteIndex(x, index.Double)
    | TypeCodes.String -> Object.deleteIndex(x, index.String)
    | TypeCodes.Object
    | TypeCodes.Function ->
      let mutable v = Object.defaultValue(index.Object)
      Object.deleteIndex(x, &v)

    | TypeCodes.Bool ->
      Object.deleteOwnProperty(x, TypeConverter.toString index.Bool)

    | TypeCodes.Clr -> 
      Object.deleteOwnProperty(x, TypeConverter.toString index.Clr)

    | _ -> failwith "Que?"
      
  //----------------------------------------------------------------------------
  // IjsStr get/has/delete index
  //----------------------------------------------------------------------------
  static member getIndex (x:IjsObj, index:IjsStr) : IjsBox = 
    let mutable i = Index.Min
    if Utils.isStringIndex(index, &i) 
      then Object.getIndex(x, i)
      else Object.getProperty(x, index)

  static member hasIndex (x:IjsObj, index:IjsStr) : IjsBool = 
    let mutable i = Index.Min
    if Utils.isStringIndex(index, &i) 
      then Object.hasIndex(x, i)
      else Object.hasProperty(x, index)

  static member deleteIndex (x:IjsObj, index:IjsStr) : IjsBool =
    let mutable i = Index.Min
    if Utils.isStringIndex(index, &i)
      then Object.deleteIndex(x, i)
      else Object.deleteOwnProperty(x, index)
        
  //----------------------------------------------------------------------------
  // IjsNum get/has/delete index
  //----------------------------------------------------------------------------
  static member getIndex (x:IjsObj, index:IjsNum) : IjsBox = 
    let i = uint32 index
    if double i = index
      then Object.getIndex(x, i)
      else Object.getProperty(x, TypeConverter.toString index)

  static member hasIndex (x:IjsObj, index:IjsNum) : IjsBool = 
    let i = uint32 index
    if double i = index
      then Object.hasIndex(x, i)
      else Object.hasProperty(x, TypeConverter.toString index)

  static member deleteIndex (x:IjsObj, index:IjsNum) : IjsBool =
    let i = uint32 index
    if double i = index
      then Object.deleteIndex(x, i)
      else Object.deleteOwnProperty(x, TypeConverter.toString index)
      
  //----------------------------------------------------------------------------
  // UInt32 get/has/delete index
  //----------------------------------------------------------------------------
  static member getIndex (x:IjsObj, ui:uint32) = 
    let mutable o = null
    if Object.hasIndex(x, &o, ui) then
      if Object.ReferenceEquals(o.IndexSparse, null)
        then o.IndexValues.[int ui]
        else o.IndexSparse.[ui]
    else
      Utils.boxedUndefined

  static member hasIndex (x:IjsObj, i:uint32) =
    let mutable o = null
    Object.hasIndex(x, &o, i)

  static member hasIndex (x:IjsObj, o:IjsObj byref, ui:uint32) =
    o <- x
      
    let i = int ui
    let mutable continue' = true

    while continue' && not (Object.ReferenceEquals(o, null)) do
      if ui >= o.IndexLength then
        o <- o.Prototype

      elif not (Object.ReferenceEquals(o.IndexValues, null)) 
          && o.IndexValues.[i].Type <> TypeCodes.Empty then

        continue' <- false

      elif not (Object.ReferenceEquals(o.IndexSparse, null)) 
        && o.IndexSparse.ContainsKey ui then

        continue' <- false

      else 
        o <- o.Prototype

    not continue'

  static member deleteIndex (x:IjsObj, ui:uint32) : bool =
    if ui < x.IndexLength then 
      if not(Object.ReferenceEquals(x.IndexSparse, null)) then
        x.IndexSparse.Remove(ui) |> ignore
      else
        let i = int ui
        x.IndexValues.[i].Clr <- null
        x.IndexValues.[i].Type <- TypeCodes.Empty
      true
    else
      false
    
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  // GENERATED OBJECT METHODS
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------

  //----------------------------------------------------------------------------
  static member putProperty (x:IjsObj, name:IjsStr, value:IjsBox byref) =
    let index = Object.createPropertyIndex(x, name)
    x.PropertyValues.[index] <- value
    value

  //----------------------------------------------------------------------------
  static member putProperty (x:IjsObj, name:IjsStr, value:IjsBool) =
    let index = Object.createPropertyIndex(x, name)
    Utils.setIjsBoolInArray x.PropertyValues index value
    value

  //----------------------------------------------------------------------------
  static member putProperty (x:IjsObj, name:IjsStr, value:IjsNum) =
    let index = Object.createPropertyIndex(x, name)
    x.PropertyValues.[index].Double <- value
    x.PropertyValues.[index].Clr <- null
    value

  //----------------------------------------------------------------------------
  static member putProperty (x:IjsObj, name:IjsStr, value:HostObject) =
    let index = Object.createPropertyIndex(x, name)
    Utils.setHostObjectInArray x.PropertyValues index value
    value

  //----------------------------------------------------------------------------
  static member putProperty (x:IjsObj, name:IjsStr, value:Undefined) =
    let index = Object.createPropertyIndex(x, name)
    Utils.setUndefinedInArray x.PropertyValues index value
    value

  //----------------------------------------------------------------------------
  static member putProperty (x:IjsObj, name:IjsStr, value:IjsStr) =
    let index = Object.createPropertyIndex(x, name)
    Utils.setIjsStrInArray x.PropertyValues index value
    value

  //----------------------------------------------------------------------------
  static member putProperty (x:IjsObj, name:IjsStr, value:IjsObj) =
    let index = Object.createPropertyIndex(x, name)
    Utils.setIjsObjInArray x.PropertyValues index value
    value

  //----------------------------------------------------------------------------
  static member putProperty (x:IjsObj, name:IjsStr, value:IjsFunc) =
    let index = Object.createPropertyIndex(x, name)
    Utils.setIjsFuncInArray x.PropertyValues index value
    value

  //----------------------------------------------------------------------------
  static member putLength (x:IjsObj, value:IjsBox byref) : IjsBox =
    Object.updateLength(x, TypeConverter.toNumber &value)
    Object.putProperty(x, "length", &value)

  //----------------------------------------------------------------------------
  static member putLength (x:IjsObj, value:IjsBool) : IjsBool =
    Object.updateLength(x, TypeConverter.toNumber value)
    Object.putProperty(x, "length", value)

  //----------------------------------------------------------------------------
  static member putLength (x:IjsObj, value:IjsNum) : IjsNum =
    Object.updateLength(x, TypeConverter.toNumber value)
    Object.putProperty(x, "length", value)

  //----------------------------------------------------------------------------
  static member putLength (x:IjsObj, value:HostObject) : HostObject =
    Object.updateLength(x, TypeConverter.toNumber value)
    Object.putProperty(x, "length", value)

  //----------------------------------------------------------------------------
  static member putLength (x:IjsObj, value:Undefined) : Undefined =
    Object.updateLength(x, TypeConverter.toNumber value)
    Object.putProperty(x, "length", value)

  //----------------------------------------------------------------------------
  static member putLength (x:IjsObj, value:IjsStr) : IjsStr =
    Object.updateLength(x, TypeConverter.toNumber value)
    Object.putProperty(x, "length", value)

  //----------------------------------------------------------------------------
  static member putLength (x:IjsObj, value:IjsObj) : IjsObj =
    Object.updateLength(x, TypeConverter.toNumber value)
    Object.putProperty(x, "length", value)

  //----------------------------------------------------------------------------
  static member putLength (x:IjsObj, value:IjsFunc) : IjsFunc =
    Object.updateLength(x, TypeConverter.toNumber value)
    Object.putProperty(x, "length", value)

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, index:Box byref, value:IjsBox byref) : IjsBox = 
    match index.Type with
    | TypeCodes.Number -> Object.putIndex(x, index.Double, &value)
    | TypeCodes.String -> Object.putIndex(x, index.String, &value)
    | TypeCodes.Undefined -> Object.putProperty(x, "undefined", &value)
    | TypeCodes.Object
    | TypeCodes.Function ->
      let mutable v = Object.defaultValue(index.Object)
      Object.putIndex(x, &v, &value)

    | TypeCodes.Bool ->
      Object.putProperty(x, TypeConverter.toString index.Bool, &value)

    | TypeCodes.Clr -> 
      Object.putProperty(x, TypeConverter.toString index.Clr, &value)

    | _ -> failwith "Que?"

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, index:Box byref, value:IjsBool) : IjsBool = 
    match index.Type with
    | TypeCodes.Number -> Object.putIndex(x, index.Double, value)
    | TypeCodes.String -> Object.putIndex(x, index.String, value)
    | TypeCodes.Undefined -> Object.putProperty(x, "undefined", value)
    | TypeCodes.Object
    | TypeCodes.Function ->
      let mutable v = Object.defaultValue(index.Object)
      Object.putIndex(x, &v, value)

    | TypeCodes.Bool ->
      Object.putProperty(x, TypeConverter.toString index.Bool, value)

    | TypeCodes.Clr -> 
      Object.putProperty(x, TypeConverter.toString index.Clr, value)

    | _ -> failwith "Que?"

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, index:Box byref, value:IjsNum) : IjsNum = 
    match index.Type with
    | TypeCodes.Number -> Object.putIndex(x, index.Double, value)
    | TypeCodes.String -> Object.putIndex(x, index.String, value)
    | TypeCodes.Undefined -> Object.putProperty(x, "undefined", value)
    | TypeCodes.Object
    | TypeCodes.Function ->
      let mutable v = Object.defaultValue(index.Object)
      Object.putIndex(x, &v, value)

    | TypeCodes.Bool ->
      Object.putProperty(x, TypeConverter.toString index.Bool, value)

    | TypeCodes.Clr -> 
      Object.putProperty(x, TypeConverter.toString index.Clr, value)

    | _ -> failwith "Que?"

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, index:Box byref, value:HostObject) : HostObject = 
    match index.Type with
    | TypeCodes.Number -> Object.putIndex(x, index.Double, value)
    | TypeCodes.String -> Object.putIndex(x, index.String, value)
    | TypeCodes.Undefined -> Object.putProperty(x, "undefined", value)
    | TypeCodes.Object
    | TypeCodes.Function ->
      let mutable v = Object.defaultValue(index.Object)
      Object.putIndex(x, &v, value)

    | TypeCodes.Bool ->
      Object.putProperty(x, TypeConverter.toString index.Bool, value)

    | TypeCodes.Clr -> 
      Object.putProperty(x, TypeConverter.toString index.Clr, value)

    | _ -> failwith "Que?"

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, index:Box byref, value:Undefined) : Undefined = 
    match index.Type with
    | TypeCodes.Number -> Object.putIndex(x, index.Double, value)
    | TypeCodes.String -> Object.putIndex(x, index.String, value)
    | TypeCodes.Undefined -> Object.putProperty(x, "undefined", value)
    | TypeCodes.Object
    | TypeCodes.Function ->
      let mutable v = Object.defaultValue(index.Object)
      Object.putIndex(x, &v, value)

    | TypeCodes.Bool ->
      Object.putProperty(x, TypeConverter.toString index.Bool, value)

    | TypeCodes.Clr -> 
      Object.putProperty(x, TypeConverter.toString index.Clr, value)

    | _ -> failwith "Que?"

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, index:Box byref, value:IjsStr) : IjsStr = 
    match index.Type with
    | TypeCodes.Number -> Object.putIndex(x, index.Double, value)
    | TypeCodes.String -> Object.putIndex(x, index.String, value)
    | TypeCodes.Undefined -> Object.putProperty(x, "undefined", value)
    | TypeCodes.Object
    | TypeCodes.Function ->
      let mutable v = Object.defaultValue(index.Object)
      Object.putIndex(x, &v, value)

    | TypeCodes.Bool ->
      Object.putProperty(x, TypeConverter.toString index.Bool, value)

    | TypeCodes.Clr -> 
      Object.putProperty(x, TypeConverter.toString index.Clr, value)

    | _ -> failwith "Que?"

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, index:Box byref, value:IjsObj) : IjsObj = 
    match index.Type with
    | TypeCodes.Number -> Object.putIndex(x, index.Double, value)
    | TypeCodes.String -> Object.putIndex(x, index.String, value)
    | TypeCodes.Undefined -> Object.putProperty(x, "undefined", value)
    | TypeCodes.Object
    | TypeCodes.Function ->
      let mutable v = Object.defaultValue(index.Object)
      Object.putIndex(x, &v, value)

    | TypeCodes.Bool ->
      Object.putProperty(x, TypeConverter.toString index.Bool, value)

    | TypeCodes.Clr -> 
      Object.putProperty(x, TypeConverter.toString index.Clr, value)

    | _ -> failwith "Que?"

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, index:Box byref, value:IjsFunc) : IjsFunc = 
    match index.Type with
    | TypeCodes.Number -> Object.putIndex(x, index.Double, value)
    | TypeCodes.String -> Object.putIndex(x, index.String, value)
    | TypeCodes.Undefined -> Object.putProperty(x, "undefined", value)
    | TypeCodes.Object
    | TypeCodes.Function ->
      let mutable v = Object.defaultValue(index.Object)
      Object.putIndex(x, &v, value)

    | TypeCodes.Bool ->
      Object.putProperty(x, TypeConverter.toString index.Bool, value)

    | TypeCodes.Clr -> 
      Object.putProperty(x, TypeConverter.toString index.Clr, value)

    | _ -> failwith "Que?"

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, index:IjsStr, value:IjsBox byref) : IjsBox = 
    let mutable i = Index.Min
    if Utils.isStringIndex(index, &i) 
      then Object.putIndex(x, i, &value)
      else 
        if x.Class=Classes.Array && index="length" 
          then Object.putLength(x, &value)
          else Object.putProperty(x, index, &value)

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, index:IjsStr, value:IjsBool) : IjsBool = 
    let mutable i = Index.Min
    if Utils.isStringIndex(index, &i) 
      then Object.putIndex(x, i, value)
      else 
        if x.Class=Classes.Array && index="length" 
          then Object.putLength(x, value)
          else Object.putProperty(x, index, value)

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, index:IjsStr, value:IjsNum) : IjsNum = 
    let mutable i = Index.Min
    if Utils.isStringIndex(index, &i) 
      then Object.putIndex(x, i, value)
      else 
        if x.Class=Classes.Array && index="length" 
          then Object.putLength(x, value)
          else Object.putProperty(x, index, value)

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, index:IjsStr, value:HostObject) : HostObject = 
    let mutable i = Index.Min
    if Utils.isStringIndex(index, &i) 
      then Object.putIndex(x, i, value)
      else 
        if x.Class=Classes.Array && index="length" 
          then Object.putLength(x, value)
          else Object.putProperty(x, index, value)

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, index:IjsStr, value:Undefined) : Undefined = 
    let mutable i = Index.Min
    if Utils.isStringIndex(index, &i) 
      then Object.putIndex(x, i, value)
      else 
        if x.Class=Classes.Array && index="length" 
          then Object.putLength(x, value)
          else Object.putProperty(x, index, value)

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, index:IjsStr, value:IjsStr) : IjsStr = 
    let mutable i = Index.Min
    if Utils.isStringIndex(index, &i) 
      then Object.putIndex(x, i, value)
      else 
        if x.Class=Classes.Array && index="length" 
          then Object.putLength(x, value)
          else Object.putProperty(x, index, value)

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, index:IjsStr, value:IjsObj) : IjsObj = 
    let mutable i = Index.Min
    if Utils.isStringIndex(index, &i) 
      then Object.putIndex(x, i, value)
      else 
        if x.Class=Classes.Array && index="length" 
          then Object.putLength(x, value)
          else Object.putProperty(x, index, value)

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, index:IjsStr, value:IjsFunc) : IjsFunc = 
    let mutable i = Index.Min
    if Utils.isStringIndex(index, &i) 
      then Object.putIndex(x, i, value)
      else 
        if x.Class=Classes.Array && index="length" 
          then Object.putLength(x, value)
          else Object.putProperty(x, index, value)

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, index:IjsNum, value:IjsBox byref) : IjsBox = 
    let i = uint32 index
    if double i = index
      then Object.putIndex(x, i, &value)
      else Object.putProperty(x, TypeConverter.toString index, &value)

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, index:IjsNum, value:IjsBool) : IjsBool = 
    let i = uint32 index
    if double i = index
      then Object.putIndex(x, i, value)
      else Object.putProperty(x, TypeConverter.toString index, value)

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, index:IjsNum, value:IjsNum) : IjsNum = 
    let i = uint32 index
    if double i = index
      then Object.putIndex(x, i, value)
      else Object.putProperty(x, TypeConverter.toString index, value)

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, index:IjsNum, value:HostObject) : HostObject = 
    let i = uint32 index
    if double i = index
      then Object.putIndex(x, i, value)
      else Object.putProperty(x, TypeConverter.toString index, value)

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, index:IjsNum, value:Undefined) : Undefined = 
    let i = uint32 index
    if double i = index
      then Object.putIndex(x, i, value)
      else Object.putProperty(x, TypeConverter.toString index, value)

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, index:IjsNum, value:IjsStr) : IjsStr = 
    let i = uint32 index
    if double i = index
      then Object.putIndex(x, i, value)
      else Object.putProperty(x, TypeConverter.toString index, value)

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, index:IjsNum, value:IjsObj) : IjsObj = 
    let i = uint32 index
    if double i = index
      then Object.putIndex(x, i, value)
      else Object.putProperty(x, TypeConverter.toString index, value)

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, index:IjsNum, value:IjsFunc) : IjsFunc = 
    let i = uint32 index
    if double i = index
      then Object.putIndex(x, i, value)
      else Object.putProperty(x, TypeConverter.toString index, value)

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, ui:uint32, value:IjsBox byref) : IjsBox =
    if ui > Index.Max then Object.initSparse x
    if Utils.isDense x then
      if ui < uint32 x.IndexValues.Length then
        x.IndexValues.[(int ui)] <- value
      else
        if ui > 255u && ui/2u > x.IndexLength then
          Object.initSparse x
          x.IndexSparse.[ui] <- value
        else
          Object.expandIndexStorage(x, int ui)
          x.IndexValues.[(int ui)] <- value
    else
      x.IndexSparse.[ui] <- value

    if ui > x.IndexLength then
      x.IndexLength <- ui + 1u
      Object.putLength(x, double (ui + 1u)) |> ignore

    value

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, ui:uint32, value:IjsBool) : IjsBool =
    if ui > Index.Max then Object.initSparse x
    if Utils.isDense x then
      if ui < uint32 x.IndexValues.Length then
        Utils.setIjsBoolInArray x.IndexValues (int ui) value
      else
        if ui > 255u && ui/2u > x.IndexLength then
          Object.initSparse x
          x.IndexSparse.[ui] <- Utils.boxIjsBool value
        else
          Object.expandIndexStorage(x, int ui)
          Utils.setIjsBoolInArray x.IndexValues (int ui) value
    else
      x.IndexSparse.[ui] <- Utils.boxIjsBool value

    if ui > x.IndexLength then
      x.IndexLength <- ui + 1u
      Object.putLength(x, double (ui + 1u)) |> ignore

    value

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, ui:uint32, value:IjsNum) : IjsNum =
    if ui > Index.Max then Object.initSparse x
    if Utils.isDense x then
      if ui < uint32 x.IndexValues.Length then
        Utils.setIjsNumInArray x.IndexValues (int ui) value
      else
        if ui > 255u && ui/2u > x.IndexLength then
          Object.initSparse x
          x.IndexSparse.[ui] <- Utils.boxIjsNum value
        else
          Object.expandIndexStorage(x, int ui)
          Utils.setIjsNumInArray x.IndexValues (int ui) value
    else
      x.IndexSparse.[ui] <- Utils.boxIjsNum value

    if ui > x.IndexLength then
      x.IndexLength <- ui + 1u
      Object.putLength(x, double (ui + 1u)) |> ignore

    value

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, ui:uint32, value:HostObject) : HostObject =
    if ui > Index.Max then Object.initSparse x
    if Utils.isDense x then
      if ui < uint32 x.IndexValues.Length then
        Utils.setHostObjectInArray x.IndexValues (int ui) value
      else
        if ui > 255u && ui/2u > x.IndexLength then
          Object.initSparse x
          x.IndexSparse.[ui] <- Utils.boxHostObject value
        else
          Object.expandIndexStorage(x, int ui)
          Utils.setHostObjectInArray x.IndexValues (int ui) value
    else
      x.IndexSparse.[ui] <- Utils.boxHostObject value

    if ui > x.IndexLength then
      x.IndexLength <- ui + 1u
      Object.putLength(x, double (ui + 1u)) |> ignore

    value

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, ui:uint32, value:Undefined) : Undefined =
    if ui > Index.Max then Object.initSparse x
    if Utils.isDense x then
      if ui < uint32 x.IndexValues.Length then
        Utils.setUndefinedInArray x.IndexValues (int ui) value
      else
        if ui > 255u && ui/2u > x.IndexLength then
          Object.initSparse x
          x.IndexSparse.[ui] <- Utils.boxUndefined value
        else
          Object.expandIndexStorage(x, int ui)
          Utils.setUndefinedInArray x.IndexValues (int ui) value
    else
      x.IndexSparse.[ui] <- Utils.boxUndefined value

    if ui > x.IndexLength then
      x.IndexLength <- ui + 1u
      Object.putLength(x, double (ui + 1u)) |> ignore

    value

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, ui:uint32, value:IjsStr) : IjsStr =
    if ui > Index.Max then Object.initSparse x
    if Utils.isDense x then
      if ui < uint32 x.IndexValues.Length then
        Utils.setIjsStrInArray x.IndexValues (int ui) value
      else
        if ui > 255u && ui/2u > x.IndexLength then
          Object.initSparse x
          x.IndexSparse.[ui] <- Utils.boxIjsStr value
        else
          Object.expandIndexStorage(x, int ui)
          Utils.setIjsStrInArray x.IndexValues (int ui) value
    else
      x.IndexSparse.[ui] <- Utils.boxIjsStr value

    if ui > x.IndexLength then
      x.IndexLength <- ui + 1u
      Object.putLength(x, double (ui + 1u)) |> ignore

    value

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, ui:uint32, value:IjsObj) : IjsObj =
    if ui > Index.Max then Object.initSparse x
    if Utils.isDense x then
      if ui < uint32 x.IndexValues.Length then
        Utils.setIjsObjInArray x.IndexValues (int ui) value
      else
        if ui > 255u && ui/2u > x.IndexLength then
          Object.initSparse x
          x.IndexSparse.[ui] <- Utils.boxIjsObj value
        else
          Object.expandIndexStorage(x, int ui)
          Utils.setIjsObjInArray x.IndexValues (int ui) value
    else
      x.IndexSparse.[ui] <- Utils.boxIjsObj value

    if ui > x.IndexLength then
      x.IndexLength <- ui + 1u
      Object.putLength(x, double (ui + 1u)) |> ignore

    value

  //----------------------------------------------------------------------------
  static member putIndex (x:IjsObj, ui:uint32, value:IjsFunc) : IjsFunc =
    if ui > Index.Max then Object.initSparse x
    if Utils.isDense x then
      if ui < uint32 x.IndexValues.Length then
        Utils.setIjsFuncInArray x.IndexValues (int ui) value
      else
        if ui > 255u && ui/2u > x.IndexLength then
          Object.initSparse x
          x.IndexSparse.[ui] <- Utils.boxIjsFunc value
        else
          Object.expandIndexStorage(x, int ui)
          Utils.setIjsFuncInArray x.IndexValues (int ui) value
    else
      x.IndexSparse.[ui] <- Utils.boxIjsFunc value

    if ui > x.IndexLength then
      x.IndexLength <- ui + 1u
      Object.putLength(x, double (ui + 1u)) |> ignore

    value
    
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  // GENERATED OBJECT METHODS
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------

module ObjectModule =

  module Property = 

    let requiredStorage (o:IjsObj) =
      o.PropertyClass.PropertyMap.Count

    let isFull (o:IjsObj) =
      requiredStorage o >= o.PropertyValues2.Length

    let getIndex (o:IjsObj) (name:string) =
      o.PropertyClass.PropertyMap.TryGetValue name

    let setMap (o:IjsObj, pc) =
      o.PropertyClass <- pc
      o.PropertyClassId <- pc.Id

    let expandStorage (o:IjsObj) =
      let values = o.PropertyValues2
      let required = requiredStorage o
      let newValues = Array.zeroCreate (required * 2)

      if values.Length > 0 then 
        Array.Copy(values, newValues, values.Length)
      
      o.PropertyValues2 <- newValues

    let ensureIndex (o:IjsObj) (name:string) =
      match getIndex o name with
      | true, index -> index
      | _ -> 
        o.PropertyClass <- PropertyClass.subClass(o.PropertyClass, name)
        o.PropertyClassId <- o.PropertyClass.Id
        if isFull o then expandStorage o
        o.PropertyClass.PropertyMap.[name]

    let find (o:IjsObj) (name) =
      let rec find o name =
        if Utils.Utils.isNull o then (-1, null)
        else
          match getIndex o name with
          | true, index ->  index, o
          | _ -> find o.Prototype name

      find o name

    let inline putBox (o:IjsObj) (name:IjsStr) (val':IjsBox) =
      let index = ensureIndex o name
      o.PropertyValues2.[index].Box <- val'
      o.PropertyValues2.[index].HasValue <- true

    let inline putRef (o:IjsObj) (name:IjsStr) (val':HostObject) (tc:TypeCode) =
      let index = ensureIndex o name
      o.PropertyValues2.[index].Box.Clr <- val'
      o.PropertyValues2.[index].Box.Type <- tc

    let inline putVal (o:IjsObj) (name:IjsStr) (val':IjsNum) =
      let index = ensureIndex o name
      o.PropertyValues2.[index].Box.Double <- val'
      o.PropertyValues2.[index].HasValue <- true
      
    let inline fst (f, _) = f
    let inline snd (_, s) = s

    let inline get (o:IjsObj) (name:IjsStr) =
      match find o name with
      | -1, _ -> Utils.boxedUndefined
      | pair -> (snd pair).PropertyValues2.[fst pair].Box

    let has (o:IjsObj) (name:IjsStr) =
      match find o name with
      | -1, _ -> false
      | _ -> true

    let delete (o:IjsObj) (name:IjsStr) =
      match getIndex o name with
      | true, index -> 
        let attrs = o.PropertyValues2.[index].Attributes
        let canDelete = Utils.Descriptor.isDeletable attrs

        if canDelete then
          o.PropertyValues2.[index].HasValue <- false
          o.PropertyValues2.[index].Box.Clr <- null
          o.PropertyValues2.[index].Box.Double <- 0.0

        canDelete

      | _ -> true
      
    let setOnInternal (im:InternalMethods) =
      {im with
        GetProperty = new GetProperty(get)
        HasProperty = new HasProperty(has)
        DeleteProperty = new DeleteProperty(delete)
        PutBoxProperty = new PutBoxProperty(putBox)
        PutRefProperty = new PutRefProperty(putRef)
        PutValProperty = new PutValProperty(putVal)
      }
    (*
  //----------------------------------------------------------------------------
  //Checks for a property, including Prototype chain
  static member hasProperty (x, name, obj:IjsObj byref, index:int byref) =
    obj <- x
    let mutable continue' = true

    while continue' && not (Object.ReferenceEquals(obj, null)) do
      if obj.PropertyClass.PropertyMap.TryGetValue(name, &index)
        then continue'  <- false  //Found
        else obj        <- obj.Prototype //Try next in chain

    not continue'
      
  //----------------------------------------------------------------------------
  //Gets the index for a property named 'name'
  static member getOwnPropertyIndex (x:IjsObj, name:string, out:int byref) =
    x.PropertyClass.PropertyMap.TryGetValue(name, &out)
      
  //----------------------------------------------------------------------------
  //Gets all property names for the current object
  static member getOwnPropertyNames (x:IjsObj) =
    seq {for x in x.PropertyClass.PropertyMap.Keys -> x}
    
  //----------------------------------------------------------------------------
  static member getOwnPropertyAttributes (x:IjsObj, i:int) =
    if not (Utils.refEquals x.PropertyAttributes null) 
     && i < x.PropertyAttributes.Length then 
      x.PropertyAttributes.[i]
    else
      0s
      
  //----------------------------------------------------------------------------
  //Checks for a property, including Prototype chain
  static member hasProperty (x:IjsObj, name:string) : IjsBool =
    let mutable o = null
    let mutable i = -1
    Object.hasProperty(x, name, &o, &i)
      
  //----------------------------------------------------------------------------
  //Gets a property value, including Prototype chain
  static member getProperty (x:IjsObj, name:string) : IjsBox =
    let mutable h = null
    let mutable i = -1
    if Object.hasProperty(x, name, &h, &i) 
      then h.PropertyValues.[i]
      else Utils.boxedUndefined
      
  //----------------------------------------------------------------------------
  //Deletes a property on the object, making it dynamic in the process
  static member deleteOwnProperty (x:IjsObj, name:string) =
    let mutable i = -1
    if Object.getOwnPropertyIndex(x, name, &i) then
      let attrs = Object.getOwnPropertyAttributes(x, i)
      if attrs &&& PropertyAttrs.DontDelete = 0s then
        x.PropertyClass <- PropertyClass.delete(x.PropertyClass, name)
        x.PropertyClassId <- x.PropertyClass.Id

        x.PropertyValues.[i].Clr <- null
        x.PropertyValues.[i].Type <- TypeCodes.Empty
        x.PropertyValues.[i].Double <- 0.0
          
        true //We managed to delete the property

      else false
    else true*)