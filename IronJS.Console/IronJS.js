/*
_outer:
for (i = 0; i < 10; ++i) {

    if (i == 5)
        break _outer;

    print("iter: " + i);
    
    _inner:
    for (j = 0; j < 10; ++j) {
    
        if (j == 5)
            continue _outer;

        print(j);
    }
}
*/

foo = function (bar) {
    this.bar = bar;
}

foo.prototype.hello = "world";

var test = new foo("test");

print(test.hello);