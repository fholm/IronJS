
namespace IronJS.Compiler.Ast
{
    interface ILabelableNode
    {
        bool IsLabeled { get; }
        void SetLabel(string label);
        void Enter(FunctionScope functionScope);
        void Exit(FunctionScope functionScope);
    }
}
