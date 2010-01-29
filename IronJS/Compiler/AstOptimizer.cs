using System;
using System.Collections.Generic;
using IronJS.Compiler.Ast;

namespace IronJS.Compiler
{
    public class AstOptimizer
    {
        public bool IsInsideWith { get { return false; } }
        public bool IsGlobal { get { return Scope == null; } }
        public Optimizer.Scope Scope { get; protected set; }

        public List<INode> Optimize(List<INode> astNodes)
        {
            var optimizedNodes = new List<INode>();

            foreach (var node in astNodes)
                optimizedNodes.Add(node.Optimize(this));

            return optimizedNodes;
        }

        public void EnterScope()
        {
            if (Scope == null)
                Scope = new Optimizer.Scope(null);
            else
                Scope = Scope.Enter();
        }

        public void ExitScope()
        {
            if (Scope == null)
                throw new AstCompilerError("Can't exit global scope");

            Scope = Scope.Exit();
        }
    }
}
