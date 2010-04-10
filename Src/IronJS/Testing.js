
var obj = {};
obj.a = "lol-obj";
a = "lol";

with(obj) {

    
    var b = a;
    a = 2;

}

var obj_a = obj.a;