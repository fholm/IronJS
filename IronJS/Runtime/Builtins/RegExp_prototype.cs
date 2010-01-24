using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    class RegExp_prototype : Obj
    {
        public RegExp_prototype(Context context)
        {
            Context = context;
            Prototype = context.ObjectConstructor.Object_prototype;
            Class = ObjClass.Object;

            SetOwnProperty("exec", new RegExp_prototype_exec(Context));
            SetOwnProperty("test", new RegExp_prototype_test(Context));
            SetOwnProperty("toString", new RegExp_prototype_toString(Context));
        }
    }
}
