

var foo = [1, 2, 3, 4];

println(2 in foo);

foo.length = 1;

println(2 in foo);

foo[5] = 6;

println(2 in foo);
println(foo.length);
println(foo[2]);

for (i in foo) {
    println(i);
}