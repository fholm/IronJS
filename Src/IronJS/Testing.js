
var obj = {};
obj.a = "lol-obj";

var bar = function () {
    
}

with(obj) {

    var foo = function () {   

        return function() {
            
        }


    }

}


trace_unit = { bar: 0, i: 0 }

var bar = {}

for(var i = 0; i < 10; ++i) {
    
    if(i == 5) {
        bar.[i] = "lol"
    } else {
        bar = "bajs";
    }

}