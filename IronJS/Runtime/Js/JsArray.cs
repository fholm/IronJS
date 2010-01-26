using System;
using System.Collections.Generic;
using System.Reflection;
using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Js
{
    public class JsArray : IObj
    {
        static public readonly ConstructorInfo Ctor
            = typeof(Obj).GetConstructor(Type.EmptyTypes);

        protected readonly Dictionary<object, IDescriptor<IObj>> Properties
            = new Dictionary<object, IDescriptor<IObj>>();

        protected object ToArrayIndex(object name)
        {
            if (name is int)
                return name;

            if (name is double)
            {
                var intval = (int)(double)name;

                if ((double)intval == (double)name)
                    return intval;
            }

            if (name is string)
            {
                var strval = (string)name;

                if (Char.IsNumber(strval[0]))
                {
                    int intval;

                    if (int.TryParse(strval, out intval))
                    {
                        if (intval.ToString() == strval)
                            return intval;
                    }
                }
            }

            return name;
        }

        #region IObj Members

        public ObjClass Class { get; set; }
        public IObj Prototype { get; set; }
        public Context Context { get; set; }

        public bool Has(object name)
        {
            return Properties.ContainsKey(ToArrayIndex(name));
        }

        public void Set(object name, IDescriptor<IObj> descriptor)
        {
            Properties.Add(ToArrayIndex(name), descriptor);
        }

        public bool Get(object name, out IDescriptor<IObj> descriptor)
        {
            return Properties.TryGetValue(ToArrayIndex(name), out descriptor);
        }

        public bool CanSet(object name)
        {
            IDescriptor<IObj> descriptor;
            return Properties.TryGetValue(ToArrayIndex(name), out descriptor) ? descriptor.IsReadOnly : true;
        }

        public bool TryDelete(object name)
        {
            return Properties.Remove(ToArrayIndex(name));
        }

        public object DefaultValue(ValueHint hint)
        {
            throw new NotImplementedException();
        }

        public List<KeyValuePair<object, IDescriptor<IObj>>> GetAllPropertyNames()
        {
            throw new NotImplementedException("ForIn for arrays are working atm");
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
