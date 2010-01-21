using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronJS.Tests
{
    [TestClass]
    public class AbstractEqualityTests
    {
        // 11.9.3, 2
        [TestMethod]
        public void TestCompareUndefinedToUndefined()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(undefined == undefined)" 
                )
            );
        }

        // 11.9.3, 3
        [TestMethod]
        public void TestCompareNullToNull()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(null == null)"
                )
            );
        }

        // 11.9.3, 5 and 6
        [TestMethod]
        public void TestCompareNaNToNaN()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(NaN == NaN)" 
                )
            );
        }

        // 11.9.3, 5 and 6
        [TestMethod]
        public void TestCompareNumberToNaN()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(1 == NaN)" 
                )
            );
        }

        // 11.9.3, 5 and 6
        [TestMethod]
        public void TestCompareNaNToNumber()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(NaN == 1)"
                )
            );
        }

        // 11.9.3, 8 and 9
        [TestMethod]
        public void TestCompareNegativeZeroToPositiveZero()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(+0 == -0)"
                )
            );
        }

        // 11.9.3, 7
        [TestMethod]
        public void TestCompareSameNumber()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(1 == 1)"
                )
            );
        }

        // 11.9.3, 7
        [TestMethod]
        public void TestCompareSameNumberFractions()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(1.25 == 1.25)"
                )
            );
        }

        // 11.9.3, 10
        [TestMethod]
        public void TestCompareTwoDifferentNumbers()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(1 == 3)"
                )
            );
        }

        // 11.9.3, 10
        [TestMethod]
        public void TestCompareTwoDifferentNumbersFractions()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(1.25 == 3.25)"
                )
            );
        }

        // 11.9.3, 11
        [TestMethod]
        public void TestCompareTwoEqualStrings()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit('foo' == 'foo')"
                )
            );
        }

        // 11.9.3, 11
        [TestMethod]
        public void TestCompareTwoNotEqualStrings()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit('bar' == 'foo')"
                )
            );
        }

        // 11.9.3, 11
        [TestMethod]
        public void TestCompareTwoStringsWithDifferentCase()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit('Foo' == 'foo')"
                )
            );
        }

        // 11.9.3, 12
        [TestMethod]
        public void TestCompareTrueAndFAlse()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(true == false)"
                )
            );
        }

        // 11.9.3, 12
        [TestMethod]
        public void TestCompareTrueAndTrue()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(true == true)"
                )
            );
        }

        // 11.9.3, 12
        [TestMethod]
        public void TestCompareFalseAndFalse()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(false == false)"
                )
            );
        }

        // 11.9.3, 13
        [TestMethod]
        public void TestCompareSameObject()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "foo = {}; emit(foo == foo)"
                )
            );
        }

        // 11.9.3, 13
        [TestMethod]
        public void TestCompareDifferentObjects()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "foo = {}; bar = {}; emit(foo == bar)"
                )
            );
        }

        // 11.9.3, 14
        [TestMethod]
        public void TestCompareNullToUndefined()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(null == undefined)"
                )
            );
        }

        // 11.9.3, 15
        [TestMethod]
        public void TestCompareUndefinedToNull()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(undefined == null)"
                )
            );
        }

        // 11.9.3, 16 and 17
        [TestMethod]
        public void TestCompareEqualStringToNumber()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(1 == '1')"
                )
            );
        }

        // 11.9.3, 16 and 17
        [TestMethod]
        public void TestCompareEqualStringToNumberFractions()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(1.25 == '1.25')"
                )
            );
        }

        // 11.9.3, 16 and 17
        [TestMethod]
        public void TestCompareNotEqualStringToNumber()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(1 == '2')"
                )
            );
        }

        // 11.9.3, 16 and 17
        [TestMethod]
        public void TestCompareNotEqualStringToNumberFractions()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(1.25 == '2.25')"
                )
            );
        }

        // 11.9.3, 18 and 19
        [TestMethod]
        public void TestCompareTrueToNumber()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(1 == true)"
                )
            );
        }

        // 11.9.3, 18 and 19
        [TestMethod]
        public void TestCompareFalseToNumber()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(0 == false)"
                )
            );
        }

        // 11.9.3, 18 and 19
        [TestMethod]
        public void TestCompareFalseToNumberFraction()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(0.5 == false)"
                )   
            );
        }

        // 11.9.3, 18 and 19
        [TestMethod]
        public void TestCompareTrueToNumberFraction()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(1.5 == true)"
                )
            );
        }

        //TODO: UNIT TEST for abstract equality comparison step 20 and 21
    }
}
