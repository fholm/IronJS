using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using IronJS.Extensions;
using IronJS.Runtime;
using IronJS.Runtime.Binders;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Microsoft.Scripting.Utils;

namespace IronJS.Compiler
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = System.Linq.Expressions.Expression;

    class FunctionScope
    {
        LabelScope _labelScope;

        internal readonly LabelTarget ReturnLabel;
        internal readonly ParameterExpression FrameExpr;

        internal LabelScope LabelScope
        {
            get
            {
                if (_labelScope != null)
                    return _labelScope;

                throw new CompilerError("Not inside a label scope");
            }
        }

        public FunctionScope()
        {
            ReturnLabel = Et.Label(typeof(object), "#return");
            FrameExpr = Et.Parameter(typeof(Frame), "#frame");
        }

        public void EnterLabelScope()
        {
            EnterLabelScope(null, true);
        }

        public void EnterLabelScope(string name, bool isLoop)
        {
            _labelScope = _labelScope.Enter(name, isLoop);
        }

        public void ExitLabelScope()
        {
            _labelScope = _labelScope.Parent;
        }
    }
}
