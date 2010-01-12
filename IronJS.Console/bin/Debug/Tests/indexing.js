
foo = { bar: { x: "x" }, y: "y" };

print(foo);
print(foo["y"]);
print(foo["bar"]);
print(foo["bar"]["x"]);
print(foo.bar["x"]);


foo["bar"]["x"] = "new x";
print(foo["bar"]["x"]);

foo["bar"] = { hello: "world" };
print(foo["bar"].hello);
