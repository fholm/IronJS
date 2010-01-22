using System.Linq.Expressions;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler
{
    class FunctionScope
    {
        LabelScope _labelScope;

        internal FunctionScope Parent { get; private set; }
        internal LabelTarget ReturnLabel { get; private set; }
        internal ParameterExpression ScopeExpr { get; private set; }
        internal ParameterExpression ThisExpr { get; private set; }

        internal LabelScope LabelScope
        {
            get
            {
                if (_labelScope != null)
                    return _labelScope;

                throw new CompilerError("Not inside a labelled statement or a loop");
            }
        }

        internal FunctionScope()
            : this(null)
        {

        }

        internal FunctionScope(FunctionScope parent)
        {
            Parent = parent;
            ReturnLabel = Et.Label(typeof(object), "#return");
            ScopeExpr = Et.Parameter(typeof(Scope), "#scope");
            ThisExpr = Et.Parameter(typeof(IObj), "#this");
        }

        internal void EnterLabelScope(string name, bool isLoop)
        {
            if (_labelScope == null)
            {
                _labelScope = new LabelScope(name, isLoop);
            }
            else
            {
                _labelScope = _labelScope.Enter(name, isLoop);
            }
        }

        internal void ExitLabelScope()
        {
            _labelScope = _labelScope.Exit();
        }

        internal FunctionScope Enter()
        {
            return new FunctionScope(this);
        }

        internal FunctionScope Exit()
        {
            return Parent;
        }

    }
}
