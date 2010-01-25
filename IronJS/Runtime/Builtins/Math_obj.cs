using System;
using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    public class Math_obj : Obj
    {
        public Math_obj(Context context)
            : base()
        {
            Context = context;
            Prototype = context.ObjectConstructor.Object_prototype;
            Class = ObjClass.Math;

            SetOwn("E", Math.E);
            SetOwn("LN10", 2.302585092994046);
            SetOwn("LN2", 0.6931471805599453);
            SetOwn("LOG2E", 1.4426950408889634);
            SetOwn("LOG10E", 0.4342944819032518);
            SetOwn("PI", Math.PI);
            SetOwn("SQRT1_2", 0.7071067811865476);
            SetOwn("SQRT2", 1.4142135623730951);

            SetOwn("abs", new Math_obj_abs(Context));
            SetOwn("acos", new Math_obj_acos(Context));
            SetOwn("asin", new Math_obj_asin(Context));
            SetOwn("atan", new Math_obj_atan(Context));
            SetOwn("atan2", new Math_obj_atan2(Context));
            SetOwn("ceil", new Math_obj_ceil(Context));
            SetOwn("cos", new Math_obj_cos(Context));
            SetOwn("exp", new Math_obj_exp(Context));
            SetOwn("floor", new Math_obj_floor(Context));
            SetOwn("log", new Math_obj_log(Context));
            SetOwn("max", new Math_obj_max(Context));
            SetOwn("min", new Math_obj_min(Context));
            SetOwn("pow", new Math_obj_pow(Context));
            SetOwn("random", new Math_obj_random(Context));
            SetOwn("round", new Math_obj_round(Context));
            SetOwn("sin", new Math_obj_sin(Context));
            SetOwn("sqrt", new Math_obj_sqrt(Context));
            SetOwn("tan", new Math_obj_tan(Context));
        }
    }
}
