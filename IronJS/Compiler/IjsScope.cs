using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using IronJS.Compiler.Optimizer;
using Et = System.Linq.Expressions.Expression;
using EtParam = System.Linq.Expressions.ParameterExpression;

namespace IronJS.Compiler
{
    public class IjsScope
    {
        public Dictionary<string, Tuple<EtParam, IjsVarInfo>> Variables { get; set; }
        public LabelTarget ReturnLabel { get; protected set; }
        public IjsFuncInfo FuncInfo { get; protected set; }

        public IjsScope(IjsFuncInfo funcInfo)
        {
            FuncInfo = funcInfo;
            ReturnLabel = Et.Label(funcInfo.ReturnType, "$return");
            Variables = new Dictionary<string,Tuple<EtParam,IjsVarInfo>>();
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
