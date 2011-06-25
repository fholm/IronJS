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

        public static TypeWrapper Create(Environment env, Type type)
        {
            var prototype = (CommonObject)null;

            return new TypeWrapper(type, env, prototype);
        }
    }
}
