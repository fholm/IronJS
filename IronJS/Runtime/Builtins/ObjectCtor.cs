using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Scripting.Utils;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Restrict = System.Dynamic.BindingRestrictions;
using EtParam = System.Linq.Expressions.ParameterExpression;

namespace IronJS.Runtime.Builtins
{
    public class ObjectCtor : Obj, IFunction
    {
        public IObj Object_prototype { get; private set; }

        protected ObjectCtor(Context context)
        {
            Context = context;
            Object_prototype = CreatePrototype();
            Put("prototype", Object_prototype);
        }

        public IObj Construct()
        {
            return Construct(null);
        }

        protected IObj CreatePrototype()
        {
            var obj = new Obj();

            obj.Class = ObjClass.Object;
            obj.Prototype = null;
            obj.Context = Context;

            return obj;
        }

        #region IFunction Members

        // 15.2.1.1
        public object Call(IObj that, object[] args)
        {
            // step 1
            if (args.Length == 0 
                || args[0] == null 
                || args[0] is Undefined)
                return Construct(args);

            // step 2
            return JsTypeConverter.ToObject(args[0], Context);
        }

        public IObj Construct(object[] args)
        {
            // step 8 (verification)
            if (args != null
                && args.Length > 0 
                && args[0] != null 
                && !(args[0] is Undefined))
            {
                var value = args[0];

                // step 3
                if (value is IObj)
                    return (IObj)value;

                // step 5, 6 and 7
                if (value is double || value is string || value is bool)
                    return JsTypeConverter.ToObject(value, Context);

                // step 4
                throw new NotImplementedException("Can't convert value of type '" + args[0].GetType() + "' to IObj");
            }

            // step 8
            var obj = Context.CreateObject();
            obj.Prototype = Object_prototype;
            return obj;
        }

        #endregion

        #region IDynamicMetaObjectProvider Members

        public Meta GetMetaObject(Et parameter)
        {
            return new IFunctionMeta(parameter, this);
        }

        #endregion

        #region Static

        static public ObjectCtor Create(Context context)
        {
            return new ObjectCtor(context);
        }

        #endregion 
    }
}
