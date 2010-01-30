using System.Collections.Generic;
using EtParam = System.Linq.Expressions.ParameterExpression;

namespace IronJS.Compiler
{
    public class IjsScope
    {
        public Dictionary<string, EtParam> Variables =
            new Dictionary<string, EtParam>();

        public EtParam this[string name]
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
