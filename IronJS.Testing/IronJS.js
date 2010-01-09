
var bar = {};

bar.baz = 1;

bar.foo = function (x, y) {
    print(this.baz);
    print(x);
    print(y);
};


bar.foo(10, 20);

bar.foo.baz = {};

print(bar);

print(bar.foo);

print(bar.foo.baz);

var foo = function () {
    print("foo");
};

print(foo);
