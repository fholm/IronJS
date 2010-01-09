using System;
using System.Text;
using System.Linq.Expressions;
using IronJS.Runtime.Js;

namespace IronJS.Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            var bar = Expression.Parameter(typeof(int), "bar");
            var label = Expression.Label(typeof(int));

            var l = Expression.Lambda<Func<int, int>>(
                Expression.Block(
                    Expression.Condition(
                        Expression.Equal(Expression.Modulo(bar, Expression.Constant(2)), Expression.Constant(0)),
                        Expression.Return(label, Expression.Multiply(bar, Expression.Constant(10))),
                        Expression.Return(label, Expression.Multiply(bar, Expression.Constant(20)))
                    ),
                    Expression.Label(label, Expression.Default(typeof(int)))
                ),
                new[] { bar }
            );

            l.Compile();

            var astBuilder = new Compiler.Ast.Builder();
            var treeGenerator = new Compiler.Tree.EtGenerator();

            var astNodes = astBuilder.Build("IronJS.js", Encoding.UTF8);

            foreach (var node in astNodes)
                Console.WriteLine(node.Print());

            Frame globalFrame = new Frame();

            globalFrame.Push(
                "print", 
                typeof(IronJS.Runtime.BuiltIns).GetMethod("Print"), 
                VarType.Global
            );

            var compiled = treeGenerator.Build(astNodes);

            compiled(globalFrame);
        }
    }
}
