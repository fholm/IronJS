using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    class Array_prototype : ArrayObj
    {
        public Array_prototype(Context context)
            : base()
        {
            Context = context;
            Prototype = context.ObjectConstructor.Object_prototype;
            Class = ObjClass.Array;

            SetOwnProperty("concat", new Array_prototype_concat(Context));
            SetOwnProperty("join", new Array_prototype_join(Context));
            SetOwnProperty("pop", new Array_prototype_pop(Context));
            SetOwnProperty("push", new Array_prototype_push(Context));
        }
    }
}
