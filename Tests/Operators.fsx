#light
#r "Microsoft.VisualStudio.QualityTools.UnitTestFramework"
#r @"../Lib/Antlr3.Runtime.dll"
#r @"../Lib/CLR4/Microsoft.Dynamic.dll"
#r @"../Lib/CLR4/Xebic.ES3.dll"
#r @"../Src/FSKit/Src/bin/Debug/FSKit.dll"
#r @"../Src/IronJS/bin/Debug/IronJS.dll"

open IronJS
open IronJS.Aliases
open FSKit.Testing.Assert
open Microsoft.VisualStudio.TestTools.UnitTesting

let test, clean, state, report = 
  FSKit.Testing.createTesters (fun () -> IronJS.Hosting.Context.Create())

test "11.3 Postfix Expressions" (fun ctx ->
  equal 0.0 (ctx.ExecuteT<double> "var i = 0; i++;")
  equal 1.0 (ctx.ExecuteT<double> "i")
  equal 1.0 (ctx.ExecuteT<double> "i--")
  equal 0.0 (ctx.ExecuteT<double> "i")
)

test "11.4.1 The delete Operator" (fun ctx ->
  //Property
  ctx.Execute "var o = {foo: 1, bar: 2}" |> ignore
  equal 1.0 (ctx.ExecuteT<double> "o.foo")
  equal 2.0 (ctx.ExecuteT<double> "o.bar")

  ctx.Execute "delete o.foo" |> ignore
  same IronJS.Undefined.Instance (ctx.ExecuteT<Undefined> "o.foo")
  equal 2.0 (ctx.ExecuteT<double> "o.bar")

  //Index
  ctx.Execute("var a = ['foo', 'bar']") |> ignore
  Assert.AreEqual("foo", ctx.ExecuteT<string>("a[0]"))
  Assert.AreEqual("bar", ctx.ExecuteT<string>("a[1]"))

  ctx.Execute("delete a[0]") |> ignore
  Assert.AreEqual(IronJS.Undefined.Instance, ctx.ExecuteT<Undefined>("a[0]"))
  Assert.AreEqual("bar", ctx.ExecuteT<string>("a[1]"))
)

test "11.4.2 The void Operator" (fun ctx ->
  Assert.AreEqual(Undefined.Instance, ctx.ExecuteT<Undefined>("void 1"));
)

test "11.4.3 The typeof Operator" (fun ctx ->
  Assert.AreEqual("undefined", ctx.ExecuteT<string>("typeof undefined"));
  Assert.AreEqual("object", ctx.ExecuteT<string>("typeof null"));
  Assert.AreEqual("boolean", ctx.ExecuteT<string>("typeof true"));
  Assert.AreEqual("string", ctx.ExecuteT<string>("typeof 'foo'"));
  Assert.AreEqual("number", ctx.ExecuteT<string>("typeof 1"));
  Assert.AreEqual("object", ctx.ExecuteT<string>("typeof {}"));
  Assert.AreEqual("function", ctx.ExecuteT<string>("typeof (function(){})"));
)

test "11.4.4 Prefix Increment Operator" (fun ctx ->
  Assert.AreEqual(0.0, ctx.ExecuteT<double>("var i = 0;"));
  Assert.AreEqual(1.0, ctx.ExecuteT<double>("++i"));
  Assert.AreEqual(1.0, ctx.ExecuteT<double>("i"));
)

test "11.4.5 Prefix Decrement Operator" (fun ctx ->
  Assert.AreEqual(1.0, ctx.ExecuteT<double>("var i = 1;"));
  Assert.AreEqual(0.0, ctx.ExecuteT<double>("--i"));
  Assert.AreEqual(0.0, ctx.ExecuteT<double>("i"));
)

test "11.4.6 Unary + Operator" (fun ctx ->
  Assert.AreEqual(1.0, ctx.ExecuteT<double>("+1"));
  Assert.AreEqual(1.0, ctx.ExecuteT<double>("+'1'"));
  Assert.AreEqual(1.0, ctx.ExecuteT<double>("+true"));
  Assert.AreEqual(0.0, ctx.ExecuteT<double>("+false"));
  Assert.AreEqual(NaN, ctx.ExecuteT<double>("+{}"));
)

