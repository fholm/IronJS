foo = { 
    bar: { 
        bar_func: function() { emit(this == foo.bar); } 
    }, 
    foo_func: function() { emit(this == foo); } 
};

with (foo) {
    foo_func();
    
    with (bar) {
        bar_func();
    }
    
    foo_func();
}