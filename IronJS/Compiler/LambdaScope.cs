using System.Collections.Generic;
using EtParam = System.Linq.Expressions.ParameterExpression;

namespace IronJS.Compiler
{
    public class LambdaScope
    {
        public LambdaScope Parent { get; protected set; }
        public Dictionary<string, EtParam> Variables =
            new Dictionary<string, EtParam>();

        public LambdaScope(LambdaScope parent)
        {
            Parent = parent;
        }

        public LambdaScope Enter()
        {
            return new LambdaScope(this);
        }

        public LambdaScope Exit()
        {
            return Parent;
        }

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
