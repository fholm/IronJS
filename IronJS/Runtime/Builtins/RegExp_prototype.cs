using IronJS.Runtime.Js;
using IronJS.Runtime.Js.Descriptors;

namespace IronJS.Runtime.Builtins
{
    class RegExp_prototype : Obj
    {
        public RegExp_prototype(Context context)
        {
            Context = context;
            Prototype = context.ObjectConstructor.Object_prototype;
            Class = ObjClass.Object;

            Set("exec", 
                new UserProperty(
                    this,
                    new RegExp_prototype_exec(Context)
                )
            );

            Set("test",
                new UserProperty(
                    this,
                    new RegExp_prototype_test(Context)
                )
            );

            Set("toString",
                new UserProperty(
                    this,
                    new RegExp_prototype_toString(Context)
                )
            );
        }
    }
}
