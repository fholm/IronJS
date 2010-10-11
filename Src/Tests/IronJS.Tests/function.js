var name = "Function Tests";

var tests = {
    simpleReturn: function () {
        var foo = function () {
            return 2;
        };

        assertEqual(foo(), 2, "foo should return 2");
    },

    callWithArgs: function () {
        var foo = function (a, b) {
            return a;
        };

        assertEqual(foo(1, 2), 1, "foo should return 1");
    },

    hasAccessToParentScope: function () {
        var z = 1;
        var closure = function () {
            return z;
        };

        assertEqual(closure(), z, "foo should equal 1");
    },

    nestedClosures: function () {
        var outer = function (a) {
            return function () {
                return a;
            };
        };

        var inner = outer(2);
        assertEqual(inner(), 2, "inner should return 2");
    },

    callWithExactArgs: function () {

        var func = function (a, b, c) {
            assertEqual(a, 1, "b should equal 2");
            assertEqual(b, 2, "b should equal 2");
            assertEqual(c, 3, "c should equal 2");
        };

        func(1, 2, 3);
    },

    callWithToFew: function () {
        var func = function (a, b, c) {
            assertEqual(a, 1, "b should equal 2");
            assertEqual(b, 2, "b should equal 2");
            assertEqual(c, undefined, "c should equal undefined");
        };

        func(1, 2);
    },

    callWithToMany: function () {
        var func = function (a, b, c) {
            assertEqual(a, 1, "b should equal 2");
            assertEqual(b, 2, "b should equal 2");
            assertEqual(c, 3, "c should equal 2");
        };

        func(1, 2, 3, 4);
    }
}