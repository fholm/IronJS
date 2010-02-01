using System.Collections.Generic;
using IronJS.Compiler.Ast;
using IronJS.Compiler.Optimizer;

namespace IronJS.Compiler
{
    public class IjsAstAnalyzer
    {
        public bool IsInsideWith { get { return false; } }
        public bool InGlobalScope { get { return Scope.Parent == null; } }
        public IjsAnalyzeScope Scope { get; protected set; }
        public IjsAnalyzeScope GlobalScope { get; protected set; }

        public List<INode> Optimize(List<INode> astNodes)
        {
            GlobalScope = Scope = new IjsAnalyzeScope(null, null);

            var optimizedNodes = new List<INode>();

            foreach (var node in astNodes)
                optimizedNodes.Add(node.Analyze(this));

            return optimizedNodes;
        }

        public void EnterScope(IjsFuncInfo funcInfo)
        {
            Scope = Scope.Enter(funcInfo);
        }

        public void ExitScope()
        {
            if (Scope == null)
                throw new AstCompilerError("Can't exit global scope");

            Scope = Scope.Exit();
        }
    }
}
