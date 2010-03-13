using System;

namespace IronJS.Ast.Nodes {
    public interface IVariable : INode {
        string Name { get; }
        void UsedAs(Type type);
        void UsedWith(INode node);
        void Setup();
        void Clear();
    }
}
