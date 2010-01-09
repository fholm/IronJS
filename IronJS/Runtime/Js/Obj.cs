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

        internal readonly List<Property> PropertyList =
            new List<Property>();

        // 8.6.2
        /*
         * This is the internal
         * prototype of this object,
         * this is not the same as the
         * public property 'prototype'.
         */
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

        public object SetOwn(object key, object value)
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

        #region static

        static internal Et SetMember(Et target, object name, Et value)
        {
            return Et.Call(
                target,
                typeof(Obj).GetMethod("SetOwn"),
                EtUtils.Box(Et.Constant(name)),
                value
            );
        }

        static internal Et CreateNew()
        {
            return AstUtils.SimpleNewHelper(
                typeof(Obj).GetConstructor(Type.EmptyTypes)
            );
        }

        #endregion 

    }
}
