var bench = function () {
    function f(x, y, z) {
        return x + y + z;
    }

    var y = 2;

    for (var i = 0; i < 2500000; ++i) {
        y = f(1, 2, 3);
    }
}

bench = function () {
    function f(x0, x1, x2, x3, x4, x5, x6, x7, x8, x9) {

    }

    for (var i = 0; i < 3000000; ++i)
        f();
}

/*
var bench = function () {
var foo = function (a, b) {
return function () {
return a;
}
}
return foo()();
}
*/

bench();