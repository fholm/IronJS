using System;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Builtins
{
    public class Function_ctor : NativeConstructor
    {
        public IFunction Function_prototype { get; private set; }

        public Function_ctor(Context context)
            : base(context, null)
        {
            Function_prototype = new Function_prototype(context);
            this.Set("prototype", Function_prototype);
        }

        public IObj Construct()
        {
            return Construct(null);
        }

        override public object Call(IObj that, object[] args)
        {
            throw new NotImplementedException();
        }

        override public IObj Construct(object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
