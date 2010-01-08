

var foo = function (baz) {

    var bar = function () {
        print(foo);
        print(baz);
    };

    bar();
};

foo(1337);