using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    static class MathObject
    {
        static Random rnd = new Random();

        internal static IObj Create(Context context)
        {
            var math = context.CreateObject();

            (math as Obj).Class = ObjClass.Math;

            // properties
            math.SetOwnProperty("E", Math.E);
            math.SetOwnProperty("LN10", 2.302585092994046);
            math.SetOwnProperty("LN2", 0.6931471805599453);
            math.SetOwnProperty("LOG2E", 1.4426950408889634);
            math.SetOwnProperty("LOG10E", 0.4342944819032518);
            math.SetOwnProperty("PI", Math.PI);
            math.SetOwnProperty("SQRT1_2", 0.7071067811865476);
            math.SetOwnProperty("SQRT2", 1.4142135623730951);

            // Math.abs
            math.SetOwnProperty("abs",
                context.CreateFunction(
                    context.BuiltinsFrame,
                    new Lambda(
                        (that, frame) =>
                        {
                             return Math.Abs(
                                TypeConverter.ToNumber(
                                    (frame as Frame).Arg("n")
                                 )
                             );
                        }, 
                        new[] { "n" }
                    )
                )
            );

            // Math.acos
            math.SetOwnProperty("acos",
                context.CreateFunction(
                    context.BuiltinsFrame,
                    new Lambda(
                        (that, frame) =>
                        {
                            return Math.Acos(
                               TypeConverter.ToNumber(
                                   (frame as Frame).Arg("n")
                               )
                            );
                        },
                        new[] { "n" }
                    )
                )
            );

            // Math.asin
            math.SetOwnProperty("asin",
                context.CreateFunction(
                    context.BuiltinsFrame,
                    new Lambda(
                        (that, frame) =>
                        {
                            return Math.Asin(
                               TypeConverter.ToNumber(
                                   (frame as Frame).Arg("n")
                               )
                            );
                        },
                        new[] { "n" }
                    )
                )
            );

            // Math.atan
            math.SetOwnProperty("atan",
                context.CreateFunction(
                    context.BuiltinsFrame,
                    new Lambda(
                        (that, frame) =>
                        {
                            return Math.Atan(
                               TypeConverter.ToNumber(
                                   (frame as Frame).Arg("n")
                               )
                            );
                        },
                        new[] { "n" }
                    )
                )
            );

            // Math.atan2
            math.SetOwnProperty("atan2",
                context.CreateFunction(
                    context.BuiltinsFrame,
                    new Lambda(
                        (that, frame) =>
                        {
                            return Math.Atan2(
                               TypeConverter.ToNumber((frame as Frame).Arg("x")),
                               TypeConverter.ToNumber((frame as Frame).Arg("y"))
                            );
                        },
                        new[] { "x", "y" }
                    )
                )
            );

            // Math.ceil
            math.SetOwnProperty("ceil",
                context.CreateFunction(
                    context.BuiltinsFrame,
                    new Lambda(
                        (that, frame) =>
                        {
                            return Math.Ceiling(
                               TypeConverter.ToNumber((frame as Frame).Arg("n"))
                            );
                        },
                        new[] { "n" }
                    )
                )
            );

            // Math.cos
            math.SetOwnProperty("cos",
                context.CreateFunction(
                    context.BuiltinsFrame,
                    new Lambda(
                        (that, frame) =>
                        {
                            return Math.Cos(
                               TypeConverter.ToNumber((frame as Frame).Arg("n"))
                            );
                        },
                        new[] { "n" }
                    )
                )
            );

            // Math.exp
            math.SetOwnProperty("exp",
                context.CreateFunction(
                    context.BuiltinsFrame,
                    new Lambda(
                        (that, frame) =>
                        {
                            return Math.Exp(
                               TypeConverter.ToNumber((frame as Frame).Arg("n"))
                            );
                        },
                        new[] { "n" }
                    )
                )
            );

            // Math.floor
            math.SetOwnProperty("floor",
                context.CreateFunction(
                    context.BuiltinsFrame,
                    new Lambda(
                        (that, frame) =>
                        {
                            return Math.Floor(
                               TypeConverter.ToNumber((frame as Frame).Arg("n"))
                            );
                        },
                        new[] { "n" }
                    )
                )
            );

            // Math.log
            math.SetOwnProperty("log",
                context.CreateFunction(
                    context.BuiltinsFrame,
                    new Lambda(
                        (that, frame) =>
                        {
                            return Math.Log(
                               TypeConverter.ToNumber((frame as Frame).Arg("n"))
                            );
                        },
                        new[] { "n" }
                    )
                )
            );

            // Math.max
            math.SetOwnProperty("max",
                context.CreateFunction(
                    context.BuiltinsFrame,
                    new Lambda(
                        (that, frame) =>
                        {
                            var args = (IObj)(frame as Frame).Arg("arguments");
                            var length = (int)TypeConverter.ToNumber(args.Get("length"));
                            var max = double.NegativeInfinity;

                            for (int i = 0; i < (int)length; ++i)
                            {
                                max = Math.Max(
                                    max, 
                                    TypeConverter.ToNumber(
                                        args.Get(i)
                                    )
                                );
                            }

                            return max;
                        }
                    )
                )
            );

            // Math.min
            math.SetOwnProperty("min",
                context.CreateFunction(
                    context.BuiltinsFrame,
                    new Lambda(
                        (that, frame) =>
                        {
                            var args = (IObj)(frame as Frame).Arg("arguments");
                            var length = (int)TypeConverter.ToNumber(args.Get("length"));
                            var min = double.PositiveInfinity;

                            for (int i = 0; i < length; ++i)
                            {
                                min = Math.Min(
                                    min,
                                    TypeConverter.ToNumber(
                                        args.Get(i)
                                    )
                                );
                            }

                            return min;
                        }
                    )
                )
            );

            // Math.pow
            math.SetOwnProperty("pow",
                context.CreateFunction(
                    context.BuiltinsFrame,
                    new Lambda(
                        (that, frame) =>
                        {
                            return Math.Pow(
                               TypeConverter.ToNumber((frame as Frame).Arg("x")),
                               TypeConverter.ToNumber((frame as Frame).Arg("y"))
                            );
                        },
                        new[] { "x", "y" }
                    )
                )
            );

            // Math.random
            math.SetOwnProperty("random",
                context.CreateFunction(
                    context.BuiltinsFrame,
                    new Lambda(
                        (that, frame) =>
                        {
                            double dbl;

                            lock (rnd)
                                dbl = rnd.NextDouble();

                            return dbl;
                        }
                    )
                )
            );

            // Math.round
            math.SetOwnProperty("round",
                context.CreateFunction(
                    context.BuiltinsFrame,
                    new Lambda(
                        (that, frame) =>
                        {
                            return Math.Round(
                               TypeConverter.ToNumber((frame as Frame).Arg("n"))
                            );
                        },
                        new[] { "n" }
                    )
                )
            );

            // Math.round
            math.SetOwnProperty("sin",
                context.CreateFunction(
                    context.BuiltinsFrame,
                    new Lambda(
                        (that, frame) =>
                        {
                            return Math.Sin(
                               TypeConverter.ToNumber((frame as Frame).Arg("n"))
                            );
                        },
                        new[] { "n" }
                    )
                )
            );

            // Math.round
            math.SetOwnProperty("sqrt",
                context.CreateFunction(
                    context.BuiltinsFrame,
                    new Lambda(
                        (that, frame) =>
                        {
                            return Math.Sqrt(
                               TypeConverter.ToNumber((frame as Frame).Arg("n"))
                            );
                        },
                        new[] { "n" }
                    )
                )
            );

            // Math.round
            math.SetOwnProperty("tan",
                context.CreateFunction(
                    context.BuiltinsFrame,
                    new Lambda(
                        (that, frame) =>
                        {
                            return Math.Tan(
                               TypeConverter.ToNumber((frame as Frame).Arg("n"))
                            );
                        },
                        new[] { "n" }
                    )
                )
            );

            return math;
        }
    }
}
