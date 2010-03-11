var b = function(c) {
    print(2);
    var d = 1;
    var x = c;
};

b(2);

var foo = function(clos, this, a, b) {

    var _1 = "1";

    with(a) {
        clos.withStack.Push(a);
    
        //print(_1);
        print({
            if(clos.withStack[0].has("_1")) {
                return clos.withStack[0]._1;
            }
            return _1;
        });
        
        with(i) {
        
            clos.withStack.Push(i);
            
            var _2.Value = "_2"; // closed over
        
            // kept as-is
            print(_2.Value); 
            
            //print(_3);
            print({
                if(clos.withStack[1].has("_3")) {
                    return clos.withStack[1]._3;
                } else if(clos.withStack[0].has("_3")) {
                    return clos.withStack[0]._3;
                }
                return _1;
            });
            
            var bar = new Func(new Clos(clos.withStack, clos.context, clos.globals), function(clos, this, y, z) {
                with(y) {
                    clos.withStack.Push(y)
                    
                    //print(_2);
                    print({
                        if(clos.withStack[2].has("_2")) {
                            return clos.withStack[2]._2;
                        }
                        return clos._2.Value;
                    });
                    
                    clos.withStack.Pop();
                };
            });
            
            
            clos.withStack.Pop(if);
        };
        
        clos.withStack.Pop();
    };

};