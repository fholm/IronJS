var name = "Break/Continue/Label Tests";

var tests = {
    simpleBreak: function () {
        var x = 1;
        while (true) {
            x = x + 1;
            break;
        }
        assertEqual(x, 2, "x should equal 2");
    },

    simpleContinue: function () {
        var x = 1;
        while (true) {
            x = x + 1;
            if (x < 3) continue;
            break;
        }
        assertEqual(x, 3, "x should equal 2");
    },

    nestedBreaks: function () {
        var x = 1;

        while (true) {
            x = x + 1;

            while (true) {
                x = x + 1;
                break;
            }

            break;
        }

        assertEqual(x, 3, "x should equal 3");
    },

    nestedLabeledBreaks: function () {
        var x = 1;

        outer: while (true) {
            x = x + 1;

            inner: while (true) {
                x = x + 1;
                break inner;
            }

            break outer;
        }

        assertEqual(x, 3, "x should equal 3");
    },

    nestedLabeledBreaksBreakingAllFromInner: function () {
        var x = 1;

        outer: while (true) {
            x = x + 1;

            inner: while (true) {
                x = x + 1;
                break outer;
            }
        }

        assertEqual(x, 3, "x should equal 3");
    },

    breakingNonLoops: function () {
        var x = 1;

        foo: if (true) {
            x = 2;
            break foo;
            x = 3;
        }

        assertEqual(x, 2, "x should equal 3");
    }
};