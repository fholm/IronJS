using System;
using System.Dynamic;
using System.Reflection;
using IronJS.Compiler.Ast;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Binders2
{
    public class JsInvokeBinder2 : InvokeBinder, IExpressionSerializable
    {
        public JsInvokeBinder2(CallInfo callInfo)
            : base(callInfo)
        {

        }

        public override Meta FallbackInvoke(Meta target, Meta[] args, Meta errorSuggestion)
        {
            if (target.Value is Delegate)
            {
                var type = target.Value.GetType();
                var invoke = type.GetMethod("Invoke");

                return new Meta(
                    EtUtils.Box2(
                        Et.Call(
                            Et.Convert(target.Expression, type),
                            invoke,
                            Et.Constant((IjsObj)args[0].Value, typeof(IjsObj))
                        )
                    ),
                    RestrictUtils.BuildCallRestrictions(
                        target,
                        args,
                        RestrictFlag.Type
                    )
                );
            }


            //TODO: refine this so it works properly... very ugly now
            var methodInfo = target.Value as MethodInfo;

            if (methodInfo == null)
                if (target.Value is IjsFunc)
                    methodInfo = (target.Value as IjsFunc).MethodInfo;

            if (methodInfo != null)
            {
                var closure = target.Value.GetType().GetField("Closure");

                if (closure == null)
                {
                    return new Meta(
                        EtUtils.Box(
                            Et.Call(
                                methodInfo,
                                EtUtils.ConvertToParamTypes(
                                    args,
                                    methodInfo.GetParameters()
                                )
                            )
                        ),
                        RestrictUtils.BuildCallRestrictions(
                            target,
                            args,
                            RestrictFlag.Instance
                        )
                    );
                }
                else
                {
                    return new Meta(
                        EtUtils.Box(
                            Et.Call(
                                methodInfo,
                                EtUtils.ConvertToParamTypes(
                                    ArrayUtils.Append(
                                        DynamicUtils.GetExpressions(args),
                                        Et.Field(
                                            Et.Convert(target.Expression, target.LimitType),
                                            closure
                                        )
                                    ),
                                    methodInfo.GetParameters()
                                )
                            )
                        ),
                        RestrictUtils.BuildCallRestrictions(
                            target,
                            args,
                            RestrictFlag.Instance
                        )
                    );
                }
            }

            throw new NotImplementedException();
        }

        public static JsInvokeBinder2 Create(int argCount)
        {
            return new JsInvokeBinder2(new CallInfo(argCount));
        }

        #region IExpressionSerializable Members

        public Et CreateExpression()
        {
            return AstUtils.SimpleCallHelper(
                GetType().GetMethod("Create"),
                Et.Constant(1)
            );
        }

        #endregion
    }
}
