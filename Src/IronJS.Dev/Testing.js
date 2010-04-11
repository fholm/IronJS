
var obj = {};
obj.a = "lol22";


var foo = function(a, b) {
    return function() {
        a = "foo";
        return a;
    }
}

obj.b = foo(1)();