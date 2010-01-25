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

            SetOwn("toString", new Array_prototype_toString(Context));
            SetOwn("toLocaleString", new Array_prototype_toLocaleString(Context));
            SetOwn("concat", new Array_prototype_concat(Context));
            SetOwn("join", new Array_prototype_join(Context));
            SetOwn("pop", new Array_prototype_pop(Context));
            SetOwn("push", new Array_prototype_push(Context));
            SetOwn("reverse", new Array_prototype_reverse(Context));
            SetOwn("shift", new Array_prototype_shift(Context));
            SetOwn("slice", new Array_prototype_slice(Context));
            SetOwn("sort", new Array_prototype_sort(Context));
            SetOwn("splice", new Array_prototype_splice(Context));
            SetOwn("unshift", new Array_prototype_unshift(Context));
        }
    }
}
