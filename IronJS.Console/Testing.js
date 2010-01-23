foo = [1, 2, 3];
bar = foo.concat(4, [5, 6, 7], 'foo', 9);

buffer = '';
println(bar.length);
for(i = 0; i < bar.length, ++i) {
    buffer += bar[i];
}
