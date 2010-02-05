
var y = function() {
    var z = 2;
    var x = function() { return z; };
    return x;
};

y();