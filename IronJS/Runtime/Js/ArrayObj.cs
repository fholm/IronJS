using System;
using System.Collections.Generic;
using System.Text;

namespace IronJS.Runtime.Js
{
    public class ArrayObj : Obj
    {
        protected object[] Vector;

        public ArrayObj()
        {
            Vector = new object[0];
            Properties["length"] = new Property(0.0D);
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
                var strval = (string)name;

                if (Int32.TryParse(strval, out intval))
                {
                    if (intval.ToString() == strval)
                        return intval;
                }
            }

            return -1;
        }

        object PutArray(int index, object value)
        {
            if (index >= Vector.Length)
            {
                var newLength = Vector.Length > 0 ? Vector.Length * 2 : 2;
                var newVector = new object[newLength < index ? index + 1 : newLength];
                Array.Copy(Vector, newVector, Vector.Length);
                Vector = newVector;
                Properties["length"].Value = (double)index + 1;
            }

            return Vector[index] = value;
        }

        object PutProperty(object name, object value)
        {
            if (name is string && (string)name == "length")
            {
                //TODO: should check length for being a proper number here
                var intval = (int)(double)value;

                // shrink
                if (intval < Vector.Length)
                {
                    var newVector = new object[intval];
                    Array.Copy(Vector, newVector, intval);
                    Vector = newVector;
                    Properties["length"].Value = (double)intval;
                }

                return value;
            }

            return base.Get(name);
        }

        object GetArray(int index)
        {
            if (index < Vector.Length)
            {
                if (Vector[index] != null)
                    return Vector[index];
            }

            return Js.Undefined.Instance;
        }

        object GetProperty(object name)
        {
            return base.Get(name);
        }

        #region IObj Members

        public override object Get(object name)
        {
            var index = AsArrayIndex(name);

            // not array index
            if (index == -1)
                return GetProperty(name);

            // array index
            return GetArray(index);
        }

        public override object Put(object name, object value)
        {
            var index = AsArrayIndex(name);

            // not array index
            if (index == -1)
                return PutProperty(name, value);

            // array index
            return PutArray(index, value);
        }

        public override bool CanPut(object name)
        {
            //TODO: this should properly use attributes
            return true;
        }

        public override bool HasProperty(object name)
        {
            var index = AsArrayIndex(name);

            // not array index
            if (index == -1)
                return base.HasProperty(name);

            // array index
            return index < Vector.Length;
        }

        public override bool Delete(object name)
        {
            var index = AsArrayIndex(name);

            // not array index
            if (index == -1)
                return base.Delete(name);

            // array index
            if (index < Vector.Length)
            {
                Vector[index] = null;
                return true;
            }

            return false;
        }

        public override bool HasOwnProperty(object name)
        {
            var index = AsArrayIndex(name);

            // not array index
            if (index == -1)
                return base.HasOwnProperty(name);

            // array index
            return index < Vector.Length;
        }

        public override object SetOwnProperty(object name, object value)
        {
            var index = AsArrayIndex(name);

            // not array index
            if (index == -1)
                return base.SetOwnProperty(name, value);

            // array index
            return PutArray(index, value);
        }

        public override object GetOwnProperty(object name)
        {
            var index = AsArrayIndex(name);

            // not array index
            if (index == -1)
                return base.GetOwnProperty(name);

            // array index
            return GetArray(index);
        }

        public override List<object> GetAllPropertyNames()
        {
            var baseProps = base.GetAllPropertyNames();

            for (int i = 0; i < Vector.Length; ++i)
                baseProps.Add((object)(double)i);

            return baseProps;
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
