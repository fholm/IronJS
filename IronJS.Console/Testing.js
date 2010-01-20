
var foo = function (a) {
    this.bar = a;
};

foo_inst = new foo("hello world");

println(foo_inst.bar);

Object();