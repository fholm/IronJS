namespace IronJS

//Disables warning on Box struct for overlaying
//several reference type fields with eachother.
#nowarn "9"

open IronJS
open IronJS.Runtime
open IronJS.Support.Aliases
open IronJS.Support.CustomOperators

open System
open System.Dynamic
open System.Reflection
open System.Reflection.Emit
open System.Runtime.InteropServices
open System.Globalization
open System.Text.RegularExpressions

type BV = BoxedValue
type Args = BV array
type Desc = Descriptor
type Undef = Undefined
type Env = IronJS.Runtime.Environment
type CO = CommonObject
type VO = ValueObject
type RO = RegExpObject
type DO = DateObject
type AO = ArrayObject

type ArgLink = ParameterStorageType * int
type CompiledCache = MutableDict<Type, Delegate>

/// This delegate type is used for functions that are called
/// with more then four arguments. Instead of compiling a function
/// for each arity above six we pass in an array of BV values 
/// instead and then sort it out inside the function body.
type VariadicFunction = Func<FO, CO, Args, BV>

// We only optimize for aritys that is <= 4, any more then that
// and we'll use the VariadicFunction delegate instead.
type Function = Func<FO, CO, BV>
type Function<'a> = Func<FO, CO, 'a, BV>
type Function<'a, 'b> = Func<FO, CO, 'a, 'b, BV>
type Function<'a, 'b, 'c> = Func<FO, CO, 'a, 'b, 'c, BV>
type Function<'a, 'b, 'c, 'd> = Func<FO, CO, 'a, 'b, 'c, 'd, BV>

type FunctionReturn<'r> = Func<FO, CO, 'r>
type FunctionReturn<'a, 'r> = Func<FO, CO, 'a, 'r>
type FunctionReturn<'a, 'b, 'r> = Func<FO, CO, 'a, 'b, 'r>
type FunctionReturn<'a, 'b, 'c, 'r> = Func<FO, CO, 'a, 'b, 'c, 'r>
type FunctionReturn<'a, 'b, 'c, 'd, 'r> = Func<FO, CO, 'a, 'b, 'c, 'd, 'r>

type HFO<'a when 'a :> Delegate> = HostFunctionObject<'a>
type SO = StringObject
type NO = NumberObject
type BO = BooleanObject
type MO = MathObject
type EO = ErrorObject
type FO = FunctionObject

type TC = TypeConverter

type DlrTC = DlrTypeConverter
and DlrTypeConverter() =
  static member ToBoxedValue(expr:Dlr.Expr) : Dlr.Expr = Dlr.callStaticT<TC> "ToBoxedValue" [expr]

  static member ToClrObject(expr:Dlr.Expr) : Dlr.Expr =  Dlr.callStaticT<TC> "ToClrObject" [expr]

  static member ToObject(env:Dlr.Expr, expr:Dlr.Expr) : Dlr.Expr = Dlr.callStaticT<TC> "ToObject" [env; expr]

  static member ToBoolean(expr:Dlr.Expr) : Dlr.Expr = Dlr.callStaticT<TC> "ToBoolean" [expr]

  static member ToPrimitive(expr:Dlr.Expr) : Dlr.Expr = Dlr.callStaticT<TC> "ToPrimitive" [expr]
  static member ToPrimitiveHintNumber(expr:Dlr.Expr) : Dlr.Expr = Dlr.callStaticT<TC> "ToPrimitive" [expr; Dlr.const' DefaultValueHint.Number]
  static member ToPrimitiveHintString(expr:Dlr.Expr) : Dlr.Expr = Dlr.callStaticT<TC> "ToPrimitive" [expr; Dlr.const' DefaultValueHint.String]

  static member ToString(expr:Dlr.Expr) : Dlr.Expr = Dlr.callStaticT<TC> "ToString" [expr]

  static member ToNumber(expr:Dlr.Expr) : Dlr.Expr = Dlr.callStaticT<TC> "ToNumber" [expr]

  static member ConvertTo (envExpr:Dlr.Expr, expr:Dlr.Expr, t:Type) =
    // If the types are identical just return the expr
    if Object.ReferenceEquals(expr.Type, t) then 
      expr

    // If expr.Type is a subclass of t, cast expr to t
    elif t.IsAssignableFrom(expr.Type) then 
      Dlr.cast t expr

    // Else, apply the javascript type converter
    else 
      if   t = typeof<double> then DlrTC.ToNumber expr
      elif t = typeof<string> then DlrTC.ToString expr
      elif t = typeof<bool> then DlrTC.ToBoolean expr
      elif t = typeof<BV> then DlrTC.ToBoxedValue expr
      elif t = typeof<CO> then DlrTC.ToObject(envExpr, expr)
      elif t = typeof<obj> then DlrTC.ToClrObject expr
      else Error.CompileError.Raise(Error.missingNoConversion expr.Type t)

type ClrArgs = obj array
type Scope = BV array
type DynamicScope = (int * CO) list
