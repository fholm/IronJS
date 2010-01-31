
var y = function () {
    var x = function () {

    };

    for (var i = 0; i < 1000000; ++i) {
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
    y();
});

//x();

/*function y() {
    function x() {
        
    };

    for (var i = 0; i < 1000000; ++i) {
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
}

time(function () {
    y();
});
*/