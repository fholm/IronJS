
var obj = {};
obj.a = "a-org";

/*
with({}){
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
}
*/

with (obj) {
    var b;
    b = a;
    a = "a-next";
}

obj.b = b;