test "11.4.7 Unary - Operator" (fun ctx ->
  Assert.AreEqual(-1.0, ctx.ExecuteT<double>("-1"));
  Assert.AreEqual(-1.0, ctx.ExecuteT<double>("-'1'"));
  Assert.AreEqual(-1.0, ctx.ExecuteT<double>("-true"));
  Assert.AreEqual(-0.0, ctx.ExecuteT<double>("-false"));

  Assert.AreEqual(1.0, ctx.ExecuteT<double>("-(-1)"));
  Assert.AreEqual(1.0, ctx.ExecuteT<double>("-(-'1')"));
  Assert.AreEqual(1.0, ctx.ExecuteT<double>("-(-true)"));
  Assert.AreEqual(0.0, ctx.ExecuteT<double>("-(-false)"));
)

test "11.4.8 Bitwise NOT Operator ( ~ )" (fun ctx ->
  Assert.AreEqual(double ~~~1, ctx.ExecuteT<double>("~1"));
  Assert.AreEqual(double ~~~(-1), ctx.ExecuteT<double>("~-1"));
)

test "11.4.9 Logical NOT Operator ( ! )" (fun ctx ->
  Assert.AreEqual(false, ctx.ExecuteT<bool>("!true"));
  Assert.AreEqual(true, ctx.ExecuteT<bool>("!false"));
  Assert.AreEqual(false, ctx.ExecuteT<bool>("!1"));
  Assert.AreEqual(true, ctx.ExecuteT<bool>("!0"));
  Assert.AreEqual(false, ctx.ExecuteT<bool>("!'foo'"));
  Assert.AreEqual(true, ctx.ExecuteT<bool>("!''"));
  Assert.AreEqual(false, ctx.ExecuteT<bool>("!{}"));
  Assert.AreEqual(false, ctx.ExecuteT<bool>("!(function(){})"));
  Assert.AreEqual(true, ctx.ExecuteT<bool>("!null"));
  Assert.AreEqual(true, ctx.ExecuteT<bool>("!undefined"));
)

test "11.5.1 Applying the * Operator" (fun ctx ->
  //If either operand is NaN, the result is NaN.
  Assert.AreEqual(1.0 * NaN, ctx.ExecuteT<double>("1 * NaN"));
  Assert.AreEqual(1.0 * NaN, ctx.ExecuteT<double>("NaN * 1"));

  //The sign of the result is positive if both operands have
  //the same sign, negative if the operands have different signs.
  Assert.AreEqual(1.0 * 0.0, ctx.ExecuteT<double>("1 * 0"));
  Assert.AreEqual(-1.0 * 1.0, ctx.ExecuteT<double>("-1 * 1"));
  Assert.AreEqual(-1.0 * -1.0, ctx.ExecuteT<double>("-1 * -1"));
  Assert.AreEqual(1.0 * -1.0, ctx.ExecuteT<double>("1 * -1"));

  //Multiplication of an infinity by a zero results in NaN.
  Assert.AreEqual(PosInf * 0.0, ctx.ExecuteT<double>("Infinity * 0"));

  //Multiplication of an infinity by an infinity results in an infinity. 
  Assert.AreEqual(PosInf * PosInf, ctx.ExecuteT<double>("Infinity * Infinity"));

  //Multiplication of an infinity by a finite non-zero value results in a signed infinity.
  Assert.AreEqual(PosInf * 1.0, ctx.ExecuteT<double>("Infinity * 1"));
  
  //Remaining cases
  Assert.AreEqual(1.0 * 0.0, ctx.ExecuteT<double>("1 * 0"));
  Assert.AreEqual(1.0 * 1.0, ctx.ExecuteT<double>("1 * 1"));
  Assert.AreEqual(1.0 * 2.0, ctx.ExecuteT<double>("1 * 2"));
  Assert.AreEqual(1.0 * 2.0, ctx.ExecuteT<double>("1 * '2'"));
  Assert.AreEqual(1.0 * NaN, ctx.ExecuteT<double>("1 * 'b'"));
  Assert.AreEqual(1.0 * 1.0, ctx.ExecuteT<double>("1 * true"));
  Assert.AreEqual(1.0 * 0.0, ctx.ExecuteT<double>("1 * false"));
)

