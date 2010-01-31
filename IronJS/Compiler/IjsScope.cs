using System;
using System.Collections.Generic;
using IronJS.Compiler.Optimizer;
using EtParam = System.Linq.Expressions.ParameterExpression;

namespace IronJS.Compiler
{
    public class IjsScope
    {
        public Dictionary<string, Tuple<EtParam, Variable>> Variables =
            new Dictionary<string, Tuple<EtParam, Variable>>();

        public Tuple<EtParam, Variable> this[string name]
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
