using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronJS.Tests
{
    [TestClass]
    public class ConstructorTests
    {
        [TestMethod]
        public void TestConstructorCreateInstance()
        {
            Assert.AreEqual(
                "[object Object]",
                ScriptRunner.Run(
                    "foo = function() { };"
                    + "emit(new foo())"
                )
            );
        }

        [TestMethod]
        public void TestConstructorHasNewObjectAsThis()
        {
            Assert.AreEqual(
                "[object Object]",
                ScriptRunner.Run(
                    "foo = function() { emit(this); };"
                    + "new foo();"
                )
            );
        }

        [TestMethod]
        public void TestConstructorThisParameterIsNotGlobal()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "foo = function() { emit(this == globals); };"
                    + "new foo();"
                )
            );
        }

        [TestMethod]
        public void TestConstructorCanAssignPropertiesToNewObjectAsThisParameterInsideConstructor()
        {
            Assert.AreEqual(
                "hello world",
                ScriptRunner.Run(
                    "foo = function() { this.bar = 'hello world'; };"
                    + "foo1 = new foo();"
                    + "emit(foo1.bar)"
                )
            );
        }

        [TestMethod]
        public void TestConstructorInstanceHasAccessToPrototype()
        {
            Assert.AreEqual(
                "hello world",
                ScriptRunner.Run(
                    "foo = function() { };"
                    + "foo.prototype.bar = 'hello world'"
                    + "foo1 = new foo();"
                    + "emit(foo1.bar)"
                )
            );
        }

        [TestMethod]
        public void TestConstructorTwoInstancesHaveAccessToTheSamePrototype()
        {
            Assert.AreEqual(
                "1122",
                ScriptRunner.Run(
                    "foo = function() { };"
                    + "foo.prototype.bar = '1'"
                    + "foo1 = new foo();"
                    + "foo2 = new foo();"
                    + "emit(foo1.bar + foo2.bar);"
                    + "foo1.bar = '2';"
                    + "emit(foo1.bar + foo2.bar);"
                )
            );
        }
    }
}