test "11.5.2 Applying the / Operator" (fun ctx ->
  //If either operand is NaN, the result is NaN.
  Assert.AreEqual(1.0 / NaN, ctx.ExecuteT<double>("1 / NaN"));
  Assert.AreEqual(NaN / 1.0, ctx.ExecuteT<double>("NaN / 1"));
  
  //The sign of the result is positive if both operands have
  //the same sign, negative if the operands have different signs.
  Assert.AreEqual(1.0 / 0.0, ctx.ExecuteT<double>("1 / 0"));
  Assert.AreEqual(-1.0 / 1.0, ctx.ExecuteT<double>("-1 / 1"));
  Assert.AreEqual(-1.0 / -1.0, ctx.ExecuteT<double>("-1 / -1"));
  Assert.AreEqual(1.0 / -1.0, ctx.ExecuteT<double>("1 / -1"));

  //Division of an infinity by an infinity results in NaN.
  Assert.AreEqual(PosInf / PosInf, ctx.ExecuteT<double>("Infinity / Infinity"));

  //Division of an infinity by a zero results in an infinity.
  Assert.AreEqual(PosInf / 0.0, ctx.ExecuteT<double>("Infinity / 0"));

  //Division of an infinity by a non-zero finite value results in a signed infinity.
  Assert.AreEqual(PosInf / 1.0, ctx.ExecuteT<double>("Infinity / 1"));

  //Division of a finite value by an infinity results in zero.
  Assert.AreEqual(1.0 / PosInf, ctx.ExecuteT<double>("1 / Infinity"));

  //Division of a zero by a zero results in NaN
  Assert.AreEqual(0.0 / 0.0, ctx.ExecuteT<double>("0 / 0"));

  //Division of a non-zero finite value by a zero results in a signed infinity.
  Assert.AreEqual(1.0 / 0.0, ctx.ExecuteT<double>("1 / 0"));

  //Remaining cases
  Assert.AreEqual(1.0 / 0.0, ctx.ExecuteT<double>("1 / 0"));
  Assert.AreEqual(1.0 / 1.0, ctx.ExecuteT<double>("1 / 1"));
  Assert.AreEqual(1.0 / 2.0, ctx.ExecuteT<double>("1 / 2"));
  Assert.AreEqual(1.0 / 2.0, ctx.ExecuteT<double>("1 / '2'"));
  Assert.AreEqual(1.0 / NaN, ctx.ExecuteT<double>("1 / 'b'"));
  Assert.AreEqual(1.0 / 1.0, ctx.ExecuteT<double>("1 / true"));
  Assert.AreEqual(1.0 / 0.0, ctx.ExecuteT<double>("1 / false"));
)

test "11.5.3 Applying the % Operator" (fun ctx ->
  //If either operand is NaN, the result is NaN.
  Assert.AreEqual(1.0 % NaN, ctx.ExecuteT<double>("1 % NaN"));
  Assert.AreEqual(NaN % 1.0, ctx.ExecuteT<double>("NaN % 1"));

  //TODO: Expand tests

  Assert.AreEqual(1.0 % 0.0, ctx.ExecuteT<double>("1 % 0"));
  Assert.AreEqual(1.0 % 2.0, ctx.ExecuteT<double>("1 % 2"));
  Assert.AreEqual(1.0 % 2.0, ctx.ExecuteT<double>("1 % '2'"));
)

test "11.6.1 The Addition operator ( + )" (fun ctx ->
  Assert.AreEqual(1.0 + NaN, ctx.ExecuteT<double>("1 + NaN"));
  Assert.AreEqual(NaN + 1.0, ctx.ExecuteT<double>("NaN + 1"));
  Assert.AreEqual(1.0 + 2.0, ctx.ExecuteT<double>("1 + 2"));
  Assert.AreEqual(1.0.ToString() + "2", ctx.ExecuteT<string>("1 + '2'"));
  Assert.AreEqual("1" + 2.0.ToString(), ctx.ExecuteT<string>("'1' + 2"));
  Assert.AreEqual(1.0 + 1.0, ctx.ExecuteT<double>("1 + true"));
  Assert.AreEqual(1.0 + 0.0, ctx.ExecuteT<double>("1 + false"));
)

