using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronJS.Tests
{
    [TestClass]
    public class TypeConversionTests
    {
        [TestMethod]
        public void TestEmptyStringToBool()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(!!'')"
                )
            );
        }

        [TestMethod]
        public void TestStringToBool()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(!!'foo')"
                )
            );
        }

        [TestMethod]
        public void TestNumberToBool()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(!!1)"
                )
            );
        }

        [TestMethod]
        public void TestNumberFractionsToBool()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(!!1.25)"
                )
            );
        }

        [TestMethod]
        public void TestNumberPositiveZeroToBool()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(!!+0)"
                )
            );
        }

        [TestMethod]
        public void TestNumberNegativeZeroToBool()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(!!-0)"
                )
            );
        }

        [TestMethod]
        public void TestNullToBool()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(!!null)"
                )
            );
        }

        [TestMethod]
        public void TestUndefinedToBool()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(!!undefined)"
                )
            );
        }

        [TestMethod]
        public void TestTrueToBool()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(!!true)"
                )
            );
        }

        [TestMethod]
        public void TestFalseToBool()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(!!false)"
                )
            );
        }

        [TestMethod]
        public void TestUndefinedToNumber()
        {
            Assert.AreEqual(
                "NaN",
                ScriptRunner.Run(
                    "emit(+undefined)"
                )
            );
        }

        [TestMethod]
        public void TestNullToNumber()
        {
            Assert.AreEqual(
                "0",
                ScriptRunner.Run(
                    "emit(+null)"
                )
            );
        }

        [TestMethod]
        public void TestTrueToNumber()
        {
            Assert.AreEqual(
                "1",
                ScriptRunner.Run(
                    "emit(+true)"
                )
            );
        }

        [TestMethod]
        public void TestFalseToNumber()
        {
            Assert.AreEqual(
                "0",
                ScriptRunner.Run(
                    "emit(+false)"
                )
            );
        }

        [TestMethod]
        public void TestNumberToNumber()
        {
            Assert.AreEqual(
                "1",
                ScriptRunner.Run(
                    "emit(+1)"
                )
            );
        }

        [TestMethod]
        public void TestNumberFractionToNumber()
        {
            Assert.AreEqual(
                "1.25",
                ScriptRunner.Run(
                    "emit(+1.25)"
                )
            );
        }

        [TestMethod]
        public void TestNumericLiteralStringToNumber()
        {
            Assert.AreEqual(
                "1",
                ScriptRunner.Run(
                    "emit(+'1')"
                )
            );
        }

        [TestMethod]
        public void TestEmptyStringToNumber()
        {
            Assert.AreEqual(
                "NaN",
                ScriptRunner.Run(
                    "emit(+'')"
                )
            );
        }

        [TestMethod]
        public void TestAlphabeticLiteralStringToNumber()
        {
            Assert.AreEqual(
                "NaN",
                ScriptRunner.Run(
                    "emit(+'abc')"
                )
            );
        }

        [TestMethod]
        public void TestNumberToString()
        {
            Assert.AreEqual(
                "3.14",
                ScriptRunner.Run(
                    "emit(3.14)"
                )
            );
        }

        [TestMethod]
        public void TestTrueToString()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(true)"
                )
            );
        }

        [TestMethod]
        public void TestFalseToString()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(false)"
                )
            );
        }

        [TestMethod]
        public void TestNaNToString()
        {
            //TODO: Test named wrong?
            Assert.AreEqual(
                "NaN",
                ScriptRunner.Run(
                    "emit(1 + undefined)"
                )
            );
        }

        [TestMethod]
        public void TestStringToString()
        {
            Assert.AreEqual(
                "foo",
                ScriptRunner.Run(
                    "emit('foo')"
                )
            );
        }

        [TestMethod]
        public void TestNullToString()
        {
            Assert.AreEqual(
                "null",
                ScriptRunner.Run(
                    "emit(null)"
                )
            );
        }

        [TestMethod]
        public void TestUndefinedToString()
        {
            Assert.AreEqual(
                "undefined",
                ScriptRunner.Run(
                    "emit(undefined)"
                )
            );
        }

        //TODO: UNIT TEST ToNumber() for Object input
        //TODO: UNIT TEST ToBoolean() for Object input
        //TODO: UNIT TEST ToString() for Object input
        //TODO: UNIT TEST ToObject() for all input
    }
}
