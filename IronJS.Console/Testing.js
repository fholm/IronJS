
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
var x2 = function () { };

x2();

var y = function (a) {

    var x = x2;

    for (var i = 0; i < a; ++i) {
        x();
    }
};

y(2);

time(function () {
    y(10000000)
});