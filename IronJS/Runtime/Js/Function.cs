using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;

    public class Function : Obj, IFunction
    {
        public Scope Scope { get; protected set; }
        public Lambda Lambda { get; protected set; }

        public Function(Scope scope, Lambda lambda)
        {
            Scope = scope;
            Lambda = lambda;
            SetOwnProperty("length", (double) lambda.Params.Length);
        }

        #region IFunction Members

        public object Call(IObj that, object[] args)
        {
            var callScope = Scope.Enter();
            var argsObject = Context.ObjectConstructor.Construct();

            callScope.Local("this", that);
            callScope.Local("arguments", argsObject);
            argsObject.SetOwnProperty("length", args.Length);

            for (var i = 0; i < args.Length; ++i)
            {
                if (i < Lambda.Params.Length)
                    callScope.Local(Lambda.Params[i], args[i]);

                argsObject.SetOwnProperty((double)i, args[i]);
            }

            return Lambda.Delegate.Invoke(callScope);
        }

        public virtual IObj Construct(object[] args)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Expression Tree

        public static Et EtScope(Et obj)
        {
            return Et.Property(
                EtUtils.Cast<IFunction>(obj),
                "Scope"
            );
        }

        public static Et EtLambda(Et obj)
        {
            return Et.Property(
                EtUtils.Cast<IFunction>(obj),
                "Lambda"
            );
        }

        public static Et EtDelegate(Et obj)
        {
            return Et.Property(
                EtLambda(obj),
                "Delegate"
            );
        }

        public static Et EtCall(Et obj, Et scope)
        {
            return Et.Call(
                EtDelegate(obj),
                typeof(LambdaType).GetMethod("Invoke"),
                scope
            );
        }

        #endregion 

        #region IDynamicMetaObjectProvider Members

        public Meta GetMetaObject(Et parameter)
        {
            return new IFunctionMeta(parameter, this);
        }

        #endregion
    }
}
