using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronJS.Tests
{
    [TestClass]
    public class ObjectTests
    {
        [TestMethod]
        public void TestObjectCreateNew()
        {
            Assert.AreEqual(
                "[object Object]",
                ScriptRunner.Run(
                    "emit(new Object())"
                )
            );
        }

        [TestMethod]
        public void TestObjectAssignValueToProperty()
        {
            Assert.AreEqual(
                "hello world",
                ScriptRunner.Run(
                    "foo = {}; foo.bar = 'hello world'; emit(foo.bar)"
                )
            );
        }

        [TestMethod]
        public void TestObjectAssignValueToIndex()
        {
            Assert.AreEqual(
                "hello world",
                ScriptRunner.Run(
                    "foo = {}; foo['bar'] = 'hello world'; emit(foo['bar'])"
                )
            );
        }

        [TestMethod]
        public void TestObjectAssignValueToIndexAndReadAsProperty()
        {
            Assert.AreEqual(
                "hello world",
                ScriptRunner.Run(
                    "foo = {}; foo['bar'] = 'hello world'; emit(foo.bar)"
                )
            );
        }

        [TestMethod]
        public void TestObjectAssignValueToPropertyAndReadAsIndex()
        {
            Assert.AreEqual(
                "hello world",
                ScriptRunner.Run(
                    "foo = {}; foo.bar = 'hello world'; emit(foo['bar'])"
                )
            );
        }

        [TestMethod]
        public void TestObjectAssignObjectToIndexAndAssignAndReadFromAssignedObjectUsingIndex()
        {
            Assert.AreEqual(
                "hello world",
                ScriptRunner.Run(
                    "foo = {}; foo.bar = {}; foo['bar']['boo'] = 'hello world'; emit(foo['bar']['boo']); "
                )
            );
        }

        [TestMethod]
        public void TestObjectAccessingNumberPropertiesAreOkWithBothIntAndString()
        {
            ScriptRunner.Run(
                @"
                foo = {}; 
                foo[0] = 'lol'; 
                assertEqual(foo['0'], foo[0], 'foo[\'0\'] and foo[0] should be equal');
                "
            );
        }

        [TestMethod]
        public void TestObjectNumericalIndexShouldBeSameAsStringIndex()
        {
            Assert.AreEqual(
                "barbar",
                ScriptRunner.Run(
                    "foo = {}; foo[0] = 'bar'; emit(foo[0]); emit(foo['0']); "
                )
            );
        }

        [TestMethod]
        public void TestObjectCreateNewShorthand()
        {
            Assert.AreEqual(
                "[object Object]",
                ScriptRunner.Run(
                    "emit({})"
                )
            );
        }

        [TestMethod]
        public void TestObjectCreateNewShorthandWithInlineProperties()
        {
            Assert.AreEqual(
                "hello world",
                ScriptRunner.Run(
                    "foo = { bar: 'hello', baz: 'world' };"
                    + "emit(foo.bar + ' ' + foo.baz);"
                )
            );
        }

        [TestMethod]
        public void TestObjectCreateNewShorthandWithInlinePropertiesNamesAsStrings()
        {
            Assert.AreEqual(
                "hello world",
                ScriptRunner.Run(
                    "foo = { 'bar': 'hello', \"baz\": 'world' };"
                    + "emit(foo.bar + ' ' + foo.baz);"
                )
            );
        }

        [TestMethod]
        public void TestObjectCallingObjectCreatesNewObject()
        {
            Assert.AreEqual(
                "[object Object]",
                ScriptRunner.Run(
                    "emit(Object());"
                )
            );
        }

        [TestMethod]
        public void TestObjectFunctionAssignedToObjectHasAccessToObjectAsThisParameterWhenCalled()
        {
            Assert.AreEqual(
                "hello world",
                ScriptRunner.Run(
                    "foo = { bar: function () { emit(this.boo); } };"
                    + "foo.boo = 'hello world';"
                    + "foo.bar();"
                )
            );
        }

        [TestMethod]
        public void TestObjectFunctionAssignedToTwoDifferentObjectsStillHaveAccessToCorrectObjectWhenInvokedAsMethod()
        {
            Assert.AreEqual(
                "foo1foo2",
                ScriptRunner.Run(
                    "foo1 = { }; foo2 = { }; "
                    + "bar = function () { emit(this.boo); }; "
                    + "foo1.bar = bar;"
                    + "foo2.bar = bar;"
                    + "foo1.boo = 'foo1';"
                    + "foo2.boo = 'foo2';"
                    + "foo1.bar();"
                    + "foo2.bar();"
                )
            );
        }
    }
}
