function benchmark() {
    var r;
    var z = [1, 2, 3, 4, 5, 6];
    var x = {a: 1, b: 2, c: 3, d: 4, e: 5, f: 6};

    for (var i = 0; i < 1000000; ++i) {
        r = x.a;
        r = x.b;
        r = x.c;
        r = x.d;
        r = x.e;
        r = x.f;

        r = x.a;
        r = x.b;
        r = x.c;
        r = x.d;
        r = x.e;
        r = x.f;

        r = z[0];
        r = z[1];
        r = z[2];
        r = z[3];
        r = z[4];
        r = z[5];
        r = z[0];

        r = z[1];
        r = z[2];
        r = z[3];
        r = z[4];
        r = z[5];
    }
};