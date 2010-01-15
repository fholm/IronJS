_outer:
for (i = 0; i < 10; ++i) {

    if (i == 5)
        break _outer;

    print("iter: " + i);
    
    _inner:
    for (j = 0; j < 10; ++j) {
    
        if (j == 5)
            continue _outer;

        print(j);
    }
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

    this.print = function () {
        println("Name: " + this.name);
        println("Age: " + age + ", real: " + this.age);
    };
};

fredrik = new user("Fredrik", 24);
anders = new user("Anders", 22);

with (fredrik) {
    print();
    anders.print();
    address.print();
    prettyPrint(" hello world ");
}

print("1".toString);