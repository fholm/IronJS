

var foo = function (baz) {

    var bar = function (boo) {
        var daz = function () {
            foo(boo + boo);
        }
        daz();
    };

    bar(baz + 2);
};

foo(2);