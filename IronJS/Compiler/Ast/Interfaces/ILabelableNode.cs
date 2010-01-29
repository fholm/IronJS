namespace IronJS.Compiler.Ast
{
    public interface ILabelableNode : INode
    {
        void SetLabel(string label);
    }
}
