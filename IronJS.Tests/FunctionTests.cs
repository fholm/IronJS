using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IronJS.Runtime;

namespace IronJS.Tests
{
    [TestClass]
    public class FunctionTests
    {
        [TestMethod]
        public void TestFunctionDefinition()
        {
            Assert.AreEqual(
                "function(){}",
                ScriptRunner.Run(
                    "emit(function(){});"
                )
            );
        }

        [TestMethod]
        public void TestFunctionAssignment()
        {
            Assert.AreEqual(
                "function(){}",
                ScriptRunner.Run(
                    "foo = function(){}; emit(foo);"
                )
            );
        }

        [TestMethod]
        public void TestFunctionLength()
        {
            Assert.AreEqual(
                "2",
                ScriptRunner.Run(
                    "foo = function(a, b){}; emit(foo.length);"
                )
            );
        }

        [TestMethod]
        public void TestFunctionLengthWithNoArguments()
        {
            Assert.AreEqual(
                "0",
                ScriptRunner.Run(
                    "foo = function(){}; emit(foo.length);"
                )
            );
        }

        [TestMethod]
        public void TestFunctionPrototypeConstructorIsFunction()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "foo = function(){}; emit(foo.prototype.constructor == foo);"
                )
            );
        }

        [TestMethod]
        public void TestFunctionDefineGlobalVariable()
        {
            Assert.AreEqual(
                "123",
                ScriptRunner.Run(
                    "foo = function(){ bar = 123; };" +
                    "foo();" +
                    "emit(bar);"
                )
            );
        }

        [TestMethod]
        public void TestFunctionDefineLocalVariable()
        {
            try
            {
                ScriptRunner.Run(
                    "foo = function(){ var bar = 123; };" +
                    "foo();" +
                    "emit(bar);"
                );

                Assert.Fail("Should throw InternalRuntimeError");
            }
            catch (InternalRuntimeError)
            {

            }
        }

        [TestMethod]
        public void TestFunctionReturn()
        {
            Assert.AreEqual(
                "123",
                ScriptRunner.Run(
                    "foo = function(){ return 123; };" +
                    "emit(foo());"
                )
            );
        }

        [TestMethod]
        public void TestFunctionIsClosure()
        {
            Assert.AreEqual(
                "123",
                ScriptRunner.Run(
                    "bar = 123; " + 
                    "foo = function(){ emit(bar); };" +
                    "foo();"
                )
            );
        }

        [TestMethod]
        public void TestFunctionNestedFunctionIsClosureOfParent()
        {
            Assert.AreEqual(
                "123",
                ScriptRunner.Run(
                    "foo = function(bar){ var nested = function() { emit(bar); }; nested(); };" +
                    "foo(123);"
                )
            );
        }

        [TestMethod]
        public void TestFunctionReturnClosureAndCall()
        {
            Assert.AreEqual(
                "123",
                ScriptRunner.Run(
                    "foo = function(bar){ return function() { emit(bar); }; };" +
                    "foo(123)();"
                )
            );
        }

        [TestMethod]
        public void TestFunctionDefineAndCallInOneStatement()
        {
            Assert.AreEqual(
                "123",
                ScriptRunner.Run(
                    "(function(bar){ emit(bar); })(123);"
                )
            );
        }

        [TestMethod]
        public void TestFunctionHasObjectAsThis()
        {
            Assert.AreEqual(
                "[object Object]",
                ScriptRunner.Run(
                    "foo = function(){ emit(this) };" +
                    "foo();"
                )
            );
        }

        [TestMethod]
        public void TestFunctionHasGlobalObjectAsThis()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "foo = function(){ emit(this == globals) };" +
                    "foo();"
                )
            );
        }

        [TestMethod]
        public void TestFunctionHasAccessToArgumentsObject()
        {
            Assert.AreEqual(
                "[object Object]",
                ScriptRunner.Run(
                    "foo = function(){ emit(arguments) };" +
                    "foo();"
                )
            );
        }

        [TestMethod]
        public void TestFunctionHasAccessToArgumentsObjectAndItHasALength()
        {
            Assert.AreEqual(
                "3",
                ScriptRunner.Run(
                    "foo = function(){ emit(arguments.length) };" +
                    "foo(1,2,3);"
                )
            );
        }

        [TestMethod]
        public void TestFunctionArgumentsObjectsCalleePropertyIsTheSameAsFunction()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "foo = function(){ emit(arguments.callee == foo) };" +
                    "foo();"
                )
            );
        }

        [TestMethod]
        public void TestFunctionLambdaFunctionsHaveAccessToCallee()
        {
            Assert.AreEqual(
                "function",
                ScriptRunner.Run(
                    "(function(){ emit(typeof arguments.callee); })();"
                )
            );
        }
    }
}