test "11.6.2 The Subtraction Operator ( - )" (fun ctx ->
  Assert.AreEqual(1.0 - NaN, ctx.ExecuteT<double>("1 - NaN"));
  Assert.AreEqual(NaN - 1.0, ctx.ExecuteT<double>("NaN - 1"));
  Assert.AreEqual(1.0 - 0.0, ctx.ExecuteT<double>("1 - 0"));
  Assert.AreEqual(1.0 - 1.0, ctx.ExecuteT<double>("1 - 1"));
  Assert.AreEqual(1.0 - 2.0, ctx.ExecuteT<double>("1 - 2"));
  Assert.AreEqual(1.0 - 2.0, ctx.ExecuteT<double>("1 - '2'"));
  Assert.AreEqual(1.0 - NaN, ctx.ExecuteT<double>("1 - 'b'"));
  Assert.AreEqual(1.0 - 1.0, ctx.ExecuteT<double>("1 - true"));
  Assert.AreEqual(1.0 - 0.0, ctx.ExecuteT<double>("1 - false"));
)

test "11.7.1 The Left Shift Operator ( << )" (fun ctx ->
  Assert.AreEqual(double (1 <<< int NaN), ctx.ExecuteT<double>("1 << NaN"));
  Assert.AreEqual(double (int NaN <<< 1), ctx.ExecuteT<double>("NaN << 1"));

  Assert.AreEqual(double (1 <<< 0), ctx.ExecuteT<double>("1 << 0"));
  Assert.AreEqual(double (1 <<< 1), ctx.ExecuteT<double>("1 << 1"));
  Assert.AreEqual(double (1 <<< 2), ctx.ExecuteT<double>("1 << 2"));
  Assert.AreEqual(double (1 <<< 2), ctx.ExecuteT<double>("1 << '2'"));
  Assert.AreEqual(double (1 <<< 1), ctx.ExecuteT<double>("1 << true"));
  Assert.AreEqual(double (1 <<< 0), ctx.ExecuteT<double>("1 << false"));
)

test "11.7.2 The Signed Right Shift Operator ( >> )" (fun ctx ->
  Assert.AreEqual(1.0, ctx.ExecuteT<double>("1 >> NaN"));
  Assert.AreEqual(0.0, ctx.ExecuteT<double>("NaN >> 1"));

  Assert.AreEqual(double (0xFF >>> 0), ctx.ExecuteT<double>("0xFF >> 0"));
  Assert.AreEqual(double (0xFF >>> 1), ctx.ExecuteT<double>("0xFF >> 1"));
  Assert.AreEqual(double (0xFF >>> 2), ctx.ExecuteT<double>("0xFF >> 2"));
  Assert.AreEqual(double (0xFF >>> 2), ctx.ExecuteT<double>("0xFF >> '2'"));
  Assert.AreEqual(double (0xFF >>> 1), ctx.ExecuteT<double>("0xFF >> true"));
  Assert.AreEqual(double (0xFF >>> 0), ctx.ExecuteT<double>("0xFF >> false"));
)

