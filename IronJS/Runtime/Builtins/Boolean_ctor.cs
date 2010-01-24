using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Builtins
{
    public class Boolean_ctor : Obj, IFunction
    {
        public IObj Boolean_prototype { get; private set; }

        protected Boolean_ctor(Context context)
        {
            Context = context;
            Class = ObjClass.Function;

            Boolean_prototype = new Boolean_prototype(Context);
            Boolean_prototype.SetOwnProperty("constructor", this);

            SetOwnProperty("prototype", Boolean_prototype);
        }

        #region IFunction Members

        public object Call(IObj that, object[] args)
        {
            return args != null && args.Length > 0 ? JsTypeConverter.ToBoolean(args[0]) : false;
        }

        public IObj Construct()
        {
            return Construct(null);
        }

        public IObj Construct(object[] args)
        {
            var bol = args != null && args.Length > 0 ? JsTypeConverter.ToBoolean(args[0]) : false;
            var obj = new ValueObj(bol);

            obj.Class = ObjClass.Boolean;
            obj.Prototype = Boolean_prototype;
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

        static public Boolean_ctor Create(Context context)
        {
            return new Boolean_ctor(context);
        }

        #endregion
    }
}
