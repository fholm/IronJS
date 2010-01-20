using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronJS.Tests
{
    [TestClass]
    public class LogicalAndOrTests
    {
        [TestMethod]
        public void TestLogicalAndReturnsSecondIfFirstIsTrue()
        {
            Assert.AreEqual(
                "2",
                ScriptRunner.Run(
                    "emit(true && 2)"
                )
            );
        }

        [TestMethod]
        public void TestLogicalAndReturnsFirstIfFirstIsFalse()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(false && 2)"
                )
            );
        }

        [TestMethod]
        public void TestLogicalOrReturnsFirstIfFirstIsTrue()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(true || 2)"
                )
            );
        }

        [TestMethod]
        public void TestLogicalOrReturnsSecondIfFirstIsFalse()
        {
            Assert.AreEqual(
                "2",
                ScriptRunner.Run(
                    "emit(false || 2)"
                )
            );
        }

        [TestMethod]
        public void TestLogicalAndWorksWithIf()
        {
            Assert.AreEqual(
                "foo",
                ScriptRunner.Run(
                    "if(true && true) { emit('foo') }"
                )
            );
        }

        [TestMethod]
        public void TestLogicalOrWorksWithIf()
        {
            Assert.AreEqual(
                "foo",
                ScriptRunner.Run(
                    "if(false || true) { emit('foo') }"
                )
            );
        }

        [TestMethod]
        public void TestLogicalAndFailsWithIfIfOneIsFalse()
        {
            Assert.AreEqual(
                "bar",
                ScriptRunner.Run(
                    "if(true && false) { emit('foo'); } else { emit('bar'); }"
                )
            );
        }

        [TestMethod]
        public void TestLogicalOrFailsWithIfIfBothAreFalse()
        {
            Assert.AreEqual(
                "bar",
                ScriptRunner.Run(
                    "if(false || false) { emit('foo'); } else { emit('bar'); }"
                )
            );
        }
    }
}
