using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IronJS.Runtime;

namespace IronJS.Tests
{
    [TestClass]
    public class TryCatchFinallyThrowTests
    {
        [TestMethod]
        public void TestThrow()
        {
            try
            {
                ScriptRunner.Run("throw {};");
                Assert.Fail("Should throw JsRuntimeError");
            }
            catch (JsRuntimeError)
            {

            }
        }

        [TestMethod]
        public void TestTryCatch()
        {
            Assert.AreEqual(
                "foo",
                ScriptRunner.Run(
                    "try { throw {}; } "
                    + "catch(ex) { emit('foo') }"
                )
            );
        }

        [TestMethod]
        public void TestTryFinally()
        {
            var emitter = new StringBuilder();

            try
            {
                ScriptRunner.Run("try { throw {}; } finally { emit('foo') }", ref emitter);
                Assert.Fail("Should throw JsRuntimeError");
            }
            catch (JsRuntimeError)
            {

            }

            Assert.AreEqual("foo", emitter.ToString());
        }

        [TestMethod]
        public void TestTryCatchFinally()
        {
            Assert.AreEqual(
                "foobar",
                ScriptRunner.Run(
                    "try { throw {}; } "
                    + "catch(ex) { emit('foo') } "
                    + "finally { emit('bar'); }"
                )
            );
        }

        //TODO: UNIT TEST use proper error object for throw statements
        //TODO: UNIT TEST unit test error object type, properties, etc
    }
}
