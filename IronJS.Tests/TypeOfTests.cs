using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronJS.Tests
{
    [TestClass]
    public class TypeOfTests
    {
        [TestMethod]
        public void TestTypeOfString()
        {
            Assert.AreEqual(
                "string",
                ScriptRunner.Run(
                    "emit(typeof 'foo')"
                )
            );
        }

        [TestMethod]
        public void TestTypeOfNumber()
        {
            Assert.AreEqual(
                "number",
                ScriptRunner.Run(
                    "emit(typeof 1)"
                )
            );
        }

        [TestMethod]
        public void TestTypeOfNumberFraction()
        {
            Assert.AreEqual(
                "number",
                ScriptRunner.Run(
                    "emit(typeof 1.25)"
                )
            );
        }

        [TestMethod]
        public void TestTypeOfTrue()
        {
            Assert.AreEqual(
                "boolean",
                ScriptRunner.Run(
                    "emit(typeof true)"
                )
            );
        }

        [TestMethod]
        public void TestTypeOfFalse()
        {
            Assert.AreEqual(
                "boolean",
                ScriptRunner.Run(
                    "emit(typeof false)"
                )
            );
        }

        [TestMethod]
        public void TestTypeOfNull()
        {
            Assert.AreEqual(
                "object",
                ScriptRunner.Run(
                    "emit(typeof null)"
                )
            );
        }

        [TestMethod]
        public void TestTypeOfUndefined()
        {
            Assert.AreEqual(
                "undefined",
                ScriptRunner.Run(
                    "emit(typeof undefined)"
                )
            );
        }

        [TestMethod]
        public void TestTypeOfFunction()
        {
            Assert.AreEqual(
                "function",
                ScriptRunner.Run(
                    "emit(typeof function(){})"
                )
            );
        }

        [TestMethod]
        public void TestTypeOfObject()
        {
            Assert.AreEqual(
                "object",
                ScriptRunner.Run(
                    "emit(typeof {})"
                )
            );
        }

        //TODO: UNIT TEST test typeof on object input
        //TODO: UNIT TEST test typeof on clr input
    }
}
