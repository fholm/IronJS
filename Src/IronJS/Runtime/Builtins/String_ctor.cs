using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

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
            String_prototype.Set("constructor", this);

            this.Set("prototype", String_prototype);
            this.Set("fromCharCode", new String_ctor_fromCharCode(Context));
        }

        public IObj Construct()
        {
            return Construct("");
        }

        public IObj Construct(string value)
        {
            var obj = new ValueObj(value);

            obj.Class = ObjClass.String;
            obj.Prototype = String_prototype;
            obj.Context = Context;
            obj.Set("length", (double)value.Length);

            return obj;
        }

        override public object Call(IObj that, object[] args)
        {
            if (args.Length > 0)
                return JsTypeConverter.ToString(args[0]);

            return "";
        }

        override public IObj Construct(object[] args)
        {
            return Construct(
                HasArgs(args) 
                ? JsTypeConverter.ToString(args[0]) 
                : ""
            );
        }
    }
}
