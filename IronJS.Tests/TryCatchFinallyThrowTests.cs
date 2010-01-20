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
    }
}
