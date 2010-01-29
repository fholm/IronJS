namespace IronJS.Compiler.Ast
{
    enum JsType { String, Boolean, Integer, Double, Null, Object, Dynamic }

    interface IExpressionNode
    {
        JsType ExpressionType { get; }
    }
}
