using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Builtins
{
    public class Boolean_ctor : NativeConstructor
    {
        public IObj Boolean_prototype { get; private set; }

        public Boolean_ctor(Context context)
            : base(context)
        {
            Boolean_prototype = new Boolean_prototype(Context);
            Boolean_prototype.Set("constructor", this);

            this.Set("prototype", Boolean_prototype);
        }

        public IObj Construct()
        {
            return Construct(false);
        }

        public IObj Construct(bool bol)
        {
            var obj = new ValueObj(bol);

            obj.Class = ObjClass.Boolean;
            obj.Prototype = Boolean_prototype;
            obj.Context = Context;

            return obj;
        }

        override public object Call(IObj that, object[] args)
        {
            return HasArgs(args) ? JsTypeConverter.ToBoolean(args[0]) : false;
        }

        override public IObj Construct(object[] args)
        {
            return Construct(
                HasArgs(args) 
                ? JsTypeConverter.ToBoolean(args[0]) 
                : false
            );
        }
    }
}