test "11.7.3 The Unsigned Right Shift Operator ( >>> )" (fun ctx ->
  Assert.AreEqual(double ((uint32 -1) >>> int NaN), ctx.ExecuteT<double>("-1 >>> NaN"));
  Assert.AreEqual(double ((uint32 -0xF) >>> 0), ctx.ExecuteT<double>("-0xF >>> 0"));
  Assert.AreEqual(double ((uint32 -0xF) >>> 1), ctx.ExecuteT<double>("-0xF >>> 1"));
  Assert.AreEqual(double ((uint32 -0xF) >>> 2), ctx.ExecuteT<double>("-0xF >>> 2"));
  Assert.AreEqual(double ((uint32 -0xF) >>> 2), ctx.ExecuteT<double>("-0xF >>> '2'"));
  Assert.AreEqual(double ((uint32 -0xF) >>> 1), ctx.ExecuteT<double>("-0xF >>> true"));
  Assert.AreEqual(double ((uint32 -0xF) >>> 0), ctx.ExecuteT<double>("-0xF >>> false"));

  Assert.AreEqual(double ((uint32 0xF) >>> 0), ctx.ExecuteT<double>("0xF >>> 0"));
  Assert.AreEqual(double ((uint32 0xF) >>> 1), ctx.ExecuteT<double>("0xF >>> 1"));
  Assert.AreEqual(double ((uint32 0xF) >>> 2), ctx.ExecuteT<double>("0xF >>> 2"));
  Assert.AreEqual(double ((uint32 0xF) >>> 2), ctx.ExecuteT<double>("0xF >>> '2'"));
  Assert.AreEqual(double ((uint32 0xF) >>> 1), ctx.ExecuteT<double>("0xF >>> true"));
  Assert.AreEqual(double ((uint32 0xF) >>> 0), ctx.ExecuteT<double>("0xF >>> false"));
)

test "11.8.1 The Less-than Operator ( < )" (fun ctx ->
  Assert.AreEqual(0.0 < NaN, ctx.ExecuteT<bool>("0 < NaN"));
  Assert.AreEqual(NaN < 0.0, ctx.ExecuteT<bool>("NaN < 0"));
  Assert.AreEqual(1.0 < 1.0, ctx.ExecuteT<bool>("1 < 1"));
  Assert.AreEqual(0.0 < 1.0, ctx.ExecuteT<bool>("0 < 1"));
  Assert.AreEqual(1.0 < 0.0, ctx.ExecuteT<bool>("1 < 0"));
  Assert.AreEqual(1.0 < 0.0, ctx.ExecuteT<bool>("1 < '0'"));
  Assert.AreEqual(1.0 < 1.0, ctx.ExecuteT<bool>("1 < true"));
  Assert.AreEqual(1.0 < 0.0, ctx.ExecuteT<bool>("1 < false"));
  Assert.AreEqual(1.0 < NaN, ctx.ExecuteT<bool>("1 < {}"));
)

test "11.8.2 The Greater-than Operator ( > )" (fun ctx ->
  Assert.AreEqual(0.0 > NaN, ctx.ExecuteT<bool>("0 > NaN"));
  Assert.AreEqual(NaN > 0.0, ctx.ExecuteT<bool>("NaN > 0"));
  Assert.AreEqual(1.0 > 1.0, ctx.ExecuteT<bool>("1 > 1"));
  Assert.AreEqual(0.0 > 1.0, ctx.ExecuteT<bool>("0 > 1"));
  Assert.AreEqual(1.0 > 0.0, ctx.ExecuteT<bool>("1 > 0"));
  Assert.AreEqual(1.0 > 0.0, ctx.ExecuteT<bool>("1 > '0'"));
  Assert.AreEqual(1.0 > 1.0, ctx.ExecuteT<bool>("1 > true"));
  Assert.AreEqual(1.0 > 0.0, ctx.ExecuteT<bool>("1 > false"));
  Assert.AreEqual(1.0 > NaN, ctx.ExecuteT<bool>("1 > {}"));
)

test "11.8.3 The Less-than-or-equal Operator ( <= )" (fun ctx ->
  Assert.AreEqual(0.0 <= NaN, ctx.ExecuteT<bool>("0 <= NaN"));
  Assert.AreEqual(NaN <= 0.0, ctx.ExecuteT<bool>("NaN <= 0"));
  Assert.AreEqual(1.0 <= 1.0, ctx.ExecuteT<bool>("1 <= 1"));
  Assert.AreEqual(0.0 <= 1.0, ctx.ExecuteT<bool>("0 <= 1"));
  Assert.AreEqual(1.0 <= 0.0, ctx.ExecuteT<bool>("1 <= 0"));
  Assert.AreEqual(1.0 <= 0.0, ctx.ExecuteT<bool>("1 <= '0'"));
  Assert.AreEqual(1.0 <= 1.0, ctx.ExecuteT<bool>("1 <= true"));
  Assert.AreEqual(1.0 <= 0.0, ctx.ExecuteT<bool>("1 <= false"));
  Assert.AreEqual(1.0 <= NaN, ctx.ExecuteT<bool>("1 <= {}"));
)

