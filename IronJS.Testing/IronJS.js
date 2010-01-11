


foo = { bar: "foo.bar", boo: "foo.boo" }

with (foo) {

    bar = "foo.bar (with)";
    print(boo);
    yar = "yar (with)";
    var lol = "lol (var-with)");

}

print(foo.bar);
print(foo.boo);
print(yar);
print(lol);