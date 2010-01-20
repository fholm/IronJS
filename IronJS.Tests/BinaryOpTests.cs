using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronJS.Tests
{
    [TestClass]
    public class BinaryOpTests
    {
        [TestMethod]
        public void TestOperatorAdd()
        {
            Assert.AreEqual(
                "3",
                ScriptRunner.Run(
                    "emit(1+2)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorAddAssign()
        {
            Assert.AreEqual(
                "3",
                ScriptRunner.Run(
                    "foo = 1;"
                  + "foo += 2;"
                  + "emit(foo);"
                )
            );
        }

        [TestMethod]
        public void TestOperatorAddFractions()
        {
            Assert.AreEqual(
                "3.75",
                ScriptRunner.Run(
                    "emit(1.5 + 2.25)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorSub()
        {
            Assert.AreEqual(
                "-1",
                ScriptRunner.Run(
                    "emit(1-2)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorSubFractions()
        {
            Assert.AreEqual(
                "-0.75",
                ScriptRunner.Run(
                    "emit(1.5 - 2.25)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorSubAssign()
        {
            Assert.AreEqual(
                "-1",
                ScriptRunner.Run(
                    "foo = 1;"
                  + "foo -= 2;"
                  + "emit(foo);"
                )
            );
        }

        [TestMethod]
        public void TestOperatorMul()
        {
            Assert.AreEqual(
                "20",
                ScriptRunner.Run(
                    "emit(4*5)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorMulFractions()
        {
            Assert.AreEqual(
                "23.625",
                ScriptRunner.Run(
                    "emit(4.5 * 5.25)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorMulAssign()
        {
            Assert.AreEqual(
                "20",
                ScriptRunner.Run(
                    "foo = 4;"
                  + "foo *= 5;"
                  + "emit(foo);"
                )
            );
        }

        [TestMethod]
        public void TestOperatorDiv()
        {
            Assert.AreEqual(
                "10",
                ScriptRunner.Run(
                    "emit(30/3)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorDivFractions()
        {
            Assert.AreEqual(
                "3.125",
                ScriptRunner.Run(
                    "emit(7.5 / 2.4)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorDivAssign()
        {
            Assert.AreEqual(
                "10",
                ScriptRunner.Run(
                    "foo = 30;"
                  + "foo /= 3;"
                  + "emit(foo);"
                )
            );
        }

        [TestMethod]
        public void TestOperatorModulo()
        {
            Assert.AreEqual(
                "1",
                ScriptRunner.Run(
                    "emit(10%3)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorModuloFractions()
        {
            Assert.AreEqual(
                "0.5",
                ScriptRunner.Run(
                    "emit(3.5 % 1.5)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorModuloAssign()
        {
            Assert.AreEqual(
                "1",
                ScriptRunner.Run(
                    "foo = 10;"
                  + "foo %= 3;"
                  + "emit(foo);"
                )
            );
        }

        [TestMethod]
        public void TestOperatorLessThan()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(1<2)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorLessThanFractions()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(1.5<2.5)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorLessThanOrEqual()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(2<=2)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorLessThanOrEqualFractions()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(2.25<=2.25)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorGreaterThen()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(1>2)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorGreaterThenOrEqual()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(1>=2)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorGreaterThenOrEqualFractions()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(1.75>=2.25)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorEquals()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(1==1)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorEqualsFractions()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(1.25==1.25)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorNotEqual()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(1!=1)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorNotEqualFractions()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(1.25!=1.25)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorAnd()
        {
            Assert.AreEqual(
                "0",
                ScriptRunner.Run(
                    "emit(1&2)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorAndFractions()
        {
            Assert.AreEqual(
                "0",
                ScriptRunner.Run(
                    "emit(1.25&2.25)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorAndAssign()
        {
            Assert.AreEqual(
                "0",
                ScriptRunner.Run(
                    "foo = 1;"
                  + "foo &= 2;"
                  + "emit(foo);"
                )
            );
        }

        [TestMethod]
        public void TestOperatorUnsignedRightShift()
        {
            Assert.AreEqual(
                "1073741820",
                ScriptRunner.Run(
                    "emit(-14 >>> 2)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorUnsignedRightShiftFractions()
        {
            Assert.AreEqual(
                "1073741820",
                ScriptRunner.Run(
                    "emit(-14.25 >>> 2)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorUnsignedRightShiftAssign()
        {
            Assert.AreEqual(
                "1073741820",
                ScriptRunner.Run(
                    "foo = -14;"
                    + "foo >>>= 2;"
                    + "emit(foo)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorOr()
        {
            Assert.AreEqual(
                "3",
                ScriptRunner.Run(
                    "emit(1|2)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorOrFractions()
        {
            Assert.AreEqual(
                "3",
                ScriptRunner.Run(
                    "emit(1.25|2.25)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorOrAssign()
        {
            Assert.AreEqual(
                "3",
                ScriptRunner.Run(
                    "foo = 1;"
                  + "foo |= 2;"
                  + "emit(foo);"
                )
            );
        }

        [TestMethod]
        public void TestOperatorExclusiveOr()
        {
            Assert.AreEqual(
                "2",
                ScriptRunner.Run(
                    "emit(1^3)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorExclusiveOrFractions()
        {
            Assert.AreEqual(
                "2",
                ScriptRunner.Run(
                    "emit(1.25^3.25)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorExclusiveOrAssign()
        {
            Assert.AreEqual(
                "2",
                ScriptRunner.Run(
                    "foo = 1;"
                  + "foo ^= 3;"
                  + "emit(foo);"
                )
            );
        }

        [TestMethod]
        public void TestOperatorLeftShift()
        {
            Assert.AreEqual(
                "4",
                ScriptRunner.Run(
                    "emit(1<<2)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorLeftShiftFractions()
        {
            Assert.AreEqual(
                "4",
                ScriptRunner.Run(
                    "emit(1.25<<2.25)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorLeftShiftAssign()
        {
            Assert.AreEqual(
                "4",
                ScriptRunner.Run(
                    "foo = 1;"
                  + "foo <<= 2;"
                  + "emit(foo);"
                )
            );
        }

        [TestMethod]
        public void TestOperatorRightShift()
        {
            Assert.AreEqual(
                "1",
                ScriptRunner.Run(
                    "emit(4>>2)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorRightShiftFractions()
        {
            Assert.AreEqual(
                "1",
                ScriptRunner.Run(
                    "emit(4.25>>2.25)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorRightShiftAssign()
        {
            Assert.AreEqual(
                "1",
                ScriptRunner.Run(
                    "foo = 4;"
                  + "foo >>= 2;"
                  + "emit(1);"
                )
            );
        }
    }
}
