(function(){
  var a = new Array(10000);

  for(var i = 0; i < 10000; ++i) {
	  a[i] = Math.random() * 10000;
  }

  a.sort(function(a, b) { return a - b; })
  a.sort();
})();