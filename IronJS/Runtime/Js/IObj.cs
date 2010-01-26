using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using IronJS.Runtime.Js.Descriptors;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Js
{
    public enum ObjClass { Object, Function, Boolean, Number, String, Math, Array, Internal, RegExp }

    //
    public enum ValueHint { None, Number, String }

    public interface IObj : IDynamicMetaObjectProvider
    {
        // 8.6.2
        ObjClass Class { get; set; }  // [[Class]]
        IObj Prototype { get; set; }  // [[Prototype]]

        // implementation specific
        Context Context { get; set; }

        // 8.6.2
        bool Has(object name);
        void Set(object name, IDescriptor<IObj> descriptor);
        bool Get(object name, out IDescriptor<IObj> descriptor);
        bool CanSet(object name); 
        bool TryDelete(object name);

        object DefaultValue(ValueHint hint);
        List<KeyValuePair<object, IDescriptor<IObj>>> GetAllPropertyNames();
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

        public static bool Search(this IObj obj, object name, out IDescriptor<IObj> descriptor)
        {
            while (obj != null)
            {
                if (obj.Get(name, out descriptor))
                    return true;

                obj = obj.Prototype;
            }

            descriptor = null;
            return false;
        }

        public static double GetNumber(this IObj obj, object name)
        {
            return JsTypeConverter.ToNumber(obj.Get(name));
        }

        public static object Get(this IObj obj, object name)
        {
            IDescriptor<IObj> descriptor;

            if (obj.Get(name, out descriptor))
                return descriptor.Get();

            return Undefined.Instance;
        }

        public static object Set(this IObj obj, object name, int value)
        {
            return obj.Set(name, (double)value);
        }

        public static object Set(this IObj obj, int name, int value)
        {
            return obj.Set(name, (double)value);
        }

        public static object Set(this IObj obj, object name, object value)
        {
            IDescriptor<IObj> descriptor;

            if (obj.Get(name, out descriptor))
                descriptor.Set(value);
            else
                obj.Set(name, new UserProperty(obj, value));

            return value;
        }
    }
}
