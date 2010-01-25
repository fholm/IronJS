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
            
            SetOwnProperty("toString", new Object_prototype_toString(Context));
            SetOwnProperty("valueOf", new Object_prototype_valueOf(Context));
            SetOwnProperty("hasOwnProperty", new Object_prototype_hasOwnProperty(Context));
            SetOwnProperty("isPrototypeOf", new Object_prototype_isPrototypeOf(Context));
            SetOwnProperty("propertyIsEnumerable", new Object_prototype_propertyIsEnumerable(Context));
            SetOwnProperty("toLocaleString", new Object_prototype_toLocaleString(Context));
        }
    }
}
