using IronJS.Runtime.Js;

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

            SetOwnProperty("toString", new Boolean_prototype_toString(Context));
            SetOwnProperty("valueOf", new Boolean_prototype_valueOf(Context));
        }
    }
}
