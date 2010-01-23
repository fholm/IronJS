using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    class String_prototype : ValueObj
    {
        public String_prototype(Context context)
            : base("")
        {
            Context = context;
            Prototype = context.ObjectConstructor.Object_prototype;
            Class = ObjClass.String;

            SetOwnProperty("toString", new String_prototype_toString(Context));
            SetOwnProperty("valueOf", new String_prototype_valueOf(Context));
            SetOwnProperty("charAt", new String_prototype_charAt(Context));
            SetOwnProperty("charCodeAt", new String_prototype_charCodeAt(Context));
            SetOwnProperty("concat", new String_prototype_concat(Context));
            SetOwnProperty("indexOf", new String_prototype_indexOf(Context));
            SetOwnProperty("lastIndexOf", new String_prototype_lastIndexOf(Context));
        }
    }
}
