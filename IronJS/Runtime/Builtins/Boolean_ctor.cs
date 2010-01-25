using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;
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
            Boolean_prototype.SetOwnProperty("constructor", this);

            SetOwnProperty("prototype", Boolean_prototype);
        }

        public IObj Construct()
        {
            return Construct(null);
        }

        override public object Call(IObj that, object[] args)
        {
            return args != null && args.Length > 0 ? JsTypeConverter.ToBoolean(args[0]) : false;
        }

        override public IObj Construct(object[] args)
        {
            var bol = args != null && args.Length > 0 ? JsTypeConverter.ToBoolean(args[0]) : false;
            var obj = new ValueObj(bol);

            obj.Class = ObjClass.Boolean;
            obj.Prototype = Boolean_prototype;
            obj.Context = Context;

            return obj;
        }
    }
}
