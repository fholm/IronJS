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
            Context = context;

            Array_prototype = new Array_prototype(context);
            Array_prototype.SetOwnProperty("constructor", this);

            Put("prototype", Array_prototype);
        }

        #region IFunction Members

        public object Call(IObj that, object[] args)
        {
            throw new NotImplementedException();
        }

        public IObj Construct(object[] args)
        {
            if (args.Length == 0)
            {
                var arrayObj = new ArrayObj();

                arrayObj.Class = ObjClass.Array;
                arrayObj.Prototype = Array_prototype;
                arrayObj.Context = Context;

                return arrayObj;
            }

            throw new NotImplementedException();
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
