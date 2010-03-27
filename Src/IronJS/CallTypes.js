
function foo(a0, a1) {
	
}

foo(closure, null, globals, 1, 2)
function foo(~closure, arguments, this, a0:int, a1:int) {
	
}

foo(closure, null, globals, 1)
function foo(~closure, arguments, this, a0:int) {
	a1:obj = undefined
}

foo(closure, null, globals, 1, 2, 3)
_:obj = 3
function foo(~closure, arguments, this, a0:int, a1:int) {
	    
}

arguments = [1, 2, 3]
arguments.callee = foo;
foo(closure, arguments, globals, 1, 2, 3)
function foo(~closure, arguments, this, a0:int, a1:int) {
	
}

//Assuming 3 is more then Func<> can handle (in reality it's 16)
function foo(a0, a1, a2) {
	return a0;
}

foo(closure, null, globals, 1, 2, 3)
function foo(~closure, arguments, this, objects[] ~args) {
	var a0 = ~args[0];
	var a1 = ~args[1];
	var a2 = ~args[2];
    return a0;
}

foo(closure, null, globals, 1, 2)
function foo(~closure, arguments, this, objects[] ~args) {
	var a0 = ~args[0];
	var a1 = ~args[1];
	var a2 = undefined;
    return a0;
}

foo(closure, null, globals, 1, 2, 3, 4)
_:obj = 4
function foo(~closure, arguments, this, objects[] ~args) {
	var a0 = ~args[0];
	var a1 = ~args[1];
	var a2 = ~args[1];
    reutrn a0;
}

arguments = [1, 2, 3, 4]
arguments.callee = foo;
foo(closure, arguments, globals, 1, 2, 3, 4)
function foo(~closure, arguments, this, objects[] ~args) {
	var a0 = ~args[0];
	var a1 = ~args[1];
	var a2 = ~args[2];
	return a0;
}