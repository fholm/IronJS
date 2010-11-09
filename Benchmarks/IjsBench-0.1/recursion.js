(function(){
  var recursive = function(n) {
    if(n < 3000) {
      return recursive(n+1);
    } else {
      return n;
    }
  };

  recursive(0);
})();