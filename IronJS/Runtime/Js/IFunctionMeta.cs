using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using IronJS.Runtime.Binders;
using IronJS.Runtime.Utils;
using Microsoft.Scripting.Utils;

namespace IronJS.Runtime.Js
{
    using Et = System.Linq.Expressions.Expression;
    using Parm = System.Linq.Expressions.ParameterExpression;
    using Meta = System.Dynamic.DynamicMetaObject;
    using Restrict = System.Dynamic.BindingRestrictions;
    using AstUtils = Microsoft.Scripting.Ast.Utils;

    class IFunctionMeta : IObjMeta
    {
        public IFunctionMeta(Et parameter, IFunction function)
            : base(parameter, function)
        {

        }

        public override Meta BindCreateInstance(CreateInstanceBinder binder, Meta[] args)
        {
            //TODO: insert defer
            var selfExpr = EtUtils.Cast<IFunction>(this.Expression);
            var selfObj = (IFunction)this.Value;

            // tmp variables
            var callFrame = Et.Variable(typeof(Scope), "#callframe");
            var tmp = Et.Variable(typeof(IObj), "#tmp");

            return new Meta(
                Et.Call(
                    selfExpr,
                    IFunctionMethods.MiConstruct,
                    AstUtils.NewArrayHelper(
                        typeof(object),
                        DynamicUtils.GetExpressions(args)
                    )
                ),
                RestrictUtils.BuildCallRestrictions(
                    this,
                    args,
                    RestrictFlag.Type
                )
            );

            /*
            return new Meta(
                Et.Block(
                    new[] { tmp, callFrame },

                    // create our new object
                    Et.Assign(
                        tmp,
                        Et.Call(
                            selfObj.ContextExpr(),
                            Context.Methods.CreateObjectCtor,
                            selfExpr
                        )
                    ),

                    IFunctionEtUtils.SetupCallBlock(
                        callFrame,
                        selfExpr,
                        selfObj,
                        args
                    ),

                    // the actual constructor call
                    IFunctionEtUtils.Call(
                        this.Expression,
                        tmp,
                        callFrame
                    ),

                    // return the built object
                    tmp
                ),

                // builds a call restriction that restricts on:
                // 1) type of this.Expression
                // 2) expression type of all meta objects in 'args'
                // 3) instance of this.Value.Lambda
                IFunctionEtUtils.BuildLambdaCallRestriction(this, args)
            );
            */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public override Meta BindInvoke(InvokeBinder binder, Meta[] args)
        {
            //TODO: insert defer
            var selfExpr = EtUtils.Cast<IFunction>(this.Expression);
            var selfObj = (IFunction)this.Value;

            // tmp variables
            var callScope = Et.Variable(typeof(Scope), "#callscope");

            return new Meta(
                Et.Block(
                    new[] { callScope },

                    EtCreateCallBlock(
                        callScope,
                        selfExpr,
                        selfObj,
                        args
                    ),

                    Function.EtCall(
                        selfExpr,
                        callScope
                    )
                ),

                EtLambdaCallRestriction(this, args)
            );
        }

        #region Expression Tree

        static internal Restrict EtLambdaCallRestriction(Meta target, Meta[] args)
        {
            return
                RestrictUtils.BuildCallRestrictions(
                    target,
                    args,
                    RestrictFlag.Type
                ).Merge(
                    Restrict.GetInstanceRestriction(
                        Function.EtLambda(target.Expression),
                        (target.Value as IFunction).Lambda
                    )
                );
        }

        internal static Et EtCreateCallBlock(Parm callScope, Et selfExpr, IFunction selfObj, Meta[] args)
        {
            var argsObj = Et.Variable(typeof(IObj), "#arguments");

            return Et.Block(
                new[] { argsObj },

                // create call scope
                Et.Assign(
                    callScope,
                    Scope.EtNew(
                        selfObj.ContextExpr(),
                        Function.EtScope(selfExpr)
                    )
                ),

                // create our 'arguments' object
                Et.Assign(
                    argsObj,
                    Et.Call(
                        selfObj.ContextExpr(),
                        Context.Methods.CreateObject
                    )
                ),

                // block that setups our call scope + arguments objects
                EtCreateCallFrame(
                    callScope,
                    argsObj,
                    selfObj.Lambda.Params,
                    args
                )
            );
        }

        static internal Et EtCreateCallFrame(Parm callScope, Parm argsObj, string[] paramNames, Meta[] args)
        {
            return EtCreateCallFrame(callScope, argsObj, paramNames, DynamicUtils.GetExpressions(args));
        }

        static internal Et EtCreateCallFrame(Parm callScope, Parm argsObj, string[] paramNames, Et[] args)
        {
            var exprs = new List<Et>();
            var that = args[0];

            // remove 'this' parameter 
            args = ArrayUtils.RemoveFirst(args);

            // Set length property
            exprs.Add(
                IObjMethods.EtSetOwnProperty(
                    argsObj,
                    "length",
                    Et.Constant((double)args.Length, typeof(object))
                )
            );

            // push arguments on scope
            exprs.Add(
                Scope.EtLocal(
                    callScope,
                    "arguments",
                    argsObj
                )
            );

            for (int i = 0; i < args.Length; ++i)
            {
                // only args with param names
                // should be pushed on the scope
                if (i < paramNames.Length)
                {
                    exprs.Add(
                        Scope.EtLocal(
                            callScope,
                            paramNames[i],
                            args[i]
                        )
                    );
                }

                // push arg on 'arguments'-object
                exprs.Add(
                    IObjMethods.EtSetOwnProperty(
                        argsObj,
                        i,
                        EtUtils.Box(args[i])
                    )
                );
            }

            // last, add 'this' variable
            exprs.Add(
                Scope.EtLocal(
                    callScope,
                    "this",
                    that
                )
            );

            return Et.Block(exprs);
        }

        #endregion 
    }
}
