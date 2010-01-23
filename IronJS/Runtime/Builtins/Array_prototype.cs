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

            SetOwnProperty("toString", new Array_prototype_toString(Context));
            SetOwnProperty("toLocaleString", new Array_prototype_toLocaleString(Context));
            SetOwnProperty("concat", new Array_prototype_concat(Context));
            SetOwnProperty("join", new Array_prototype_join(Context));
            SetOwnProperty("pop", new Array_prototype_pop(Context));
            SetOwnProperty("push", new Array_prototype_push(Context));
            SetOwnProperty("reverse", new Array_prototype_reverse(Context));
            SetOwnProperty("shift", new Array_prototype_shift(Context));
            SetOwnProperty("slice", new Array_prototype_slice(Context));
            SetOwnProperty("sort", new Array_prototype_sort(Context));
            SetOwnProperty("splice", new Array_prototype_splice(Context));
            SetOwnProperty("unshift", new Array_prototype_unshift(Context));
        }
    }
}
