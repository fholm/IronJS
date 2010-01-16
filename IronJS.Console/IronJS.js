
/*
print(true.toString());
print((3.14).toString());

bar = {};
bar.x = 2;
*/

print(Math.abs("-3.14"));
print(Math.abs());
print(Math.max(1, 10, 20, 5));
print(Math.min(5, 6, 3, 10));
print(Math.PI.toString());

/*
try {
    throw bar;
} catch (x) {
    print(x.x);
}

prettyPrint = function (str) {
    println("~*~ " + str + " ~*~");
};

address = function (street, zipcode, city) {
    this.street = street;
    this.zipcode = zipcode;
    this.city = city;
    this.print = function () {
        println("");
        prettyPrint(street);
        println(zipcode + " " + city);
    };
};

user = function (name, age) {
    this.name = name;
    this.age = age + 1;
    this.address = new address("Backa Ringgata 26", 51163, "Skene");

    test = this;

    this.print = function (testline) {
        println("line: " + testline);
        println("Name: " + this.name);
        println("Age: " + age + ", real: " + this.age);
    };
};

user.prototype.lol = "tjo";
fredrik = new user("Fredrik", 24);
anders = new user("Anders", 22);

with (fredrik) {
    print("1");
    anders.print("2");
    address.print("3");
    prettyPrint(" hello world ");
}

println("foo".lol); // <- undefined

foo = { "a": 1, "b": 2, "c": 3 };

delete foo.a;
delete foo["c"];

for (bar.key in foo) {
    println(bar.key + ": " + foo[bar.key]);
};
*/