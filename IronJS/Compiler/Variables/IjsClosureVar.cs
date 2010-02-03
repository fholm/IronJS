using System;
using IronJS.Compiler.Ast;
using IronJS.Runtime2.Js;
using System.Linq.Expressions;

namespace IronJS.Compiler
{
    public class IjsClosureVar : IjsIVar
    {
        string _name;
        FuncNode _func;

        public IjsClosureVar(string name, FuncNode func)
        {
            _name = name;
            _func = func;
        }

        #region IjsIVarInfo Members

        public ParameterExpression Expr { get; set; }

        public Type ExprType
        {
            get
            {
                if (_func.ClosureParm != null)
                {
                    var field = _func.ClosureParm.Type.GetField(_name);

                    if (field == null)
                        throw new EtCompilerError("No closure variable named '{0}' exists", _name);

                    return field.FieldType;
                }

                return IjsTypes.Undefined;
            }
        }

        #endregion
    }
}
