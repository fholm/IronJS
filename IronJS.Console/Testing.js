
foo = { bar: 1 };

with (foo) {
    bar = 2;
    baz = 3;
}

println(foo.bar);
println(foo.baz);