using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using IronJS.Compiler.Ast;
using IronJS.Compiler.Optimizer;
using Et = System.Linq.Expressions.Expression;
using EtParam = System.Linq.Expressions.ParameterExpression;

namespace IronJS.Compiler
{
    public class IjsScope
    {
        public Dictionary<string, Tuple<EtParam, IjsVarInfo>> Variables { get; set; }
        public FuncNode FuncInfo { get; protected set; }

        public IjsScope(FuncNode funcInfo)
        {
            FuncInfo = funcInfo;
            Variables = new Dictionary<string, Tuple<EtParam,IjsVarInfo>>();
        }

        public Tuple<EtParam, IjsVarInfo> this[string name]
        {
            get
            {
                return Variables[name];
            }
            set
            {
                Variables[name] = value;
            }
        }
    }
}
