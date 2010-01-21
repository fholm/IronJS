using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronJS.Tests
{
    [TestClass]
    public class TryCatchFinallyThrowTests
    {
        [TestMethod]
        public void TestThrow()
        {
            var exceptionCaught = false;

            try
            {
                ScriptRunner.Run("throw {};");
            }
            catch (IronJS.Runtime.JsRuntimeError)
            {
                exceptionCaught = true;
            }

            Assert.AreEqual(true, exceptionCaught);
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
            var exceptionCaught = false;
            string result = "";
            var emitter = new StringBuilder();

            try
            {
                ScriptRunner.Run("try { throw {}; } finally { emit('foo') }", ref emitter);
            }
            catch (IronJS.Runtime.JsRuntimeError)
            {
                exceptionCaught = true;
            }

            Assert.AreEqual("foo", emitter.ToString());
            Assert.AreEqual(true, exceptionCaught);
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
