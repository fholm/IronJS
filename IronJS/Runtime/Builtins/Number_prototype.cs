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

            SetOwnProperty("toString", new Number_prototype_toString(Context));
            SetOwnProperty("toLocaleString", new Number_prototype_toLocaleString(Context));
            SetOwnProperty("valueOf", new Number_prototype_valueOf(Context));
            SetOwnProperty("toFixed", new Number_prototype_toFixed(Context));
            SetOwnProperty("toExponential", new Number_prototype_toExponential(Context));
            SetOwnProperty("toPrecision", new Number_prototype_toPrecision(Context));
        }
    }
}
