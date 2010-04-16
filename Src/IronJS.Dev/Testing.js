
var foo = function () {
    var x = 2;

    var bar = function () {
        x = "asd";
        return x;
    }

    return bar();
}

a_val = foo();