using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Linq.Expressions;
using IronJS.Runtime;
using IronJS.Compiler.Ast;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Binders
{
    using Et = System.Linq.Expressions.Expression;
    using ParamEt = System.Linq.Expressions.ParameterExpression;
    using Meta = System.Dynamic.DynamicMetaObject;
    using Restrict = System.Dynamic.BindingRestrictions;

    class JsBinaryOpBinder : BinaryOperationBinder
    {
        public JsBinaryOpBinder(ExpressionType op)
            : base(op)
        {

        }

        public override Meta FallbackBinaryOperation(Meta target, Meta arg, Meta error)
        {
            //TODO: insert defer
            //TODO: optimize common double + double case for all operations
            //TODO: handle infinity + zero stuff correct

            var leftTmp = Et.Parameter(typeof(object), "leftTmp");
            var rightTmp = Et.Parameter(typeof(object), "rightTmp");

            var typeRestriction = true;

            Et expr = null;

            switch (Operation)
            {
                // 11.6.1
                case ExpressionType.Add:
                    // step 12 - 15
                    if (target.LimitType == typeof(string) || arg.LimitType == typeof(string))
                    {
                        expr = Et.Call(
                            typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) }),
                            TypeConverter.ToString(target),
                            TypeConverter.ToString(arg)
                        );
                    }
                    // step 1 - 6 and 8 - 11
                    else
                    {
                        //TODO: verify handling of NaN and Infinity
                        expr = Et.Add(
                            TypeConverter.ToNumber(target),
                            TypeConverter.ToNumber(arg)
                        );
                    }
                    break;

                case ExpressionType.Subtract: // 11.6.2
                case ExpressionType.Multiply: // 11.5.1
                case ExpressionType.Divide: // 11.5.2
                case ExpressionType.Modulo: // 11.5.3
                    //TODO: verify handling of NaN and Infinity
                    expr = Et.MakeBinary(
                        Operation,
                        TypeConverter.ToNumber(target),
                        TypeConverter.ToNumber(arg)
                    );
                    break;

                case ExpressionType.LessThan: // 11.8.1
                case ExpressionType.GreaterThan: // 11.8.2
                case ExpressionType.LessThanOrEqual: // 11.8.3
                case ExpressionType.GreaterThanOrEqual: // 11.8.4
                    // 11.8.5 comparison algorithm
                    // step 3 and 16 - 17
                    if (target.LimitType == typeof(string) && arg.LimitType == typeof(string))
                    {
                        //TODO: implement string comparison
                        throw new NotImplementedException("String comparison not implemented");
                    }
                    // step 4 - 15 and 18 - 21
                    else
                    {
                        expr = Et.MakeBinary(
                            Operation,
                            // step 1 - 2 are implicit in ToNumber
                            //TODO: verify handling of NaN and Infinity
                            //TODO: might need a double.NaN > Js.Undefined.Instance cast
                            TypeConverter.ToNumber(target),
                            TypeConverter.ToNumber(arg)
                        );
                    }
                    break;

                // 11.9.1
                case ExpressionType.Equal:
                    expr = Equality(target, arg, ref typeRestriction);
                    break;

                // 11.9.2
                case ExpressionType.NotEqual:
                    expr = Et.Not(Equality(target, arg, ref typeRestriction));
                    break;

                // 11.10
                case ExpressionType.And: 
                case ExpressionType.Or:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                    //TODO: convert to number/check so we're not casting aa Js.Obj, string, etc. to double > int
                    expr = Et.Convert(
                        Et.MakeBinary(
                            Operation,
                            EtUtils.CastForBitOp(target.Expression),
                            EtUtils.CastForBitOp(arg.Expression)
                        ),
                        typeof(double) // and then back to double here, 
                                       // since all JS numbers are doubles
                    );
                    break;

            }

            Restrict restrictions;

            if (typeRestriction)
            {
                restrictions =
                    Restrict.GetTypeRestriction(
                        target.Expression,
                        target.LimitType
                    ).Merge(
                        Restrict.GetTypeRestriction(
                            arg.Expression,
                            arg.LimitType
                        )
                    );
            }
            else
            {
                restrictions =
                    Restrict.GetInstanceRestriction(
                        target.Expression,
                        target.Value
                    ).Merge(
                        Restrict.GetInstanceRestriction(
                            arg.Expression,
                            arg.Value
                        )
                    );
            }

            return new Meta(
                EtUtils.Box(expr), 
                restrictions
            );
        }

        /// <summary>
        /// This function implements the horrible JavaScript equality comparison
        /// </summary>
        /// <param name="target"></param>
        /// <param name="arg"></param>
        /// <param name="typeRestriction"></param>
        /// <returns></returns>
        private Et Equality(Meta target, Meta arg, ref bool typeRestriction)
        {
            //TODO: verify all boolean cases from 11.9.3
            
            var targetAsLimit = Et.Convert(target.Expression, target.LimitType);
            var argAsLimit = Et.Convert(arg.Expression, arg.LimitType);

            // step 1
            if (target.LimitType == arg.LimitType)
            {
                // step 2
                if (target.LimitType == typeof(Js.Undefined))
                {
                    return Et.Constant(true, typeof(object));
                }
                // step 3 (Null is a type in Js, which it isnt here)
                else if (target.Value == null && arg.Value == null)
                {
                    typeRestriction = false; // null requires instance checking
                    return Et.Constant(true, typeof(object));
                }
                // step 4 - 12
                //TODO: verify step 4 - 12 translates into "target.Equals(arg)"
                else if (
                    target.LimitType == typeof(double)
                 || target.LimitType == typeof(string)
                 || target.LimitType == typeof(bool)
                )
                {
                    return Et.MakeBinary(
                        ExpressionType.Equal,
                        targetAsLimit,
                        argAsLimit
                    );
                }
                // step 13
                else
                {
                    return Et.Call(
                        typeof(object).GetMethod("ReferenceEquals"),
                        EtUtils.Box(target.Expression),
                        EtUtils.Box(arg.Expression)
                    );
                }
            }
            else
            {
                // step 14 - 15
                if (
                    (target.Value == null && arg.LimitType == typeof(Js.Undefined))
                 || (target.LimitType == typeof(Js.Undefined) && arg.Value == null)
                )
                {
                    typeRestriction = false; // null requires instance checking, 
                                             // and checking undefined for instance 
                                             // is ok because there is only one
                    return Et.Constant(true, typeof(object));
                }
                // step 16 - 19
                else if (
                    (target.LimitType == typeof(double) && arg.LimitType == typeof(string))
                 || (target.LimitType == typeof(string) && arg.LimitType == typeof(double))
                 || (target.LimitType == typeof(double) && arg.LimitType == typeof(bool))
                 || (target.LimitType == typeof(bool) && arg.LimitType == typeof(double))
                )
                {
                    return Et.Equal(
                        TypeConverter.ToNumber(target),
                        TypeConverter.ToNumber(arg)
                    );
                }
                // step 20
                else if (arg.LimitType == typeof(Js.Obj))
                {
                    if (target.LimitType == typeof(double))
                    {
                        return Et.Equal(
                            targetAsLimit,
                            TypeConverter.ToNumber(arg)
                        );
                    }
                    else if (target.LimitType == typeof(string))
                    {
                        return Et.Equal(
                            targetAsLimit,
                            TypeConverter.ToString(arg)
                        );
                    }
                    else
                    {
                        return Et.Constant(false, typeof(object));
                    }
                }
                // step 21
                else if (target.LimitType == typeof(Js.Obj))
                {
                    if (arg.LimitType == typeof(double))
                    {
                        return Et.Equal(
                            TypeConverter.ToNumber(target),
                            argAsLimit
                        );
                    }
                    else if (arg.LimitType == typeof(string))
                    {
                        return Et.Equal(
                            TypeConverter.ToString(target),
                            argAsLimit
                        );
                    }
                    else
                    {
                        return Et.Constant(false, typeof(object));
                    }
                }
                // step 22
                // this isn't identical to step 22
                // but we need to be able to compare 
                // .NET objects
                else
                {
                    return Et.Call(
                        target.Expression,
                        typeof(object).GetMethod("Equals"),
                        arg.Expression
                    );
                }
            }
        }
    }
}
