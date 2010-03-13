using System;

namespace IronJS.Ast.Nodes.Interfaces {
    interface IVariable : INode {
        void UsedAs(Type type);
        void UsedWith(INode node);
        void Setup();
        void Clear();
    }
}
