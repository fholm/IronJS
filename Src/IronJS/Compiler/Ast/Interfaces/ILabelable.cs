namespace IronJS.Compiler.Ast
{
    public interface ILabelable : INode
    {
        void SetLabel(string label);
    }
}
