using System;
using System.Collections.Generic;
using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Js
{
    public class ArrayObj : Obj
    {
        public ArrayObj()
        {
            Properties["length"] = new Property(0.0D);
        }

        internal int Length
        {
            get { return (int)(double)GetOwnProperty("length"); }
            set { Properties["length"].Value = (double)value; }
        }

        object UpdateLength(object value)
        {
            var intval = JsTypeConverter.ToInt32(value);

            if ((double)intval != JsTypeConverter.ToNumber(value))
                throw new ShouldThrowTypeError();

            var length = Length - 1;

            for (; length >= intval; --length)
                Delete((double)length);

            Properties["length"].Value = (double)intval;

            return intval;
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

        #region IObj Members

        public override object Get(object name)
        {
            var index = AsArrayIndex(name);

            if (index == -1)
                return base.Get(name);

            return base.Get((double)index);
        }

        public override object Put(object name, object value)
        {
            var index = AsArrayIndex(name);

            if (index == -1)
            {
                if (name is string && (string)name == "length")
                    return UpdateLength(value);

                return base.Put(name, value);
            }

            if (index >= Length)
                Length = index + 1;

            return base.Put((double)index, value);
        }

        public override bool HasProperty(object name)
        {
            var index = AsArrayIndex(name);

            if (index == -1)
                return base.HasProperty(name);

            return base.HasProperty((double)index);
        }

        public override bool Delete(object name)
        {
            var index = AsArrayIndex(name);

            if (index == -1)
                return base.Delete(name);

            return base.Delete((double)index);
        }

        public override bool HasOwnProperty(object name)
        {
            var index = AsArrayIndex(name);

            if (index == -1)
                return base.HasOwnProperty(name);

            return base.HasOwnProperty((double)index);
        }

        public override object SetOwnProperty(object name, object value)
        {
            var index = AsArrayIndex(name);

            if (index == -1)
            {
                if (name is string && (string)name == "length")
                    return UpdateLength(value);

                return base.SetOwnProperty(name, value);
            }

            if (index >= Length)
                Length = index + 1;

            return base.SetOwnProperty((double)index, value);
        }

        public override object GetOwnProperty(object name)
        {
            var index = AsArrayIndex(name);

            if (index == -1)
                return base.GetOwnProperty(name);

            return base.GetOwnProperty((double)index);
        }

        public override string ToString()
        {
            return (string) (Get("join") as IFunction).Call(this, new[] { (object)"," });
        }

        #endregion

        #region IDynamicMetaObjectProvider Members

        public Meta GetMetaObject(Et parameter)
        {
            return new IObjMeta(parameter, this);
        }

        #endregion
    }
}
