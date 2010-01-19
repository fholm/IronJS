using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronJS.Tests
{
    [TestClass]
    public class TypeConversionAddTests
    {
        [TestMethod]
        public void TestAddNumberToString()
        {
            Assert.AreEqual(
                "31",
                ScriptRunner.Run(
                    "emit(3 + '1')"
                )
            );
        }

        [TestMethod]
        public void TestAddTrueToString()
        {
            Assert.AreEqual(
                "truefoo",
                ScriptRunner.Run(
                    "emit(true + 'foo')"
                )
            );
        }

        [TestMethod]
        public void TestAddFalseToString()
        {
            Assert.AreEqual(
                "falsefoo",
                ScriptRunner.Run(
                    "emit(false + 'foo')"
                )
            );
        }

        [TestMethod]
        public void TestAddNullToString()
        {
            Assert.AreEqual(
                "nullfoo",
                ScriptRunner.Run(
                    "emit(null + 'foo')"
                )
            );
        }

        [TestMethod]
        public void TestAddUndefinedToString()
        {
            Assert.AreEqual(
                "undefinedfoo",
                ScriptRunner.Run(
                    "emit(undefined + 'foo')"
                )
            );
        }

        [TestMethod]
        public void TestAddStringToString()
        {
            Assert.AreEqual(
                "foofoo",
                ScriptRunner.Run(
                    "emit('foo' + 'foo')"
                )
            );
        }

        [TestMethod]
        public void TestAddTrueToNumber()
        {
            Assert.AreEqual(
                "2",
                ScriptRunner.Run(
                    "emit(true + 1)"
                )
            );
        }

        [TestMethod]
        public void TestAddFalseToNumber()
        {
            Assert.AreEqual(
                "1",
                ScriptRunner.Run(
                    "emit(false + 1)"
                )
            );
        }

        [TestMethod]
        public void TestAddNullToNumber()
        {
            Assert.AreEqual(
                "1",
                ScriptRunner.Run(
                    "emit(null + 1)"
                )
            );
        }

        [TestMethod]
        public void TestAddUndefinedToNumber()
        {
            Assert.AreEqual(
                "NaN",
                ScriptRunner.Run(
                    "emit(foo + 1)" // foo == undefined == ToNumber(undefined) == NaN
                )
            );
        }

        [TestMethod]
        public void TestNaNIsViral()
        {
            Assert.AreEqual(
                "NaN",
                ScriptRunner.Run(
                    "emit(1 + 2 + 3 + foo)" // foo == undefined == ToNumber(undefined) == NaN
                )
            );
        }
    }
}
