using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronJS.Tests.Builtins
{
    [TestClass]
    public class ObjectTests
    {
        //TODO: UNIT TEST for Object_ctor

        [TestMethod]
        public void TestObject_prototype()
        {
            //TODO: UNIT TEST for Object_prototype_propertyIsEnumerable

            ScriptRunner.Run(
                @"
                foo = { 
                    bar: { a: 1, b: 2 }, 
                    boo: { c: 3, d: 4 }
                };

                assertEqual(foo.valueOf(), foo, 'foo.valueOf() and foo should be equal');
                assertEqual(foo.toLocaleString(), foo.toString(), 'foo.toLocaleString() and foo.toString() should be equal');

                assertTrue(foo.hasOwnProperty('bar'), 'foo should have bar property as own');
                assertFalse(foo.hasOwnProperty('toString'), 'foo should not have toString property as own');

                bar = function() { };
                barObj = new bar();

                assertTrue(bar.prototype.isPrototypeOf(barObj), 'bar.prototype should be [[Prototype]] of barObj');
                assertEqual(barObj.constructor, bar, 'barObj.constructor should be bar');
                "
            );
        }
    }
}
