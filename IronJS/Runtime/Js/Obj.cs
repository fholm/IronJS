using System.Collections.Generic;
using System.Dynamic;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Js
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;
    using System;

    enum ObjClass { Object, Function }

    class Obj : IDynamicMetaObjectProvider
    {
        public readonly Frame Frame;
        public readonly Lambda Lambda;

        internal readonly Dictionary<object, Property> Properties =
            new Dictionary<object, Property>();

        // 8.6.2
        internal Obj Prototype;

        // 8.6.2
        public readonly ObjClass Class;

        // 8.6.2
        internal object Value;

        public Obj()
        {
            Class = ObjClass.Object;
        }

        public Obj(Frame frame, Lambda lambda)
        {
            Frame = frame;
            Lambda = lambda;
            Class = ObjClass.Function;
        }

        public object SetOwnProperty(object key, object value)
        {
            Properties[key] = new Property(value);
            return value;
        }

        #region IDynamicMetaObjectProvider Members

        Meta IDynamicMetaObjectProvider.GetMetaObject(Et parameter)
        {
            return new ObjMeta(parameter, this);
        }

        #endregion
    }
}
