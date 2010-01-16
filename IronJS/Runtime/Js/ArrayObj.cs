using System;
using System.Collections.Generic;
using System.Text;

namespace IronJS.Runtime.Js
{
    public class ArrayObj : IObj
    {
        protected object[] Array;
        protected readonly Dictionary<object, Property> Properties =
                       new Dictionary<object, Property>();

        public ArrayObj()
        {
            Array = new object[8];
        }

        int AsArrayIndex(object name)
        {
            if (name is double)
            {
                var dbl = (double)name;
                var intval = (int)dbl;

                if (dbl == (double)intval)
                    return intval;
            }

            if (name is string)
            {
                int intval;

                if(Int32.TryParse((string)name, out intval))
                {
                    if (intval.ToString() == name)
                        return intval;
                }
            }

            return -1;
        }

        object PutArray(int index, object value)
        {
            throw new NotImplementedException();
        }

        object PutProperty(object name, object value)
        {
            throw new NotImplementedException();
        }

        object GetArray(int index)
        {
            throw new NotImplementedException();
        }

        object GetProperty(object name)
        {
            throw new NotImplementedException();
        }

        #region IObj Members

        public ObjClass Class { get; protected set; }
        public IObj Prototype { get; protected set; }
        public Context Context { get; protected set; }

        public object Get(object name)
        {
            var index = AsArrayIndex(name);

            // not array index
            if (index == -1)
                return GetProperty(name);

            // array index
            return GetArray(index);
        }

        public object Put(object name, object value)
        {
            var index = AsArrayIndex(name);

            // not array index
            if (index == -1)
                return PutProperty(name, value);

            // array index
            return PutArray(index, value);
        }

        public bool CanPut(object name)
        {
            throw new NotImplementedException();
        }

        public bool HasProperty(object name)
        {
            throw new NotImplementedException();
        }

        public bool Delete(object name)
        {
            throw new NotImplementedException();
        }

        public object DefaultValue(ValueHint hint)
        {
            throw new NotImplementedException();
        }

        public bool HasOwnProperty(object name)
        {
            throw new NotImplementedException();
        }

        public object SetOwnProperty(object name, object value)
        {
            throw new NotImplementedException();
        }

        public object GetOwnProperty(object name)
        {
            throw new NotImplementedException();
        }

        public List<object> GetAllPropertyNames()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDynamicMetaObjectProvider Members

        public System.Dynamic.DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression parameter)
        {
            return new IObjMeta(parameter, this);
        }

        #endregion
    }
}
