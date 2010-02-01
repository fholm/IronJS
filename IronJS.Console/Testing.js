
var y = function () {
    var z = 4;

    x = function () {

        var r = 2;

        y2 = function () {
            return r;
        };

        y2();
        return z;
    };

    x();
};

y();