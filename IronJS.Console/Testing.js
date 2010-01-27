
var lambda = function () {

};

function foo(func) {
    println(typeof func);
};

foo(function bar() { println("test"); });

bar();

foo(function () {});

/*
var foo = { baz: function () { this.bar = function () { println("test"); } } };

var test = new a.b.c.d
*/