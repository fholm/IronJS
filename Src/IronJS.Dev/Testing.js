/*
var bench = function () {
    function f(x, y, z) {
        return x + y + z;
    }

    var y = 2;

    for (var i = 0; i < 2500000; ++i) {
        y = f(1, 2, 3);
    }
}
*/

var bench = function () {
    function f() {
        function g() { }
    }

    for (var i = 0; i < 300000; ++i)
        f();
}

/*
var bench = function () {
var foo = function (a, b) {
return function () {
return function () {
return a;
}
}
}
return foo()()();
}
*/

bench();