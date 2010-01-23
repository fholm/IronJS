using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronJS.Tests.Builtins
{
    [TestClass]
    public class FunctionTests
    {
        //TODO: UNIT TEST for Function_prototype_toString
        //TODO: UNIT TEST for Function_prototype_apply
        //TODO: UNIT TEST for Function_ctor

        [TestMethod]
        public void TestFunction_prototype_call()
        {
            ScriptRunner.Run(
                @"
                buffer = '';

                foo = function(a) {
                    buffer += this.x;
                    buffer += a
                };

                foo.call({x: 1}, 2);
                foo.call({x: 3}, 4);

                assertEqual(buffer, '1234', 'Buffer should equal 1234');

                foo2 = function() {
                    assertEqual(this, globals, 'this should equal globals');
                };

                foo2.call();
                "
            );
        }
    }
}
