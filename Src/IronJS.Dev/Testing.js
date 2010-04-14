
var obj = {};
obj.a = "a-org";

with (obj) {
    var foo = function(x, y) {
        var myobj = {}
        myobj.z = "lol";
        var z = 2;
        var a = "plz no";
        with(myobj) {
            z = 4; 
            return function() {
                with({}) {
                    return a;
                }
            }
        }
    }

    var b;
    b = a;
    a = "a-next";
}

obj.b = b;


var outer = {}
outer.lol = "fail";

var inner = {}
inner.lol = "ok";

a_val = foo(1, 2)();

/*
with(outer){
    with(inner){
        a_val = lol;
    }
}
*/