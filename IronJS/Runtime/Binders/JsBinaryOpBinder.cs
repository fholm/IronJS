using System;
using System.Dynamic;
using System.Linq.Expressions;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;
using Restrict = System.Dynamic.BindingRestrictions;

namespace IronJS.Runtime.Binders
{

    class JsBinaryOpBinder : BinaryOperationBinder
    {
        Context _context;

        public JsBinaryOpBinder(ExpressionType op, Context context)
            : base(op)
        {
            _context = context;
        }

        public override Meta FallbackBinaryOperation(Meta target, Meta arg, Meta error)
        {
            //TODO: insert defer
            //TODO: optimize common double + double case for all operations
            //TODO: handle infinity + zero stuff correct

            Et expr = null;
            var typeRestriction = true;

            switch (Operation)
            {
                // 11.6.1
                case ExpressionType.Add:
                    // step 12 - 15
                    if (target.LimitType == typeof(string) || arg.LimitType == typeof(string))
                    {
                        expr = Et.Call(
                            typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) }),
                            JsTypeConverter.EtToString(target),
                            JsTypeConverter.EtToString(arg)
                        );
                    }
                    // step 1 - 6 and 8 - 11
                    else
                    {
                        //TODO: verify handling of NaN and Infinity
                        expr = Et.Add(
                            JsTypeConverter.EtToNumber(target),
                            JsTypeConverter.EtToNumber(arg)
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
                        JsTypeConverter.EtToNumber(target),
                        JsTypeConverter.EtToNumber(arg)
                    );
                    break;

                case ExpressionType.LessThan: // 11.8.1
                case ExpressionType.GreaterThan: // 11.8.2
                case ExpressionType.LessThanOrEqual: // 11.8.3
                case ExpressionType.GreaterThanOrEqual: // 11.8.4
                    // 11.8.5 comparison algorithm
                    // step 3 and 16 - 21
                    if (target.LimitType == typeof(string) && arg.LimitType == typeof(string))
                    {
                        expr = Et.Call(
                            typeof(string).GetMethod("Compare", new[] { typeof(string), typeof(string) }),
                            EtUtils.Cast<string>(target.Expression),
                            EtUtils.Cast<string>(arg.Expression)
                        );

                        switch(Operation)
                        {
                            case ExpressionType.LessThan:
                                expr = Et.Equal(
                                    expr,
                                    Et.Constant(-1)
                                );
                                break;

                            case ExpressionType.GreaterThan:
                                expr = Et.Equal(
                                    expr,
                                    Et.Constant(1)
                                );
                                break;
                            
                            case ExpressionType.LessThanOrEqual:
                                expr = Et.LessThanOrEqual(
                                    expr,
                                    Et.Constant(0)
                                );
                                break;
                            
                            case ExpressionType.GreaterThanOrEqual:
                                expr = Et.LessThanOrEqual(
                                    expr,
                                    Et.Constant(0)
                                );
                                break;

                            default:
                                throw new NotImplementedException("This should never happend");
                        }
                    }
                    // step 4 - 15
                    else
                    {
                        expr = Et.MakeBinary(
                            Operation,
                            // step 1 - 2 are implicit in ToNumber
                            //TODO: verify handling of NaN and Infinity
                            //TODO: might need a double.NaN > Js.Undefined.Instance cast
                            JsTypeConverter.EtToNumber(target),
                            JsTypeConverter.EtToNumber(arg)
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
                    //TODO: convert to number/check so we're not casting a Js.IObj, string, etc. to double > int
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
                    RestrictUtils.GetNullHandledTypeRestriction(
                        target
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
        /// This function implements the horrible JavaScript abstract equality comparison
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
                        JsTypeConverter.EtToNumber(target),
                        JsTypeConverter.EtToNumber(arg)
                    );
                }
                // step 20
                else if (arg.LimitType == typeof(Js.IObj))
                {
                    if (target.LimitType == typeof(double))
                    {
                        return Et.Equal(
                            targetAsLimit,
                            JsTypeConverter.EtToNumber(arg)
                        );
                    }
                    else if (target.LimitType == typeof(string))
                    {
                        return Et.Equal(
                            targetAsLimit,
                            JsTypeConverter.EtToString(arg)
                        );
                    }
                    else
                    {
                        return Et.Constant(false, typeof(object));
                    }
                }
                // step 21
                else if (target.LimitType == typeof(Js.IObj))
                {
                    if (arg.LimitType == typeof(double))
                    {
                        return Et.Equal(
                            JsTypeConverter.EtToNumber(target),
                            argAsLimit
                        );
                    }
                    else if (arg.LimitType == typeof(string))
                    {
                        return Et.Equal(
                            JsTypeConverter.EtToString(target),
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
