
println("--BGN--");

1;
3.14;
"foo";
true;
false;
null;

foo = 1;
foo = 1 + 1;

if (foo) {
    bar = 1;
}

if (!foo) {
    baz = 1;
} else {
    bar = 2;
}

function foo(a, b) {
    return function () {
        boo = a + b;
        println(this);
    };
};

foo(1, 2)();

println("--END--");