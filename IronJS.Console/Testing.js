
var y = function () {
    var z = 4;
    
    return function () {
        return z;
    };
};

var x = y();
var z = x();