using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronJS.Tests
{
    [TestClass]
    public class ForStepTests
    {
        [TestMethod]
        public void TestForStep()
        {
            Assert.AreEqual(
                "01234",
                ScriptRunner.Run(
                    "for(var i = 0; i < 5; ++i) { emit(i); }"
                )
            );
        }

        [TestMethod]
        public void TestForStepWithoutVar()
        {
            Assert.AreEqual(
                "012345",
                ScriptRunner.Run(
                    "for(i = 0; i < 5; ++i) { emit(i); } emit(i);"
                )
            );
        }

        [TestMethod]
        public void TestForStepWithNoSetup()
        {
            Assert.AreEqual(
                "01234",
                ScriptRunner.Run(
                    "i = 0; for( ; i < 5; ++i) { emit(i); }"
                )
            );
        }

        [TestMethod]
        public void TestForStepWithNoSetupOrIncrement()
        {
            Assert.AreEqual(
                "01234",
                ScriptRunner.Run(
                    "i = 0; for( ; i < 5 ; ) { emit(i); ++i; }"
                )
            );
        }

        [TestMethod]
        public void TestForStepOtherTrueishValueIsOk()
        {
            Assert.AreEqual(
                "foo",
                ScriptRunner.Run(
                    "for( ; 1 ; ) { emit('foo'); break; }"
                )
            );
        }

        [TestMethod]
        public void TestForStepOtherFalseishValueIsOk()
        {
            Assert.AreEqual(
                "bar",
                ScriptRunner.Run(
                    "for( ; 0 ; ) { emit('foo'); break; } emit('bar'); "
                )
            );
        }

        [TestMethod]
        public void TestForStepWithNoSetupOrIncrementOrTestAndBreak()
        {
            Assert.AreEqual(
                "0",
                ScriptRunner.Run(
                    "i = 0; for( ; ; ) { emit(i); break; }"
                )
            );
        }

        [TestMethod]
        public void TestForStepLabelledBreak()
        {
            Assert.AreEqual(
                "0",
                ScriptRunner.Run(
                    "outer: for(i = 0; i < 5; ++i) { for(j = 0; j < 2; ++i) { emit(j); break outer; } } "
                )
            );
        }

        [TestMethod]
        public void TestForStepNestedBreakNoLabels()
        {
            Assert.AreEqual(
                "012012",
                ScriptRunner.Run(
                    "for(i = 0; i < 2; ++i) { for(j = 0; j < 5; ++j) { if(j == 3) break; emit(j);  } } "
                )
            );
        }

        [TestMethod]
        public void TestForStepContinue()
        {
            Assert.AreEqual(
                "0124",
                ScriptRunner.Run(
                    "for(i = 0; i < 5; ++i) { if(i == 3) continue; emit(i); } "
                )
            );
        }

        [TestMethod]
        public void TestForStepNestedContinueNoLabels()
        {
            Assert.AreEqual(
                "0124",
                ScriptRunner.Run(
                    "for(i = 0; i < 5; ++i) { for(j = 0; j < 5; ++j) { if(j == 3) continue; emit(j);  } break; } "
                )
            );
        }

        [TestMethod]
        public void TestForStepLabelledContinue()
        {
            Assert.AreEqual(
                "00000",
                ScriptRunner.Run(
                    "outer: for(i = 0; i < 5; ++i) { for(j = 0; j < 2; ++i) { emit(j); continue outer; } } "
                )
            );
        }
    }
}
