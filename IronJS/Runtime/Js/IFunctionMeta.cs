using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using IronJS.Runtime.Binders;
using IronJS.Runtime.Utils;
using IronJS.Runtime.Js.Utils;
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
            var callFrame = Et.Variable(typeof(IFrame), "#callframe");
            var argsObj = Et.Variable(typeof(IObj), "#arguments");
            var tmp = Et.Variable(typeof(IObj), "#tmp");

            return new Meta(
                Et.Block(
                    new[] { tmp, callFrame, argsObj },

                    // create our new object
                    Et.Assign(
                        tmp,
                        Et.Call(
                            selfObj.ContextExpr(),
                            Context.Methods.CreateObjectCtor,
                            selfExpr
                        )
                    ),

                    // create a new empty call frame
                    FrameUtils.Enter(
                        callFrame,
                        IFunctionEtUtils.Frame(selfExpr)
                    ),

                    // create our new 'arguments' object
                    Et.Assign(
                        argsObj,
                        Et.Call(
                            selfObj.ContextExpr(),
                            Context.Methods.CreateObject
                        )
                    ),

                    // block that setups our call frame + arguments objects
                    IFunctionEtUtils.BuildFrameBlock(
                        callFrame,
                        argsObj,
                        selfObj.Lambda.Params,
                        args
                    ),

                    // push 'this' variable on call frame
                    FrameUtils.Push(
                        callFrame,
                        "this",
                        tmp,
                        VarType.Local
                    ),

                    // the actual constructor call
                    IFunctionEtUtils.Call(
                        this.Expression,
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
            var callFrame = Et.Variable(typeof(IFrame), "#callframe");
            var argsObj = Et.Variable(typeof(IObj), "#arguments");

            return new Meta(
                Et.Block(
                    new[] { callFrame, argsObj },

                    // create a new empty call frame
                    FrameUtils.Enter(
                        callFrame,
                        IFunctionEtUtils.Frame(selfExpr)
                    ),

                    // create our new 'arguments' object
                    Et.Assign(
                        argsObj,
                        Et.Call(
                            selfObj.ContextExpr(),
                            Context.Methods.CreateObject
                        )
                    ),
                    
                    // block that setups our call frame + arguments objects
                    IFunctionEtUtils.BuildFrameBlock(
                        callFrame,
                        argsObj,
                        selfObj.Lambda.Params,
                        args
                    ),

                    // push 'this' variable on call frame
                    FrameUtils.Push(
                        callFrame,
                        "this",
                        Et.Default(typeof(object)),
                        VarType.Local
                    ),

                    // the actual constructor call
                    IFunctionEtUtils.Call(
                        this.Expression,
                        callFrame
                    )
                ),
                IFunctionEtUtils.BuildLambdaCallRestriction(this, args)
            );
        }
    }
}
