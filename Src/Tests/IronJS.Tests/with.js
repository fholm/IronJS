var name = "With Tests";

var tests = {
    simpleWith: function () {
        var x = 2;
        with ({ x: 1 }) {
            assertEqual(x, 1, "x should equal 1");
        }
    },

    nestedWith: function () {
        with ({ x: "outerX", y: "outerY" }) {
            with ({ x: "innerX" }) {
                assertEqual(x, "innerX", "x should equal 'innerX'");
                assertEqual(y, "outerY", "y should equal 'outerY'");
            }
        }
    },

    withScopeShouldBeClosedOver: function () {
        var x = "outerX";
        with ({ x: "withX" }) {
            var y = function () { return x; };
        }
        assertEqual(y(), "withX", "y should return 'withX'");
    }
}