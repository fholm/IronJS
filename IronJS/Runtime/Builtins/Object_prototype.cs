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
            
            SetOwn("toString", new Object_prototype_toString(Context));
            SetOwn("valueOf", new Object_prototype_valueOf(Context));
            SetOwn("hasOwnProperty", new Object_prototype_hasOwnProperty(Context));
            SetOwn("isPrototypeOf", new Object_prototype_isPrototypeOf(Context));
            SetOwn("propertyIsEnumerable", new Object_prototype_propertyIsEnumerable(Context));
            SetOwn("toLocaleString", new Object_prototype_toLocaleString(Context));
        }
    }
}
