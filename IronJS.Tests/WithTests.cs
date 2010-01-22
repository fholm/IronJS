using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronJS.Tests
{
    [TestClass]
    public class WithTests
    {
        [TestMethod]
        public void TestWithScopeChanges()
        {
            Assert.AreEqual(
                "hello world",
                ScriptRunner.Run(
                    "foo = { bar: 'hello world' }; with(foo) { emit(bar); }"
                )
            );
        }

        [TestMethod]
        public void TestWithPropertiesChangeOnOriginalObject()
        {
            Assert.AreEqual(
                "12",
                ScriptRunner.Run(
                    "foo = { bar: 1 }; with(foo) { emit(bar); bar = 2; } emit(foo.bar);"
                )
            );
        }

        [TestMethod]
        public void TestWithStillHaveAccessToOriginalObjectUnderRealName()
        {
            Assert.AreEqual(
                "1",
                ScriptRunner.Run(
                    "foo = { bar: 1 }; with(foo) { emit(foo.bar); }"
                )
            );
        }

        [TestMethod]
        public void TestWithCanNotDefineNewPropertiesOnObjectTheyAreDefinedInClosestNormalScope()
        {
            Assert.AreEqual(
                "1undefined2",
                ScriptRunner.Run(
                    "foo = { bar: 1 }; with(foo) { boo = 2; }; emit(foo.bar); emit(foo.boo); emit(boo);"
                )
            );
        }

        [TestMethod]
        public void TestWithNestedWithHasAccessToCorrectProperties()
        {
            Assert.AreEqual(
                "3123",
                ScriptRunner.Run(
                    "foo = { bar: { baz: 1, foo: 2 }, baz: 3 }; with(foo) { emit(baz); with(bar) { emit(baz); emit(foo); } emit(baz); }"
                )
            );
        }

        [TestMethod]
        public void TestWithFunctionsCalledOnScopeHaveAccessToThisObjectAsOriginalObject()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "foo = { bar: function() { emit(this == foo); } }; with(foo) { bar(); }"
                )
            );
        }

        [TestMethod]
        public void TestWithFunctionsCallsNestedInsideMultipleWithStatemntsGetCorectObjectAsThisParameter()
        {
            Assert.AreEqual(
                "truetruetruetrue",
                ScriptRunner.Run(
                    "foo = { bar: { "
                    + "bar_func: function() { emit(this == foo.bar); } }, "
                    + "foo_func: function() { emit(this == foo); } }; "
                    + "with(foo) { foo_func(); with(bar) { bar_func(); foo_func(); } foo_func(); }"
                )
            );
        }

        [TestMethod]
        public void TestWithNormalFunctionsCallsInsideWithStmtStillHaveGlobalsAsThisParameter()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "foo = { }; "
                    + "with(foo) { (function(){ emit(this == globals); })(); }"
                )
            );
        }
    }
}
