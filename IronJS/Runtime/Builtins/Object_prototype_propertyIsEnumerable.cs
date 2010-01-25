using System;
using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    class Object_prototype_propertyIsEnumerable : NativeFunction
    {
        public Object_prototype_propertyIsEnumerable(Context context)
            : base(context)
        {
            SetOwn("length", 1);
        }

        public override object Call(IObj that, object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
