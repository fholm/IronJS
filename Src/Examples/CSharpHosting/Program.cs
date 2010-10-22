using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS;
using IronJS.Api;

namespace CSharpHosting {
    class Program {
        static void Main(string[] args) {
            var ctx = Hosting.Context.Create();
            var env = ctx.Environment;

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

            var book = ctx.ExecuteT<string>("book.Name();");
            Console.WriteLine(book);
            Console.ReadLine();
        }
    }
}
