using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IronJS.Runtime;

namespace IronJS.Tests
{
    [TestClass]
    public class DeleteInTests
    {
        [TestMethod]
        public void TestDeleteRemovesProperty()
        {
            Assert.AreEqual(
                "1undefined",
                ScriptRunner.Run(
                    "foo = { bar: 1 }; emit(foo.bar); delete foo.bar; emit(foo.bar);"
                )
            );
        }

        [TestMethod]
        public void TestDeleteRemovesIndex()
        {
            Assert.AreEqual(
                "1undefined",
                ScriptRunner.Run(
                    "foo = { bar: 1 }; emit(foo['bar']); delete foo['bar']; emit(foo['bar']);"
                )
            );
        }

        [TestMethod]
        public void TestDeleteRemovesGlobalVariable()
        {
            try
            {
                ScriptRunner.Run("foo = 1; delete foo; emit(foo);");
                Assert.Fail("Should throw InternalRuntimeError");
            }
            catch (InternalRuntimeError)
            {

            }
        }

        [TestMethod]
        public void TestDeleteShouldNotRemoveLocalVariable()
        {
            try
            {
                ScriptRunner.Run("var foo = 1; delete foo; emit(foo);");
            }
            catch (InternalRuntimeError)
            {
                Assert.Fail("Should not throw InternalRuntimeError");
            }
        }

        [TestMethod]
        public void TestInDeleteMakesInReturnFalse()
        {
            Assert.AreEqual(
                "truefalse",
                ScriptRunner.Run(
                    "foo = { bar: 'val' }; emit('bar' in foo); delete foo.bar; emit('bar' in foo);"
                )
            );
        }

        [TestMethod]
        public void TestInSettingUndefinedStillReturnsTrue()
        {
            Assert.AreEqual(
                "truetrue",
                ScriptRunner.Run(
                    "foo = { bar: 'val' }; emit('bar' in foo); foo.bar = undefined; emit('bar' in foo);"
                )
            );
        }
    }
}