test "11.8.4 The Greater-than-or-equal Operator ( >= )" (fun ctx ->
  Assert.AreEqual(0.0 >= NaN, ctx.ExecuteT<bool>("0 >= NaN"));
  Assert.AreEqual(NaN >= 0.0, ctx.ExecuteT<bool>("NaN >= 0"));
  Assert.AreEqual(1.0 >= 1.0, ctx.ExecuteT<bool>("1 >= 1"));
  Assert.AreEqual(0.0 >= 1.0, ctx.ExecuteT<bool>("0 >= 1"));
  Assert.AreEqual(1.0 >= 0.0, ctx.ExecuteT<bool>("1 >= 0"));
  Assert.AreEqual(1.0 >= 0.0, ctx.ExecuteT<bool>("1 >= '0'"));
  Assert.AreEqual(1.0 >= 1.0, ctx.ExecuteT<bool>("1 >= true"));
  Assert.AreEqual(1.0 >= 0.0, ctx.ExecuteT<bool>("1 >= false"));
  Assert.AreEqual(1.0 >= NaN, ctx.ExecuteT<bool>("1 >= {}"));
)

test "11.9.1 The Equals Operator ( == )" (fun ctx ->
  Assert.AreEqual((NaN = NaN), ctx.ExecuteT<bool>("NaN == NaN"));
  Assert.AreEqual((0.0 = NaN), ctx.ExecuteT<bool>("0 == NaN"));
  Assert.AreEqual((NaN = 0.0), ctx.ExecuteT<bool>("NaN == 0"));
  Assert.AreEqual(1.0 = 1.0, ctx.ExecuteT<bool>("1 == 1"));
  Assert.AreEqual(0.0 = 1.0, ctx.ExecuteT<bool>("0 == 1"));
  Assert.AreEqual(1.0 = 0.0, ctx.ExecuteT<bool>("1 == 0"));
  Assert.AreEqual(false, ctx.ExecuteT<bool>("1 == '0'"));
  Assert.AreEqual(true, ctx.ExecuteT<bool>("1 == '1'"));
  Assert.AreEqual(true, ctx.ExecuteT<bool>("1 == true"));
  Assert.AreEqual(false, ctx.ExecuteT<bool>("1 == false"));
  Assert.AreEqual(false, ctx.ExecuteT<bool>("1 == {}"));
  Assert.AreEqual(false, ctx.ExecuteT<bool>("({} == {})"));
  Assert.AreEqual(true, ctx.ExecuteT<bool>("var x = {}; (x == x)"));
  Assert.AreEqual("foo" = "foo", ctx.ExecuteT<bool>("'foo' == 'foo'"));
  Assert.AreEqual("foo" = "bar", ctx.ExecuteT<bool>("'foo' == 'bar'"));
)

test "11.9.2 The Does-not-equals Operator ( != )" (fun ctx ->
  Assert.AreEqual(NaN <> NaN, ctx.ExecuteT<bool>("NaN != NaN"));
  Assert.AreEqual(0.0 <> NaN, ctx.ExecuteT<bool>("0 != NaN"));
  Assert.AreEqual(NaN <> 0.0, ctx.ExecuteT<bool>("NaN != 0"));
  Assert.AreEqual(1.0 <> 1.0, ctx.ExecuteT<bool>("1 != 1"));
  Assert.AreEqual(0.0 <> 1.0, ctx.ExecuteT<bool>("0 != 1"));
  Assert.AreEqual(1.0 <> 0.0, ctx.ExecuteT<bool>("1 != 0"));
  Assert.AreEqual(true, ctx.ExecuteT<bool>("1 != '0'"));
  Assert.AreEqual(false, ctx.ExecuteT<bool>("1 != '1'"));
  Assert.AreEqual(false, ctx.ExecuteT<bool>("1 != true"));
  Assert.AreEqual(true, ctx.ExecuteT<bool>("1 != false"));
  Assert.AreEqual(true, ctx.ExecuteT<bool>("1 != {}"));
  Assert.AreEqual(true, ctx.ExecuteT<bool>("({} != {})"));
  Assert.AreEqual(false, ctx.ExecuteT<bool>("var x = {}; (x != x)"));
  Assert.AreEqual("foo" <> "foo", ctx.ExecuteT<bool>("'foo' != 'foo'"));
  Assert.AreEqual("foo" <> "bar", ctx.ExecuteT<bool>("'foo' != 'bar'"));
)

