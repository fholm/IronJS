var y = function(a) {
    var _ = 2;
    return function() { return _ * a; };
};

for (var i = 0; i < 10; ++i) {
    y(1);
}

y("asd");

y(1);