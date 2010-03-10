namespace IronJS.Ast.Nodes
{
    public interface ILabelable : INode
    {
        void SetLabel(string label);
    }
}
