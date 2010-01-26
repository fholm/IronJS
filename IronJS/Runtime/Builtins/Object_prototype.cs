using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    class Object_prototype : Obj
    {
        public Object_prototype(Context context)
            : base()
        {
            Context = context;
            Prototype = null;
            Class = ObjClass.Object;

            this.Set("toString", new Object_prototype_toString(Context));
            this.Set("valueOf", new Object_prototype_valueOf(Context));
            this.Set("hasOwnProperty", new Object_prototype_hasOwnProperty(Context));
            this.Set("isPrototypeOf", new Object_prototype_isPrototypeOf(Context));
            this.Set("propertyIsEnumerable", new Object_prototype_propertyIsEnumerable(Context));
            this.Set("toLocaleString", new Object_prototype_toLocaleString(Context));
        }
    }
}
