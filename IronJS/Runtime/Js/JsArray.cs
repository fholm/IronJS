using System;
using System.Collections.Generic;
using System.Reflection;
using IronJS.Runtime.Js.Descriptors;
using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Js
{
    public class JsArray : IObj
    {
        static public readonly ConstructorInfo Ctor
            = typeof(Obj).GetConstructor(Type.EmptyTypes);

        internal readonly Dictionary<object, IDescriptor<IObj>> Properties
            = new Dictionary<object, IDescriptor<IObj>>();

        public LengthProperty Length { get; protected set; }

        public JsArray()
        {
            Length = new LengthProperty(this);
            Properties.Add("length", Length);
        }

        public override string ToString()
        {
            return (string) (this.Search("join") as IFunction).Call(this, new object[] { "," });
        }

        protected object IsArrayIndex(object name)
        {
            return name is int;
        }

        #region IObj Members

        public ObjClass Class { get; set; }
        public IObj Prototype { get; set; }
        public Context Context { get; set; }

        public bool Has(object name)
        {
            return Properties.ContainsKey(name);
        }

        public void Set(object name, IDescriptor<IObj> descriptor)
        {
            if (name is int)
                Length.Update((int)name);

            Properties.Add(name, descriptor);
        }

        public bool Get(object name, out IDescriptor<IObj> descriptor)
        {
            return Properties.TryGetValue(name, out descriptor);
        }

        public bool CanSet(object name)
        {
            IDescriptor<IObj> descriptor;
            return Properties.TryGetValue(name, out descriptor) ? descriptor.IsReadOnly : true;
        }

        public bool TryDelete(object name)
        {
            return Properties.Remove(name);
        }

        public object DefaultValue(ValueHint hint)
        {
            if (hint == ValueHint.String)
                return this.DefaultValueString();

            return this.DefaultValueNumber();
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
