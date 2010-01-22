foo = { 
    bar: { 
        bar_func: function() { println(this == foo.bar); } 
    }, 
    foo_func: function() { println(this == foo); } 
};

with (foo) {
    foo_func();
    
    with (bar) {
        bar_func();
    }
    
    foo_func();
}