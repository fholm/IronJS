var obj = {};

var foo = function (a1, a2, a3) {
    return function () {
        var x = "this is a string";
        x = a1; // will fail
        a3 = "fuuuu!"
        return a2;
    };
};

var bar = foo(1, 2, "lol")();