var name = "Eval Tests";

var tests = {
    setVarValueInside: function () {
        var x = 1;
        eval("x = 2");
        assertEqual(x, 2, "x should equal 2");
    },

    defineVarInside: function () {
        var inner = function () {
            var y = 2;
            eval("var x = 2");
            assertEqual(y, 2, "y should equal 2");
            assertEqual(x, 2, "x should equal 2");
        }

        inner();

        assertEqual(y, undefined, "y should equal undefined");
        assertEqual(x, undefined, "x should equal undefined");
    },

    defineFunctionInside: function () {
        var x = 1;
        eval("var func = function() { return x; }");
        assertEqual(func(), 1, "func should return 1");
    },

    shouldReturnLastValue: function () {
        var z = eval("var b = 1; b");
        assertEqual(z, 1, "z should equal 1");
    },

    closureHasAccessToEvalDefinedVars: function () {
        eval("var y = 1");
        var closure = function () {
            return y;
        };

        assertEqual(closure(), 1, "closure should return 1");
    }
}