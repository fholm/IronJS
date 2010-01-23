using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronJS.Tests.Builtins
{
    [TestClass]
    public class ArrayTests
    {
        [TestMethod]
        public void TestArray_ctor()
        {
            ScriptRunner.Run(
                @"
                var boo = Array(2);
                assertEqual(2, boo.length, 'boo.length should be 2');

                var baz = new Array(3);
                assertEqual(3, baz.length, 'baz.length should be 3');

                var foo = Array('x', 'y', 'z');
                buffer = '';
                for(i = 0; i < foo.length; ++i) { buffer += foo[i]; }
                assertEqual('xyz', buffer, 'buffer should equal \'xyz\'');

                var bar = new Array('a', 'b', 'c');
                buffer = '';
                for(i = 0; i < bar.length; ++i) { buffer += bar[i]; }
                assertEqual('abc', buffer, 'buffer should equal \'abc\'');
                "
            );
        }

        [TestMethod]
        public void TestArray_prototype_concat()
        {
            ScriptRunner.Run(
                @"
                foo = [1, 2, 3];
                bar = foo.concat(4, [5, 6, 7], 'foo', 9);

                buffer = '';
                for(i = 0; i < bar.length; ++i) {
                    buffer += bar[i];
                }

                assertEqual('1234567foo9', buffer, 'buffer should equal \'1234567foo9\'');
                assertFalse(foo == bar, 'foo should not equal bar');
                assertEqual(9, bar.length, 'bar.length should be 9');
                assertTrue(8 in bar, 'bar should have key 8');
                assertFalse(9 in bar, 'bar should nto have key 9');
                "
            );
        }

        [TestMethod]
        public void TestArray_prototype_join()
        {
            ScriptRunner.Run(
                @"
                foo = ['a', 'b', 'c', 1, 2, 3];

                assertEqual('a,b,c,1,2,3', foo.join(','), 'foo.join(,) should equal a,b,c,1,2,3');
                assertEqual(foo.join(','), foo.join(), 'foo.join(,) should equal foo.join()');
                assertEqual('a--b--c--1--2--3', foo.join('--'), 'foo.join(--) should equal a--b--c--1--2--3');
                "
            );
        }

        [TestMethod]
        public void TestArray_prototype_toString()
        {
            ScriptRunner.Run(
                @"
                foo = ['a', 'b', 'c', 1, 2, 3];

                assertEqual('a,b,c,1,2,3', foo.toString(), 'foo.toString() should equal a,b,c,1,2,3');
                "
            );
        }

        [TestMethod]
        public void TestArray_prototype_toLocaleString()
        {
            ScriptRunner.Run(
                @"
                foo = ['a', 'b', 'c', 1, 2, 3];

                assertEqual('a,b,c,1,2,3', foo.toLocaleString(), 'foo.toLocaleString() should equal a,b,c,1,2,3');
                "
            );
        }

        [TestMethod]
        public void TestArray_prototype_pop()
        {
            ScriptRunner.Run(
                @"
                foo = ['a', 'b', 'c'];

                assertEqual(3, foo.length, 'foo.length is 3');

                assertEqual('c', foo.pop(), 'foo.pop() returns c');
                assertEqual(2, foo.length, 'foo.length is 2');

                assertEqual('b', foo.pop(), 'foo.pop() returns b');
                assertEqual(1, foo.length, 'foo.length is 1');

                assertEqual('a', foo.pop(), 'foo.pop() returns a');
                assertEqual(0, foo.length, 'foo.length is 0');

                assertEqual(undefined, foo.pop(), 'foo.pop() returns undefined');
                assertEqual(0, foo.length, 'foo.length is 0');
                "
            );
        }

        [TestMethod]
        public void TestArray_prototype_push()
        {
            ScriptRunner.Run(
                @"
                foo = ['a', 'b', 'c'];

                assertEqual(3, foo.length, 'foo.length is 3');
                
                foo.push('d');
                assertEqual(4, foo.length, 'foo.length is 4');
                
                foo.push('e', 'f');
                assertEqual(6, foo.length, 'foo.length is 6');

                assertEqual('abcdef', foo.join(''), 'foo.join(\'\') is abcdef');
                "
            );
        }

        [TestMethod]
        public void TestArray_prototype_reverse()
        {
            ScriptRunner.Run(
                @"
                foo = ['a', 'b', 'c', 'd'];

                original = foo.join('');
                assertEqual('abcd', original, 'original should equal abcd');

                foo.reverse();
                reversed = foo.join('');
                assertEqual('dcba', reversed, 'reversed should equal dcba');

                foo.reverse();
                rev_rev = foo.join('');
                assertEqual(original, rev_rev, 'original should equal rev_rev');

                foo[10] = 'e';
                assertEqual('edcba', foo.reverse().join(''), 'foo.reverse().join('') should equal edcba');
                assertEqual('abcde', foo.reverse().join(''), 'foo.reverse().join('') should equal abcde');

                assertEqual(foo, foo.reverse(), 'foo.reverse() should return foo');
                "
            );
        }

        [TestMethod]
        public void TestArray_prototype_shift()
        {
            ScriptRunner.Run(
                @"
                foo = ['a', 'b', 'c', 'd'];
                foo[10] = 'e';

                assertEqual('a', foo.shift(), 'foo.shift() should equal a');
                assertEqual(10, foo.length, 'foo.lenght should equal 10');

                assertEqual('b', foo.shift(), 'foo.shift() should equal b');
                assertEqual(9, foo.length, 'foo.lenght should equal 9');

                assertEqual('c', foo.shift(), 'foo.shift() should equal c');
                assertEqual(8, foo.length, 'foo.lenght should equal 8');

                assertEqual('d', foo.shift(), 'foo.shift() should equal d');
                assertEqual(7, foo.length, 'foo.lenght should equal 7');

                assertEqual(undefined, foo.shift(), 'foo.shift() should equal undefined');
                assertEqual(6, foo.length, 'foo.lenght should equal 6');

                assertEqual(undefined, foo.shift(), 'foo.shift() should equal undefined');
                assertEqual(5, foo.length, 'foo.lenght should equal 5');

                assertEqual('e', foo[4], 'foo[4] should equal e');
                "
            );
        }

        [TestMethod]
        public void TestArray_prototype_slice()
        {
            ScriptRunner.Run(
                @"
                foo = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];

                assertEqual('2,3', foo.slice(2, 4).toString(), 'foo.slice() should equal 2,3');
                assertEqual('5,6,7,8', foo.slice(-5, 9).toString(), 'foo.slice() should equal 5,6,7,8');
                assertEqual('5,6,7', foo.slice(-5, -2).toString(), 'foo.slice() should equal 5,6,7');
                assertEqual('2,3,4,5,6,7', foo.slice(2, -2).toString(), 'foo.slice() should equal 2,3,4,5,6,7');
                "
            );
        }

        [TestMethod]
        public void TestArray_prototype_sort()
        {
            ScriptRunner.Run(
                @"
                foo = [4, 5, 7, 7, 1, 23, 2, 1, 2, 52, 232];
                foo.sort(function (a, b) { return a - b; });
                assertEqual('1,1,2,2,4,5,7,7,23,52,232', foo.toString(), 'foo.toString() should equal 1,1,2,2,4,5,7,7,23,52,232');

                foo = [4, 5, 7, 7, 1, 23, 2, 1, 2, 52, 232];
                foo.sort();
                assertEqual('1,1,2,2,4,5,7,7,23,52,232', foo.toString(), 'foo.toString() should equal 1,1,2,2,4,5,7,7,23,52,232');

                bar = ['aa', 'bbb', 'ddddd', 'cccc', 'fffffff', 'eeeee'];
                bar.sort(function (a, b) { return a.length - b.length; });
                assertEqual('aa,bbb,cccc,ddddd,eeeeee,fffffff', bar.toString(), 'bar.toString() should equal aa,bbb,cccc,ddddd,eeeeee,fffffff');
                "
            );
        }



    }
}
