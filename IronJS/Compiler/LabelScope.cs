using System.Linq.Expressions;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler
{
    class LabelScope
    {
        readonly LabelTarget _break;
        readonly LabelTarget _continue;

        internal LabelScope Parent { get; private set; }
        internal string Name { get; private set; }
        internal bool IsLoop { get; private set; }

        internal LabelScope(string name = null, bool isLoop = true)
            :this(null, name, isLoop)
        {

        }

        internal LabelScope(LabelScope parent, string name = null, bool isLoop = true)
        {
            Name = name;
            IsLoop = isLoop;
            Parent = parent;

            _break = Et.Label(typeof(void), "#break");

            if(IsLoop)
                _continue = Et.Label(typeof(void), "#continue");
        }

        internal LabelTarget Break(string name = null)
        {
            var scope = this;

            while (scope != null)
            {
                if (scope.IsLoop && name == null)
                    return scope._break;

                if (scope.Name == name)
                    return scope._break;

                scope = scope.Parent;
            }

            if(name == null)
                throw new CompilerError("Not inside a loop");

            throw new CompilerError("No label named '" + name + "' found");
        }

        internal LabelTarget Continue(string name = null)
        {
            var scope = this;

            while (scope != null)
            {
                if (scope.IsLoop)
                {
                    if (name == null)
                        return scope._continue;

                    if (name == scope.Name)
                        return scope._continue;
                }

                scope = scope.Parent;
            }

            if(name == null)
                throw new CompilerError("Not inside a loop");

            throw new CompilerError("No loop labelled '" + name + "' found (continue only works with loops)");
        }

        internal LabelScope Enter(string name, bool isLoop)
        {
            return new LabelScope(this, name, isLoop);
        }

        internal LabelScope Exit()
        {
            return Parent;
        }
    }
}
