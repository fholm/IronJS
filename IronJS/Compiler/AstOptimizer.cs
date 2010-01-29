using System.Collections.Generic;
using IronJS.Compiler.Ast;

namespace IronJS.Compiler
{
    public class AstOptimizer
    {
        public List<Node> Optimize(List<Node> astNodes)
        {
            var optimizedNodes = new List<Node>();

            foreach (var node in astNodes)
                optimizedNodes.Add(node.Optimize(this));

            return optimizedNodes;
        }
    }
}
