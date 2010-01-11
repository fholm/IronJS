

var foo = function (bar) {
    print(bar);
    this.test = "hello world";
};

var bar = new foo("y 'elo thar!");
