using IronJS.Runtime.Js;
using IronJS.Runtime.Js.Descriptors;

namespace IronJS.Runtime.Builtins
{
    class Array_prototype : JsArray
    {
        public Array_prototype(Context context)
            : base()
        {
            Context = context;
            Prototype = context.ObjectConstructor.Object_prototype;
            Class = ObjClass.Array;

            this.Set("toString", new Array_prototype_toString(Context));
            this.Set("toLocaleString", new Array_prototype_toLocaleString(Context));
            this.Set("concat", new Array_prototype_concat(Context));
            this.Set("join", new Array_prototype_join(Context));
            this.Set("pop", new Array_prototype_pop(Context));
            this.Set("push", new Array_prototype_push(Context));
            this.Set("reverse", new Array_prototype_reverse(Context));
            this.Set("shift", new Array_prototype_shift(Context));
            this.Set("slice", new Array_prototype_slice(Context));
            this.Set("sort", new Array_prototype_sort(Context));
            this.Set("splice", new Array_prototype_splice(Context));
            this.Set("unshift", new Array_prototype_unshift(Context));
        }
    }
}
