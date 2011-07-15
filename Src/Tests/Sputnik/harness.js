function SputnikError(message) {
    this.name = "SputnikError";
    this.message = (message || "");
}
SputnikError.prototype = Error.prototype;

var finished = false;

function testFinished() {
    if (finished !== true) {
        $ERROR("Test failed to run.")
    }
}

ES5Harness = {};
ES5Harness.registerTest = function (test) {
    finished = false;

    if (test.precondition && !test.precondition()) {
        $ERROR("Precondition Failed");
    } else {
        var res = test.test();

        if (res !== true && typeof res !== "undefined") {
            $ERROR("Test reported failure.");
        }
    }

    finished = true;
}
