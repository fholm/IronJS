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
