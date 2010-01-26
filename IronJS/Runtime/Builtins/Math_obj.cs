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

            this.Set("E", Math.E);
            this.Set("LN10", 2.302585092994046);
            this.Set("LN2", 0.6931471805599453);
            this.Set("LOG2E", 1.4426950408889634);
            this.Set("LOG10E", 0.4342944819032518);
            this.Set("PI", Math.PI);
            this.Set("SQRT1_2", 0.7071067811865476);
            this.Set("SQRT2", 1.4142135623730951);

            this.Set("abs", new Math_obj_abs(Context));
            this.Set("acos", new Math_obj_acos(Context));
            this.Set("asin", new Math_obj_asin(Context));
            this.Set("atan", new Math_obj_atan(Context));
            this.Set("atan2", new Math_obj_atan2(Context));
            this.Set("ceil", new Math_obj_ceil(Context));
            this.Set("cos", new Math_obj_cos(Context));
            this.Set("exp", new Math_obj_exp(Context));
            this.Set("floor", new Math_obj_floor(Context));
            this.Set("log", new Math_obj_log(Context));
            this.Set("max", new Math_obj_max(Context));
            this.Set("min", new Math_obj_min(Context));
            this.Set("pow", new Math_obj_pow(Context));
            this.Set("random", new Math_obj_random(Context));
            this.Set("round", new Math_obj_round(Context));
            this.Set("sin", new Math_obj_sin(Context));
            this.Set("sqrt", new Math_obj_sqrt(Context));
            this.Set("tan", new Math_obj_tan(Context));
        }
    }
}
