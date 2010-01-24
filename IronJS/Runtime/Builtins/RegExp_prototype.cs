using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    class RegExp_prototype : Obj
    {
        public RegExp_prototype(Context context)
        {
            Context = context;
            Prototype = context.ObjectConstructor.Object_prototype;
            Class = ObjClass.Object;

            SetOwnProperty("toString", new RegExp_prototype_toString(Context));
        }
    }
}
