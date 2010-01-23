using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Builtins
{
    public class Array_ctor : Obj, IFunction
    {
        public IObj Array_prototype { get; private set; }

        protected Array_ctor(Context context)
        {
            Class = ObjClass.Function;
            Context = context;
            
            // 15.4.3
            Prototype = context.FunctionConstructor.Function_prototype;

            Array_prototype = new Array_prototype(context);
            Array_prototype.SetOwnProperty("constructor", this);

            // 15.4.3
            SetOwnProperty("length", 1.0D);

            // 15.4.3.1
            SetOwnProperty("prototype", Array_prototype);
        }

        #region IFunction Members

        public object Call(IObj that, object[] args)
        {
            // 15.4.1.1
            return Construct(args);
        }

        public IObj Construct()
        {
            return Construct(null);
        }

        public IObj Construct(object[] args)
        {
            var arrayObj = new ArrayObj();

            arrayObj.Class = ObjClass.Array;
            arrayObj.Prototype = Array_prototype;
            arrayObj.Context = Context;

            if (args != null && args.Length != 1)
            {
                // 15.4.2.1
                for (int i = 0; i < args.Length; ++i)
                    arrayObj.SetOwnProperty((double)i, args[i]);
            }
            else if(args != null)
            {
                // 15.4.2.2
                var len = JsTypeConverter.ToNumber(args[0]);

                if ((double)JsTypeConverter.ToInt32(len) == len)
                {
                    arrayObj.SetOwnProperty("length", len);
                }
                else
                {
                    throw new ShouldThrowTypeError();
                }
            }

            return arrayObj;
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

        static public Array_ctor Create(Context context)
        {
            return new Array_ctor(context);
        }

        #endregion 
    }
}
