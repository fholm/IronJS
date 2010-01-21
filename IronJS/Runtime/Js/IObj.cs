using System.Collections.Generic;
using System.Dynamic;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Runtime.Js
{
    //TODO: need support for 'Host' object class
    public enum ObjClass { Object, Function, Boolean, Number, String, Math, Array, Frame, Scope }

    //
    public enum ValueHint { None, Number, String }

    public interface IObj : IDynamicMetaObjectProvider
    {
        // 8.6.2
        ObjClass Class { get; } // [[Class]]
        IObj Prototype { get; } // [[Prototype]]

        // implementation specific
        Context Context { get; }

        // 8.6.2
        object Get(object name);                // [[Get]]
        object Put(object name, object value);  // [[Put]]
        bool CanPut(object name);               // [[CanPut]]
        bool HasProperty(object name);          // [[HasProperty]]
        bool Delete(object name);               // [[Delete]]
        object DefaultValue(ValueHint hint);    // [[DefaultValue]]

        // implementation specific
        bool HasOwnProperty(object name);
        object SetOwnProperty(object name, object value);
        object GetOwnProperty(object name);
        List<object> GetAllPropertyNames();
    }

    public static class IObjMethods
    {
        public static bool HasValue(this IObj obj)
        {
            return (obj is IValueObj);
        }

        public static bool IsFunction(this IObj obj)
        {
            return (obj is IFunction);
        }

        static internal Et EtSetOwnProperty(Et target, object name, Et value)
        {
            return Et.Call(
                target,
                typeof(IObj).GetMethod("SetOwnProperty"),
                Et.Constant(name, typeof(object)),
                value
            );
        }

        static internal Et EtHasProperty(Et target, Et name)
        {
            return Et.Call(
                target,
                typeof(IObj).GetMethod("HasProperty"),
                name
            );
        }
    }
}
