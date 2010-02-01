
var y = function () {

    var a = 1;
    var b = 2;
    var c = 3;

    var x = function () {
        var _a = a;
        var _c = c;

        var z = function () {
            var __a = a;
            var __b = b;
            var __c = _c;
        };
    }

    return a;
};

y();