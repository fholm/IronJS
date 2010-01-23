using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronJS.Tests
{
    [TestClass]
    public class ArrayObjTests
    {
        [TestMethod]
        public void TestArrayObj()
        {
            ScriptRunner.Run(
                @"
                var foo = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'];

                assertTrue(2 in foo, 'foo should have key 2');
                assertFalse(10 in foo, 'foo should not have key 10');
                assertEqual(foo.length, 8, 'foo.length should be 8');

                var buffer = '';
                for(i = 0; i < foo.length; ++i) { buffer += foo[i]; }   
                assertEqual(buffer, 'abcdefgh', 'Buffer should be \'abcdefgh\'');

                foo[2] = undefined;
                assertTrue(2 in foo, 'foo should contain key 2 after it\'s set to undefined');

                delete foo[3];
                assertFalse(3 in foo, 'foo should not contain key 3 after it\'s deleted');
                assertEqual(foo[2], foo[3], 'foo[2] and foo[3] should both return undefined');

                foo.length = 15;
                assertEqual(foo.length, 15, 'foo.length should be 15');    
                assertFalse(14 in foo, 'foo should not have key 14');

                foo[20] = 'test';
                assertTrue(20 in foo, 'foo should have key 20');
                assertEqual(foo.length, 21, 'foo should have length 21');

                foo.length = 2;
                assertFalse(4 in foo, 'foo should not have key 4');
                assertEqual(foo.length, 2, 'foo.length should be 2');
                assertTrue(1 in foo, 'foo should have key 1');
                "
            );
        }
    }
}
