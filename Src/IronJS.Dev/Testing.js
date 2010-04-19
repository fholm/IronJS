
function bench()
{
    function f(x, y, z)
    {
        return x + y + z;
    }
    var y = "lol"
    y = 2;

    var z = "lol"
    z = 2;

    for (var i = 0; i < 2500000; ++i) {
        f(1, 2, 3);
        y = z;
    }
}

bench();
