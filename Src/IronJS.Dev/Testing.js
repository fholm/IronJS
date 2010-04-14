
var obj = {};
obj.a = "a-org";

with (obj) {
    var foo = function(x, y) {
        var myobj = {}
        myobj.a = "lol";
        var z = 2;
        var a = "plz no";
        with(myobj) {
            z = 4; 
            return function() {
                return a;
            }
        }
    }

    var b;
    b = a;
    a = "a-next";
}

obj.b = b;
a_val = foo(1, 2)();