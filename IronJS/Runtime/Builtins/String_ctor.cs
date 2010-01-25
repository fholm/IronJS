using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Builtins
{
    public class String_ctor : NativeConstructor
    {
        public IObj String_prototype { get; private set; }

        public String_ctor(Context context)
            : base(context)
        {
            String_prototype = new String_prototype(Context);
            String_prototype.SetOwnProperty("constructor", this);

            SetOwnProperty("prototype", String_prototype);
            SetOwnProperty("fromCharCode", new String_ctor_fromCharCode(Context));
        }

        public IObj Construct()
        {
            return Construct(null);
        }

        override public object Call(IObj that, object[] args)
        {
            if (args.Length > 0)
                return JsTypeConverter.ToString(args[0]);

            return "";
        }

        override public IObj Construct(object[] args)
        {
            var str = args != null && args.Length > 0 ? JsTypeConverter.ToString(args[0]) : "";
            var obj = new ValueObj(str);

            obj.Class = ObjClass.String;
            obj.Prototype = String_prototype;
            obj.Context = Context;
            obj.SetOwnProperty("length", (double) str.Length);

            return obj;
        }
    }
}
