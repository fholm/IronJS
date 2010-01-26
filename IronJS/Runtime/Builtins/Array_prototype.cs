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

            Set("toString", 
                new UserProperty(
                    this, new Array_prototype_toString(Context)
                )
            );

            Set("toLocaleString",
                new UserProperty(
                    this, new Array_prototype_toLocaleString(Context)
                )
            );

            Set("concat",
                new UserProperty(
                    this, new Array_prototype_concat(Context)
                )
            );

            Set("join",
                new UserProperty(
                    this, new Array_prototype_join(Context)
                )
            );

            Set("pop",
                new UserProperty(
                    this, new Array_prototype_pop(Context)
                )
            );

            Set("push",
                new UserProperty(
                    this, new Array_prototype_push(Context)
                )
            );

            Set("reverse",
                new UserProperty(
                    this, new Array_prototype_reverse(Context)
                )
            );

            Set("shift",
                new UserProperty(
                    this, new Array_prototype_shift(Context)
                )
            );

            Set("slice",
                new UserProperty(
                    this, new Array_prototype_slice(Context)
                )
            );

            Set("sort",
                new UserProperty(
                    this, new Array_prototype_sort(Context)
                )
            );

            Set("splice",
                new UserProperty(
                    this, new Array_prototype_splice(Context)
                )
            );

            Set("unshift",
                new UserProperty(
                    this, new Array_prototype_unshift(Context)
                )
            );
        }
    }
}
