
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