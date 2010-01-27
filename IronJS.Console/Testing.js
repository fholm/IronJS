var x = new foo; // new(foo)
var x = new foo(); // CALL(new(foo), ARGS)

var x = new foo.bar; // BYFIELD(new(foo), bar)
var x = new foo.bar(); // CALL(BYFIELD(new(foo), bar), ARGS)

var x = new foo["bar"]; // BYINDEX(new(foo), bar)
var x = new foo["bar"](); // CALL(BYINDEX(new(foo), bar), ARGS)

var x = (new foo).func(); // CALL(BYFIELD(PAREXPR(new(foo)), func), ARGS)
var x = (new foo()).func(); // CALL(BYFIELD(CALL(new(foo), ARGS), func), ARGS)

var x = new foo().func(); // CALL(BYFIELD(CALL(new(foo), ARGS), func), ARGS)

var x = (new foo.bar).func(); // CALL(BYFIELD(PAREXPR(BYFIELD(new(foo), bar)), func), ARGS)
var x = (new foo.bar()).func(); // CALL(BYFIELD(CALL(BYFIELD(new(foo), bar), ARGS), func), ARGS)

var x = (new foo["bar"]).func(); // CALL(BYFIELD(PAREXPR(BYINDEX(new(foo), 'bar')), func), ARGS)
var x = (new foo["bar"]()).func(); // CALL(BYFIELD(CALL(BYINDEX(new(foo), 'bar'), ARGS), func), ARGS)

//var foo = { baz: function () { this.bar = function () { println("test"); } } };
//var test = new a.b.c.d
