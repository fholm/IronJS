
var y = function () {
    var z = 4;

    x = function () {

        var r = 2;

        a = function () {
            return r;
        };

        a();
        return z;
    };

    x();
};

y();