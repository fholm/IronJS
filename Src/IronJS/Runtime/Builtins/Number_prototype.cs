using IronJS.Runtime.Js;
using IronJS.Runtime.Js.Descriptors;

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

            this.Set("toString", new Number_prototype_toString(Context));
            this.Set("toLocaleString", new Number_prototype_toLocaleString(Context));
            this.Set("valueOf", new Number_prototype_valueOf(Context));
            this.Set("toFixed", new Number_prototype_toFixed(Context));
            this.Set("toExponential", new Number_prototype_toExponential(Context));
            this.Set("toPrecision", new Number_prototype_toPrecision(Context));
        }
    }
}
