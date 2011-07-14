function SputnikError(message) {
    this.name = "SputnikError";
    this.message = (message || "");
}
SputnikError.prototype = Error.prototype;

var currentTest = {};

function testRun(id, path, description, codeString, preconditionString, result, error) {
    currentTest.id = id;
    currentTest.path = path;
    currentTest.description = description;
    currentTest.result = result;
    currentTest.error = error;
    currentTest.code = codeString;
    currentTest.pre = preconditionString;
}

function testFinished() {
    if (typeof currentTest.result === "undefined") {
        currentTest.result = "fail";
        currentTest.error = "Failed to Load";
    } else if (typeof currentTest.error !== "undefined") {
        if (currentTest.error instanceof SputnikError) {
            currentTest.error = currentTest.message;
        } else {
            currentTest.error = currentTest.error.name + ": " + currentTest.error.message
        }
    }
}

ES5Harness = {};
ES5Harness.registerTest = function (test) {
    var error;
    if (test.precondition && !test.precondition()) {
        testRun(test.id, test.path, test.description, test.test.toString(), typeof test.precondition !== 'undefined' ? test.precondition.toString() : '', 'fail', 'Precondition Failed');
    } else {
        try { var res = test.test(); } catch (e) { res = 'fail'; error = e; }
        var retVal = /^s/i.test(test.id) ? (res === true || typeof res === 'undefined' ? 'pass' : 'fail') : (res === true ? 'pass' : 'fail');
        testRun(test.id, test.path, test.description, test.test.toString(), typeof test.precondition !== 'undefined' ? test.precondition.toString() : '', retVal, error);
    }
}
