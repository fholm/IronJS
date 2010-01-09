
var foo = { bar: function (lol) { print(arguments); return lol; }, baz: "hello world" };


var bar = function () {
    bar();
};

bar();

print(foo);
print(foo.bar);
print(foo.bar("wtf!?", "foo", "bar", "baz", "daz", "jazz", "zazz"));
print(foo.baz);
