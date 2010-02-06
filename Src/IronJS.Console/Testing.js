var y = function(a) {
    var _ = 2;
    return function() { return _ * a; };
};

var x = function() { return 1; };
var z = y(x());
var __ = function() { };
var ___ = __(1, 2, 3, 4);