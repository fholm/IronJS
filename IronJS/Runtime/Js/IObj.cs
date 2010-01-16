using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using IronJS.Extensions;
using IronJS.Runtime;
using IronJS.Runtime.Binders;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Microsoft.Scripting.Utils;

namespace IronJS.Runtime.Js
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = System.Linq.Expressions.Expression;

    //TODO: need support for 'Host' object class
    public enum ObjClass { Object, Function, Boolean, Number, String, Math, Array }

    //
    public enum ValueHint { None, Number, String }

    public interface IObj : IDynamicMetaObjectProvider
    {
        // 8.6.2
        ObjClass Class { get; } // [[Class]]
        IObj Prototype { get; } // [[Prototype]]

        // implementation specific
        Context Context { get; }

        // 8.6.2
        object Get(object name);                // [[Get]]
        object Put(object name, object value);  // [[Put]]
        bool CanPut(object name);               // [[CanPut]]
        bool HasProperty(object name);          // [[HasProperty]]
        bool Delete(object name);               // [[Delete]]
        object DefaultValue(ValueHint hint);    // [[DefaultValue]]

        // implementation specific
        bool HasOwnProperty(object name);
        object SetOwnProperty(object name, object value);
        object GetOwnProperty(object name);
        List<object> GetAllPropertyNames();
    }

    public static class IObjMethods
    {
        public static bool HasValue(this IObj obj)
        {
            return (obj is IValueObj);
        }

        public static bool IsFunction(this IObj obj)
        {
            return (obj is IFunction);
        }

        public static Et ContextExpr(this IObj obj)
        {
            return Et.Constant(obj.Context, typeof(Context));
        }

        public static object CallMethod(this IObj obj, object propertyName, params object[] args)
        {
            var target = obj.Get(propertyName);

            if (target is IFunction)
            {
                var targetFunc = (IFunction)target;
                var callFrame = new Frame(targetFunc.Frame);
                var argsObj = obj.Context.CreateObject();

                for (int i = 0; i < args.Length; ++i)
                {
                    argsObj.SetOwnProperty(i, args[i]);

                    if (i < targetFunc.Lambda.Params.Count)
                        callFrame.Push(
                            targetFunc.Lambda.Params[i], 
                            args[i], 
                            VarType.Local
                        );
                }

                callFrame.Push(
                    "arguments", 
                    argsObj, 
                    VarType.Local
                );

                return targetFunc.Lambda.Delegate(obj, callFrame);
            }

            throw new NotImplementedException();
        }
    }
}
