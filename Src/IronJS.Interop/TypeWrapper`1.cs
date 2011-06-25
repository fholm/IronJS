using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace IronJS.Interop
{
    internal class TypeWrapper<T> : CommonObject
    {
        private readonly Type type;

        private TypeWrapper(Environment env, CommonObject prototype)
            : base(env, prototype)
        {
            this.type = typeof(T);
        }

        public override BoxedValue Get(string name)
        {
            return base.Get(name);
        }

        public override bool Has(string name)
        {
            return base.Has(name);
        }

        public override bool HasOwn(string name)
        {
            return base.HasOwn(name);
        }

        public override string ClassName
        {
            get
            {
                return "typeof(" + this.type.FullName + ")";
            }
        }

        public static TypeWrapper<T> Create(Environment env)
        {
            var prototype = env.Prototypes.Object;
            return new TypeWrapper<T>(env, prototype);
        }
    }

    internal static class TypeWrapper
    {
        public static CommonObject Create(Type type, Environment env)
        {
            var typeWrapper = typeof(TypeWrapper<>).MakeGenericType(new[] { type });
            var createMethod = typeWrapper.GetMethod("Create", BindingFlags.Static);
            var result = createMethod.Invoke(null, new[] { env });
            return (CommonObject)result;
        }
    }
}
