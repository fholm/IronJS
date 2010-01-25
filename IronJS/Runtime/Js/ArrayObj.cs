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
            get { return (int)(double)GetOwn("length"); }
            set { Properties["length"].Value = (double)value; }
        }

        object UpdateLength(object value)
        {
            var intval = JsTypeConverter.ToInt32(value);

            if ((double)intval != JsTypeConverter.ToNumber(value))
                throw new ShouldThrowTypeError();

            var length = Length - 1;

            for (; length >= intval; --length)
                TryDelete((double)length);

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

        public override object Set(object name, object value)
        {
            var index = AsArrayIndex(name);

            if (index == -1)
            {
                if (name is string && (string)name == "length")
                    return UpdateLength(value);

                return base.Set(name, value);
            }

            if (index >= Length)
                Length = index + 1;

            return base.Set((double)index, value);
        }

        public override bool Has(object name)
        {
            var index = AsArrayIndex(name);

            if (index == -1)
                return base.Has(name);

            return base.Has((double)index);
        }

        public override bool TryDelete(object name)
        {
            var index = AsArrayIndex(name);

            if (index == -1)
                return base.TryDelete(name);

            return base.TryDelete((double)index);
        }

        public override bool HasOwn(object name)
        {
            var index = AsArrayIndex(name);

            if (index == -1)
                return base.HasOwn(name);

            return base.HasOwn((double)index);
        }

        public override object SetOwn(object name, object value)
        {
            var index = AsArrayIndex(name);

            if (index == -1)
            {
                if (name is string && (string)name == "length")
                    return UpdateLength(value);

                return base.SetOwn(name, value);
            }

            if (index >= Length)
                Length = index + 1;

            return base.SetOwn((double)index, value);
        }

        public override object GetOwn(object name)
        {
            var index = AsArrayIndex(name);

            if (index == -1)
                return base.GetOwn(name);

            return base.GetOwn((double)index);
        }

        public override string ToString()
        {
            return (string) (Get("join") as IFunction).Call(this, new[] { (object)"," });
        }

        #endregion
    }
}
