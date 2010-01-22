boo = function () { println("boo.this == globals: " + (this == globals)); }

foo = {
    bar: {
        bar_func: function () { println("foo.bar.bar_func.this == foo.bar: " + (this == foo.bar));  }
    },
    foo_func: function () { println("foo.foo_func.this == this: " + (this == foo)); }
};

with (foo) {
    boo();
}

boo();