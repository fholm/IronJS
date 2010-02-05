var y = function (a) {

    var x = function() { };

    for (var i = 0; i < a; ++i) {
        x();
        x();
        x();
        x();
        x();
        x();
        x();
        x();
        x();
        x();
    }
};

time(function () {
    y(1000000);
});