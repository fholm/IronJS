

var lol = 1;

var foo = function (baz) {

    var bar = function (boo) {
        var daz = function () {
            print(foo);
            print(bar);
            print(daz);
            print(boo);
            print(lol);
        }
        daz();
    };

    bar(baz + 2);
};

foo(2);