test "11.9.4 The Strict Equals Operator ( === )" (fun ctx ->
  Assert.AreEqual((NaN = NaN), ctx.ExecuteT<bool>("NaN === NaN"));
  Assert.AreEqual((0.0 = NaN), ctx.ExecuteT<bool>("0 === NaN"));
  Assert.AreEqual((NaN = 0.0), ctx.ExecuteT<bool>("NaN === 0"));
  Assert.AreEqual(1.0 = 1.0, ctx.ExecuteT<bool>("1 === 1"));
  Assert.AreEqual(0.0 = 1.0, ctx.ExecuteT<bool>("0 === 1"));
  Assert.AreEqual(1.0 = 0.0, ctx.ExecuteT<bool>("1 === 0"));
  Assert.AreEqual(false, ctx.ExecuteT<bool>("1 === '0'"));
  Assert.AreEqual(false, ctx.ExecuteT<bool>("1 === '1'"));
  Assert.AreEqual(false, ctx.ExecuteT<bool>("1 === true"));
  Assert.AreEqual(false, ctx.ExecuteT<bool>("1 === false"));
  Assert.AreEqual(false, ctx.ExecuteT<bool>("1 === {}"));
  Assert.AreEqual(false, ctx.ExecuteT<bool>("({} === {})"));
  Assert.AreEqual(true, ctx.ExecuteT<bool>("var x = {}; (x === x)"));
  Assert.AreEqual("foo" = "foo", ctx.ExecuteT<bool>("'foo' === 'foo'"));
  Assert.AreEqual("foo" = "bar", ctx.ExecuteT<bool>("'foo' === 'bar'"));
)

test "11.9.5 The Strict Does-not-equal Operator ( !== )" (fun ctx ->
  Assert.AreEqual(NaN <> NaN, ctx.ExecuteT<bool>("NaN !== NaN"));
  Assert.AreEqual(0.0 <> NaN, ctx.ExecuteT<bool>("0 !== NaN"));
  Assert.AreEqual(NaN <> 0.0, ctx.ExecuteT<bool>("NaN !== 0"));
  Assert.AreEqual(1.0 <> 1.0, ctx.ExecuteT<bool>("1 !== 1"));
  Assert.AreEqual(0.0 <> 1.0, ctx.ExecuteT<bool>("0 !== 1"));
  Assert.AreEqual(1.0 <> 0.0, ctx.ExecuteT<bool>("1 !== 0"));
  Assert.AreEqual(true, ctx.ExecuteT<bool>("1 !== '0'"));
  Assert.AreEqual(true, ctx.ExecuteT<bool>("1 !== '1'"));
  Assert.AreEqual(true, ctx.ExecuteT<bool>("1 !== true"));
  Assert.AreEqual(true, ctx.ExecuteT<bool>("1 !== false"));
  Assert.AreEqual(true, ctx.ExecuteT<bool>("1 !== {}"));
  Assert.AreEqual(true, ctx.ExecuteT<bool>("({} !== {})"));
  Assert.AreEqual(false, ctx.ExecuteT<bool>("var x = {}; (x !== x)"));
  Assert.AreEqual("foo" <> "foo", ctx.ExecuteT<bool>("'foo' !== 'foo'"));
  Assert.AreEqual("foo" <> "bar", ctx.ExecuteT<bool>("'foo' !== 'bar'"));
)

