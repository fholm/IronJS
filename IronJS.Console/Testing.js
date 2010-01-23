
println("abc".length);
println("abc".toString());
println("abc".valueOf());
println("abc".charAt(1));
println("abc".charCodeAt(1));
println("a".concat("b", "c"));
println("abcdefg".indexOf("cde"));
println("aacdaacdaa".lastIndexOf("cd"));

bar = ['aa', 'bbb', 'ddddd', 'cccc', 'fffffff', 'eeeee'];
bar.sort(function (a, b) { println(a.length - b.length); return a.length - b.length; });
println(bar.toString());