
var obj = {};
obj.a = "lol22";


var foo = function(a, b) {
    var x = 1;
    var y;
    with({}) {
        y = 2;
        return function() {
            a = "foo";
            return a;
        }
    }
}

obj.b = foo(1, "foooooo", {})();