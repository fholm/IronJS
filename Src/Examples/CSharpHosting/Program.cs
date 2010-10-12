using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS;

namespace CSharpHosting {
  class Program {
    static void test(IronJS.Box b, IronJS.Box b2, int a) {
      return;
    }

    static void Main(string[] args) {
      var ctx = Hosting.Context.Create();
      var env = ctx.Environment;

      //var fn = IronJS.Api.DelegateFunction<int>.create(env, new Action<IronJS.Box, IronJS.Box, int>(test));
      //IronJS.Api.Object.putProperty(env, "test2", fn);

      ctx.Execute(@"
function Book(name) {
    var _name = name;

    this.Name = function(n) {
        if(n) {
            _name = n;
        }
        return _name;
    };
};

var book = new Book('IronJS in Action');

");
      dynamic book = ctx.ExecuteT<string>("book.Name();");
      Console.WriteLine(book);
      Console.ReadLine();

      book = ctx.Execute("book.Name('IronJS to C#');");
      Console.WriteLine(book);
      Console.ReadLine();

      ctx.Execute("1 === 1");
    }
  }
}
