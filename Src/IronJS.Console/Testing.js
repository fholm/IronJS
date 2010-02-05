
var y = function (a) {

    var x = function(_) { };

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
};

time(function () {
    y(1000000);
});