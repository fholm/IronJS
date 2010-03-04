using System;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;
using IronJS.Tools;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{
    public class Obj : Node
    {
        public Dictionary<string, INode> Properties { get; protected set; }

        public Obj(Dictionary<string, INode> properties, ITree node)
            : base(NodeType.Object, node)
        {
            Properties = properties;
        }

        public override Type Type
        {
            get
            {
                return IjsTypes.Object;
            }
        }

        public override INode Analyze(Stack<Function> astopt)
        {
            foreach (string key in DictionaryTools.GetKeys(Properties))
                Properties[key] = Properties[key].Analyze(astopt);

            return this;
        }
    }
}
