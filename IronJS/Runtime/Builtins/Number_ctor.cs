using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Builtins
{
    public class Number_ctor : NativeConstructor
    {
        public IObj Number_prototype { get; private set; }

        public Number_ctor(Context context)
            : base(context)
        {
            Number_prototype = new Number_prototype(Context);
            Number_prototype.SetOwn("constructor", this);

            SetOwn("prototype", Number_prototype);
            SetOwn("MAX_VALUE", double.MaxValue);
            SetOwn("MIN_VALUE", double.MinValue);
            SetOwn("NaN", double.NaN);
            SetOwn("NEGATIVE_INFINITY", double.NegativeInfinity);
            SetOwn("POSITIVE_INFINITY", double.PositiveInfinity);
        }

        public IObj Construct()
        {
            return Construct(0.0D);
        }

        public IObj Construct(double num)
        {
            var obj = new ValueObj(num);

            obj.Class = ObjClass.Number;
            obj.Prototype = Number_prototype;
            obj.Context = Context;

            return obj;
        }

        override public object Call(IObj that, object[] args)
        {
            return args != null && args.Length > 0 ? JsTypeConverter.ToNumber(args[0]) : 0.0D;
        }

        override public IObj Construct(object[] args)
        {
            return Construct(
                HasArgs(args) 
                ? JsTypeConverter.ToNumber(args[0]) 
                : 0.0D
            );
        }
    }
}
