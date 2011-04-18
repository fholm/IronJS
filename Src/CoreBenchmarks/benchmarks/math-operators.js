function benchmark() {
    var r = 0;
    var a = 5;

    for (var i = 0; i < 1000000; ++i) {
        r = 1 + a;
        r = 2 + a;
        r = 3 + a;
        r = 4 + a;
        r = 5 + a;
        r = 6 + a;

        r = 1 - a;
        r = 2 - a;
        r = 3 - a;
        r = 4 - a;
        r = 5 - a;
        r = 6 - a;

        r = 1 / a;
        r = 2 / a;
        r = 3 / a;
        r = 4 / a;
        r = 5 / a;
        r = 6 / a;

        r = 1 * a;
        r = 2 * a;
        r = 3 * a;
        r = 4 * a;
        r = 5 * a;
        r = 6 * a;

        r = 1 % a;
        r = 2 % a;
        r = 3 % a;
        r = 4 % a;
        r = 5 % a;
        r = 6 % a;
    }
};