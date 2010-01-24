using System;
using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    public class Math_obj : Obj
    {
        internal Math_obj(Context context)
            : base()
        {
            Context = context;
            Prototype = context.ObjectConstructor.Object_prototype;
            Class = ObjClass.Math;

            SetOwnProperty("E", Math.E);
            SetOwnProperty("LN10", 2.302585092994046);
            SetOwnProperty("LN2", 0.6931471805599453);
            SetOwnProperty("LOG2E", 1.4426950408889634);
            SetOwnProperty("LOG10E", 0.4342944819032518);
            SetOwnProperty("PI", Math.PI);
            SetOwnProperty("SQRT1_2", 0.7071067811865476);
            SetOwnProperty("SQRT2", 1.4142135623730951);

            SetOwnProperty("abs", new Math_obj_abs(Context));
            SetOwnProperty("acos", new Math_obj_acos(Context));
            SetOwnProperty("asin", new Math_obj_asin(Context));
            SetOwnProperty("atan", new Math_obj_atan(Context));
            SetOwnProperty("atan2", new Math_obj_atan2(Context));
            SetOwnProperty("ceil", new Math_obj_ceil(Context));
            SetOwnProperty("cos", new Math_obj_cos(Context));
            SetOwnProperty("exp", new Math_obj_exp(Context));
            SetOwnProperty("floor", new Math_obj_floor(Context));
            SetOwnProperty("log", new Math_obj_log(Context));
            SetOwnProperty("max", new Math_obj_max(Context));
            SetOwnProperty("min", new Math_obj_min(Context));
            SetOwnProperty("pow", new Math_obj_pow(Context));
            SetOwnProperty("random", new Math_obj_random(Context));
            SetOwnProperty("round", new Math_obj_round(Context));
            SetOwnProperty("sin", new Math_obj_sin(Context));
            SetOwnProperty("sqrt", new Math_obj_sqrt(Context));
            SetOwnProperty("tan", new Math_obj_tan(Context));
        }

        internal static Math_obj Create(Context context)
        {
            return new Math_obj(context);
        }
    }
}
