
var fooCtor = function () {

};

var barCtor = function () {

};

foo = new fooCtor();
bar = new barCtor();

println(foo instanceof fooCtor);
println(foo instanceof barCtor);
println(bar instanceof fooCtor);
println(bar instanceof barCtor);