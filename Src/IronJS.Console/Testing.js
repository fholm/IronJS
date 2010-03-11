var b = function(c) {
    print(2);
    var d = 1;
    var x = c;
};

b(2);



var foo = function(x) {

    var bar = function(y) {
        return y * x;
    };

    return bar(x);

};

var zaz = foo(2);