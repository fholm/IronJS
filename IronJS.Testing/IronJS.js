


/*
function f() {
    alert(this.x);
}

foo = { f: f, x: "foo.x", y: "foo.y" }
bar = { f: f, x: "bar.x" }

with (foo) {

    alert(bar);
    alert(foo);
    alert(x);

    with (bar) {

        aaa = function () {
            alert("aaa: " + x);
        };

        var bbb = function () {
            alert("bbb: " + foo.x);
        };

        alert(y);

        f();

    }

    f();

    x = "foo.x (with)";

    f();

}

alert(foo.x);
alert(bar.x);
aaa();
bbb();
*/