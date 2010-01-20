using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronJS.Tests
{
    [TestClass]
    public class StrictEqualityTests
    {
        // 11.9.6, 1
        [TestMethod]
        public void TestStrictCompareNumericalStringToNumber()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(1 === '1')"
                )
            );
        }

        // 11.9.6, 1
        [TestMethod]
        public void TestStrictCompareNumericalStringToNumberFraction()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(1.25 === '1.25')"
                )
            );
        }

        // 11.9.6, 1
        [TestMethod]
        public void TestStrictCompareNumberToTrue()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(1 === true)"
                )
            );
        }

        // 11.9.6, 1
        [TestMethod]
        public void TestStrictCompareNumberToFalse()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(0 === false)"
                )
            );
        }

        // 11.9.6, 1
        [TestMethod]
        public void TestStrictCompareUndefinedToNull()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(undefined === null)"
                )
            );
        }

        // 11.9.6, 2
        [TestMethod]
        public void TestStrictCompareUndefinedToUndefined()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(undefined === undefined)"
                )
            );
        }

        // 11.9.6, 3
        [TestMethod]
        public void TestStrictCompareNullToNull()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(null === null)"
                )
            );
        }

        // 11.9.6, 5 and 6
        [TestMethod]
        public void TestStrictCompareNaNToNaN()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(NaN === NaN)"
                )
            );
        }

        // 11.9.6, 7
        [TestMethod]
        public void TestStrictCompareNumberToNumber()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(1 === 1)"
                )
            );
        }

        // 11.9.6, 7
        [TestMethod]
        public void TestStrictCompareNumberFractionsToNumberFractions()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(1.25 === 1.25)"
                )
            );
        }

        // 11.9.6, 8 and 9
        [TestMethod]
        public void TestStrictCompareNegativeZeroToPositiveZero()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(-0 === +0)"
                )
            );
        }

        // 11.9.6, 10
        [TestMethod]
        public void TestStrictCompareNonEqualNumbers()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(1 === 2)"
                )
            );
        }

        // 11.9.6, 10
        [TestMethod]
        public void TestStrictCompareEqualStrings()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit('foo' === 'foo')"
                )
            );
        }

        // 11.9.6, 10
        [TestMethod]
        public void TestStrictCompareNotEqualStrings()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit('Foo' === 'foo')"
                )
            );
        }

        // 11.9.3, 12
        [TestMethod]
        public void TestStrictCompareTrueAndFalse()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(true === false)"
                )
            );
        }

        // 11.9.3, 12
        [TestMethod]
        public void TestStrictCompareTrueAndTrue()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(true === true)"
                )
            );
        }

        // 11.9.3, 12
        [TestMethod]
        public void TestStrictCompareFalseAndFalse()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(false === false)"
                )
            );
        }

        //TODO: UNIT TEST strict equality comparison for objects
    }
}
