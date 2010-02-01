
var y = function () {
    var z = 4;

    x = function () {
        for (var i = 0; i < 1000000; ++i) {
            print(z);
        }
    };

    x();
};

y();