var foo = function(a1, a2, a3) {
    var bar = 2;
    bar = a1;
    a2 = 2;
    var foo = function() {
        var x = bar;
        var y = a2;
    }
};
var bar = "lol";

foo(1, 2);