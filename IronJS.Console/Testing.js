
var foo = 2

var x1 = function () {

    var y = foo;
    var a = 3;

    var x2 = function (z) {
        bar = a;
        return function (x) {
            return y * z * x * foo;
        };
    };

    return x2(2);
};

x()(2);


// --------------


tbl[0] = function (x, clos) {
    return y * z * x * foo;
};

tbl[1] = function (z, clos) {
    bar = clos.i1;
    return func(tbl[0], closure());
};

tbl[2] = function () {
    var y = cell(foo);
    var a = cell(3);
    var x2 = func(tbl[1], closure(a));
    return x2(2);
};

var foo = 2
var x = func(tbl[2], empty);
x()(2);
