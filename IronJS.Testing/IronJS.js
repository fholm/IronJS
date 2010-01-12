

function foo_bar_baz(as) {
    this.as = as;
    this.built = "from baz";
};

function foo_bar(as) {
    this.built = "from bar";
    this.as = as;
    return { baz: foo_bar_baz };
};

foo = {}
foo.bar = foo_bar;
foo.bar.baz = foo_bar_baz;

var obj = new (foo.bar().baz)("hello world");
print(obj.built);
print(obj.as);

var obj2 = new foo.bar.baz("lol");
print(obj2.built);
print(obj2.as);


/*

var foo = new foo.bar.baz("hello world");

var bar = new (foo.bar().baz)("hello world");

*/