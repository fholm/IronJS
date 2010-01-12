
function f() {
    print(this.x);
}

foo = { f: f, x: "foo.x", y: "foo.y" }
bar = { f: f, x: "bar.x", foo: foo }

with (foo) {

    print(bar);
    print(foo);
    print(x);

    with (bar) {

        aaa = function () {
            print("aaa: " + x);
        };

        var bbb = function () {
            print("bbb: " + foo.x);
        };

        print(y);

        f();

        print(foo.x);

        print(bar.foo.x);

    }

    f();

    x = "foo.x (with)";

    f();

}

print(foo.x);
print(bar.x);
aaa();
bbb();
