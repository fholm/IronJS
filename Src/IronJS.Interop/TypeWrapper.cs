using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Interop
{
    internal class TypeWrapper : CommonObject
    {
        private readonly Type type;

        private TypeWrapper(Type type, Environment env, CommonObject prototype)
            : base(env, prototype)
        {
            this.type = type;
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

        public static TypeWrapper Create(Environment env, Type type)
        {
            var prototype = env.Prototypes.Object;
            return new TypeWrapper(type, env, prototype);
        }
    }
}
