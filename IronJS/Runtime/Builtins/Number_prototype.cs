using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    class Number_prototype : ValueObj
    {
        public Number_prototype(Context context)
            : base(0.0D)
        {
            Context = context;
            Prototype = context.ObjectConstructor.Object_prototype;
            Class = ObjClass.Number;

            SetOwn("toString", new Number_prototype_toString(Context));
            SetOwn("toLocaleString", new Number_prototype_toLocaleString(Context));
            SetOwn("valueOf", new Number_prototype_valueOf(Context));
            SetOwn("toFixed", new Number_prototype_toFixed(Context));
            SetOwn("toExponential", new Number_prototype_toExponential(Context));
            SetOwn("toPrecision", new Number_prototype_toPrecision(Context));
        }
    }
}
