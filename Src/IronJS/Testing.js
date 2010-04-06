var obj = {};

var tst = function () {
    return "hey";
};

var foo = function (a1, a2, a3) {
    var bar = 2;
    bar = a1;
    a2 = 2;
    zoo = "hello global world";

    foo2 = function () {
        var x = bar;
        foo3 = function () {
            return a2;
        }
    }

    foo2();
};
var bar = "lol";

foo(1, 2, "lol");
foo(1, 2);
foo(1, 2);
foo(1, "lol");

bar = foo3();