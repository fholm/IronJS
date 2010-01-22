using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronJS.Tests
{
    [TestClass]
    public class IfTest
    {
        [TestMethod]
        public void TestIf()
        {
            Assert.AreEqual(
                "if",
                ScriptRunner.Run(
                    "if(true) { emit('if'); }"
                )
            );
        }

        [TestMethod]
        public void TestIfElse()
        {
            Assert.AreEqual(
                "else",
                ScriptRunner.Run(
                    "if(false) { emit('if'); } else { emit('else'); }"
                )
            );
        }

        [TestMethod]
        public void TestIfElseIfelse()
        {
            Assert.AreEqual(
                "else-if",
                ScriptRunner.Run(
                    "if(false) { "
                        + "emit('if'); "

                    +"} else if(true) { "
                        + "emit('else-if'); "

                    +"} else { "
                        + "emit('else'); "
                    +"}"
                )
            );
        }

        [TestMethod]
        public void TestOperatorConditionalWhenTrue()
        {
            Assert.AreEqual(
                "is-true",
                ScriptRunner.Run(
                    "emit(true ? 'is-true' : 'is-false');"
                )
            );
        }

        [TestMethod]
        public void TestOperatorConditionalWhenFalse()
        {
            Assert.AreEqual(
                "is-false",
                ScriptRunner.Run(
                    "emit(false ? 'is-true' : 'is-false');"
                )
            );
        }
    }
}
