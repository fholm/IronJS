using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Builtins
{
    public class String_ctor : Obj, IFunction
    {
        public IObj String_prototype { get; private set; }

        protected String_ctor(Context context)
        {
            Context = context;
            Class = ObjClass.Function;

            String_prototype = new String_prototype(Context);
            String_prototype.SetOwnProperty("constructor", this);

            SetOwnProperty("prototype", String_prototype);
            SetOwnProperty("fromCharCode", new String_fromCharCode(Context));
        }

        #region IFunction Members

        public object Call(IObj that, object[] args)
        {
            if (args.Length > 0)
                return JsTypeConverter.ToString(args[0]);

            return "";
        }

        public IObj Construct()
        {
            return Construct(null);
        }

        public IObj Construct(object[] args)
        {
            var obj = new ValueObj(args.Length > 0 ? JsTypeConverter.ToString(args[0]) : "");

            obj.Class = ObjClass.String;
            obj.Prototype = String_prototype;
            obj.Context = Context;

            return obj;
        }

        public bool HasInstance(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDynamicMetaObjectProvider Members

        public Meta GetMetaObject(Et parameter)
        {
            return new IFunctionMeta(parameter, this);
        }

        #endregion

        #region Static

        static public String_ctor Create(Context context)
        {
            return new String_ctor(context);
        }

        #endregion 
    }
}
