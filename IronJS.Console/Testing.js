

foo = ['a', 'b', 'c', 'd'];
foo[10] = 'e';

assertEqual('a', foo.shift());
assertEqual(10, foo.length);

assertEqual('b', foo.shift());
assertEqual(9, foo.length);

assertEqual('c', foo.shift());
assertEqual(8, foo.length);

assertEqual('d', foo.shift());
assertEqual(7, foo.length);

assertEqual(undefined, foo.shift());
assertEqual(6, foo.length);

assertEqual(undefined, foo.shift());
assertEqual(5, foo.length);

assertEqual('e', foo[4]);
