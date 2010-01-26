using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Js.Descriptors;

namespace IronJS.Runtime.Builtins
{
    class Object_prototype_propertyIsEnumerable : NativeFunction
    {
        public Object_prototype_propertyIsEnumerable(Context context)
            : base(context)
        {
            Set("length",
                new UserProperty(this, 1.0D)
            );
        }

        public override object Call(IObj that, object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
