using System.Collections.Generic;

namespace IronJS.Compiler.Optimizer
{
    public class Scope
    {
        public Scope Parent { get; protected set; }
        public Dictionary<string, Variable> Variables { get; protected set; }

        public Scope(Scope parent)
        {
            Parent = parent;
            Variables = new Dictionary<string, Variable>();
        }

        public Scope Enter()
        {
            return new Scope(this);
        }

        public Scope Exit()
        {
            return Parent;
        }

        public Variable CreateVariable(string name)
        {
            if (Variables.ContainsKey(name))
                throw new AstCompilerError("A variable named {0} already exists", name);

            Variables.Add(name, new Variable(name));
            return Variables[name];
        }

        public bool GetVariable(string name, out Variable variable)
        {
            if (Variables.TryGetValue(name, out variable))
                return true;

            if (Parent != null)
            {
                if (Parent.GetVariable(name, out variable))
                {
                    variable.IsClosedOver = true;
                    return true;
                }
            }

            variable = null;
            return false;
        }
    }
}
