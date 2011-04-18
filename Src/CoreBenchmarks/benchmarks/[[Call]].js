function benchmark() {
    function foo() { };
    for (var i = 0; i < 1000000; ++i) {
        foo();
        foo(1);
        foo(1, 2);
        foo(1, 2, 3);
        foo(1, 2, 3, 4);
        foo(1, 2, 3, 4, 5);

        foo();
        foo(1);
        foo(1, 2);
        foo(1, 2, 3);
        foo(1, 2, 3, 4);
        foo(1, 2, 3, 4, 5);
    }
};