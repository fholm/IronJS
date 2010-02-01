
var y = function () {
    var z = 4;

    x = function () {

        var r = 2;

        y = function () {
            return r;
        };

        y();
        return z;
    };

    x();
};

y();