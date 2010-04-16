
var foo = function () {
    var x = 2;

    var bar = function () {
        x = "asd";
        return x;
    }

    return bar();
}

a_val = foo();

/*
obj.b = b;
a_val = foo(1, 2);
*/

/*
var outer = {}
outer.lol = "fail";
var inner = {}
inner.lol = "ok";
with(outer){
    with(inner){
        a_val = lol;
    }
}
*/

/*
var x = { foo: 1 }
var y = { bar: 2 }
y.Prototype = x

cell.id = -1
cell.index = -1
cell.resolver = null

if(cell.id == x.id) {
	x.properties.[cell.index]
} else if(cell.resolver != null) {
    tmp = cell.resolver(x)
    if(pair.Item1) {
        
    } else {
        
    }
} else {
	
}

x.chain.[cell.level].id = cell.id
*/