test "11.10 Binary Bitwise Operators" (fun ctx ->
  Assert.AreEqual(double (1 &&& 2), ctx.ExecuteT<double>("1 & 2"));
  Assert.AreEqual(double (2 &&& 1), ctx.ExecuteT<double>("2 & true"));
  Assert.AreEqual(double (2 &&& 0), ctx.ExecuteT<double>("2 & false"));
  Assert.AreEqual(double (2 &&& 3), ctx.ExecuteT<double>("2 & '3'"));

  Assert.AreEqual(double (1 ||| 2), ctx.ExecuteT<double>("1 | 2"));
  Assert.AreEqual(double (2 ||| 1), ctx.ExecuteT<double>("2 | true"));
  Assert.AreEqual(double (2 ||| 0), ctx.ExecuteT<double>("2 | false"));
  Assert.AreEqual(double (2 ||| 3), ctx.ExecuteT<double>("2 | '3'"));

  Assert.AreEqual(double (1 ^^^ 2), ctx.ExecuteT<double>("1 ^ 2"));
  Assert.AreEqual(double (2 ^^^ 1), ctx.ExecuteT<double>("2 ^ true"));
  Assert.AreEqual(double (2 ^^^ 0), ctx.ExecuteT<double>("2 ^ false"));
  Assert.AreEqual(double (2 ^^^ 3), ctx.ExecuteT<double>("2 ^ '3'"));

  Assert.AreEqual(double (2 &&& 0), ctx.ExecuteT<double>("2 & 'd'"));
  Assert.AreEqual(double (2 ||| 0), ctx.ExecuteT<double>("2 | 'd'"));
  Assert.AreEqual(double (2 ^^^ 0), ctx.ExecuteT<double>("2 ^ 'd'"));
)

test "11.11 Binary Logical Operators" (fun ctx ->
  Assert.AreEqual(true, ctx.ExecuteT<bool>("true && true"));
  Assert.AreEqual(false, ctx.ExecuteT<bool>("true && false"));
  Assert.AreEqual(false, ctx.ExecuteT<bool>("false && false"));
  Assert.AreEqual(false, ctx.ExecuteT<bool>("false && true"));

  Assert.AreEqual(true, ctx.ExecuteT<bool>("true || true"));
  Assert.AreEqual(true, ctx.ExecuteT<bool>("true || false"));
  Assert.AreEqual(false, ctx.ExecuteT<bool>("false || false"));
  Assert.AreEqual(true, ctx.ExecuteT<bool>("false || true"));

  Assert.AreEqual(1.0, ctx.ExecuteT<double>("1 && 1"));
  Assert.AreEqual(0.0, ctx.ExecuteT<double>("1 && 0"));
  Assert.AreEqual(0.0, ctx.ExecuteT<double>("0 && 0"));
  Assert.AreEqual(0.0, ctx.ExecuteT<double>("0 && 1"));

  Assert.AreEqual(1.0, ctx.ExecuteT<double>("1 || 1"));
  Assert.AreEqual(1.0, ctx.ExecuteT<double>("1 || 0"));
  Assert.AreEqual(0.0, ctx.ExecuteT<double>("0 || 0"));
  Assert.AreEqual(1.0, ctx.ExecuteT<double>("0 || 1"));

  Assert.AreEqual("bar", ctx.ExecuteT<string>("'foo' && 'bar'"));
  Assert.AreEqual("", ctx.ExecuteT<string>("'foo' && ''"));
  Assert.AreEqual("", ctx.ExecuteT<string>("'' && ''"));
  Assert.AreEqual("", ctx.ExecuteT<string>("'' && 'bar'"));
)

test "11.12 Conditional Operator ( ?: )" (fun ctx ->
  Assert.AreEqual(1.0, ctx.ExecuteT<double>("true ? 1 : 0"));
  Assert.AreEqual(0.0, ctx.ExecuteT<double>("false ? 1 : 0"));

  Assert.AreEqual(1.0, ctx.ExecuteT<double>("1 ? 1 : 0"));
  Assert.AreEqual(0.0, ctx.ExecuteT<double>("0 ? 1 : 0"));

  Assert.AreEqual(1.0, ctx.ExecuteT<double>("'foo' ? 1 : 0"));
  Assert.AreEqual(0.0, ctx.ExecuteT<double>("'' ? 1 : 0"));
)

report()