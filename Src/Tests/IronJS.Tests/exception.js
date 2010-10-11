var name = "Exception Tests";

var tests = {
    simpleTryCatch: function () {
        try {
            throw 1;

        } catch (x) {
            assertEqual(x, 1, "x should equal 1");
        }
    },

    simpleTryCatchFinally: function () {
        try {
            throw 1;

        } catch (x) {
            var y = 2;
            assertEqual(x, 1, "x should equal 1");

        } finally {
            assertEqual(y, 2, "y should equal 1");
            assertEqual(x, undefined, "x should be undefined");
        }
    },

    nestedTryCatch: function () {
        try {
            throw 1;

        } catch (x) {
            try {
                throw 2;

            } catch (y) {
                assertEqual(x, 1, "x should equal 1");
                assertEqual(y, 2, "y should equal 1");
            }
        }
    },

    catchVariableCanBeClosedOver: function () {
        try {
            throw 1;
        } catch (x) {
            var bar = function () { return x; }
        }

        assertEqual(bar(), 1, "bar should equal 1");
    },

    nesedCatchVariablesCanBeClosedOver: function () {
        try {
            throw 1;

        } catch (x) {
            try {
                throw 2;

            } catch (y) {
                var foo = function () { return x; }
                var bar = function () { return y; }
            }
        }

        assertEqual(foo(), 1, "foo should return 1");
        assertEqual(bar(), 2, "bar should return 2");
    },

    catchScopeHasAccessToClosureEnvironment: function () {
        var outer = function (a) {
            return function () {
                try {
                    throw 2;
                } catch (b) {
                    assertEqual(a, 1, "a should equal 1");
                    assertEqual(b, 2, "b should equal 2");
                }
            };
        };

        var inner = outer(1);
        inner();
    }
}