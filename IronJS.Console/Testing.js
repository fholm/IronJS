
/*
var y = function (p) {
var a = 1;
var b = 2;
var c = 3;
var x = 2;

var x = function () {
var _a = a;
var _c = c;

var z = function () {
var __a = a;
var __b = b;
var __c = _c;
var __p = p;
};
}

return a;
};

y();
*/
var x2 = function (_) { };

x2(1);

var y = function (a) {

    var x = x2;

    for (var i = 0; i < a; ++i) {
        x(i);
        x(i);
        x(i);
        x(i);
        x(i);
        x(i);
        x(i);
        x(i);
        x(i);
        x(i);
    }


    return i;
};

y(2);

var i = 0;

time(function () {
    i = y(1000000)
});