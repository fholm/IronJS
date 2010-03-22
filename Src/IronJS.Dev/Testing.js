var y = "foo";
var z = 2;
var x = function(foo) {
    return function() {
        return foo;
    }
};

var __fooval = x(2)();