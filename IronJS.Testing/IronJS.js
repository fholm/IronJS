

var foo = function () {
    print(arguments);
    foo();
};

foo();
