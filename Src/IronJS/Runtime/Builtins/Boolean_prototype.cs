using IronJS.Runtime.Js;
using IronJS.Runtime.Js.Descriptors;

namespace IronJS.Runtime.Builtins
{
    class Boolean_prototype : ValueObj
    {
        public Boolean_prototype(Context context)
            : base(false)
        {
            Context = context;
            Prototype = context.ObjectConstructor.Object_prototype;
            Class = ObjClass.Boolean;

            this.Set("toString", new Boolean_prototype_toString(Context));
            this.Set("valueOf", new Boolean_prototype_valueOf(Context));
        }
    }
}
