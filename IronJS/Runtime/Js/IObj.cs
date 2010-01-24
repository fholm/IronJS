using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Runtime.Js
{
    //TODO: need support for 'Host' object class
    public enum ObjClass { Object, Function, Boolean, Number, String, Math, Array, Internal, RegExp }

    //
    public enum ValueHint { None, Number, String }

    public interface IObj : IDynamicMetaObjectProvider
    {
        // 8.6.2
        ObjClass Class { get; set; }  // [[Class]]
        IObj Prototype { get; set;  } // [[Prototype]]

        // implementation specific
        Context Context { get; set; }

        // 8.6.2
        object Get(object name);                // [[Get]]
        object Put(object name, object value);  // [[Put]]
        bool CanPut(object name);               // [[CanPut]]
        bool HasProperty(object name);          // [[HasProperty]]
        bool Delete(object name);               // [[Delete]]
        object DefaultValue(ValueHint hint);    // [[DefaultValue]]

        // implementation specific
        bool TryGet(object name, out object value);
        bool HasOwnProperty(object name);
        object SetOwnProperty(object name, object value);
        object GetOwnProperty(object name);
        List<object> GetAllPropertyNames();
    }

    public static class IObjMethods
    {
        static public readonly PropertyInfo PiClass = typeof(IObj).GetProperty("Class");
        static public readonly PropertyInfo PiPrototype = typeof(IObj).GetProperty("Prototype");
        static public readonly PropertyInfo PiContext = typeof(IObj).GetProperty("Context");

        static public readonly MethodInfo MiGet = typeof(IObj).GetMethod("Get");
        static public readonly MethodInfo MiPut = typeof(IObj).GetMethod("Put");
        static public readonly MethodInfo MiCanPut = typeof(IObj).GetMethod("CanPut");
        static public readonly MethodInfo MiHasProperty = typeof(IObj).GetMethod("HasProperty");
        static public readonly MethodInfo MiDelete = typeof(IObj).GetMethod("Delete");
        static public readonly MethodInfo MiDefaultValue = typeof(IObj).GetMethod("DefaultValue");
        static public readonly MethodInfo MiHasOwnProperty = typeof(IObj).GetMethod("HasOwnProperty");
        static public readonly MethodInfo MiSetOwnProperty = typeof(IObj).GetMethod("HasSetProperty");
        static public readonly MethodInfo MiGetOwnProperty = typeof(IObj).GetMethod("HasGetProperty");
        static public readonly MethodInfo MiGetAllPropertyNames = typeof(IObj).GetMethod("GetAllPropertyNames");
    }

    public static class IObjUtils
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
                IObjMethods.MiSetOwnProperty,
                Et.Constant(name, typeof(object)),
                value
            );
        }

        static internal Et EtHasProperty(Et target, Et name)
        {
            return Et.Call(
                target,
                IObjMethods.MiHasProperty,
                name
            );
        }
    }
}
