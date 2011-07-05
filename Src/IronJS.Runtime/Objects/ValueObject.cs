using System;

namespace IronJS.Runtime.Objects
{
    public abstract class ValueObject : CommonObject
    {
        private Descriptor value;

        public ValueObject(Environment env, Schema map, CommonObject prototype)
            : base(env, map, prototype)
        {
        }

        public static BoxedValue GetValue(CommonObject o)
        {
            var vo = o as ValueObject;

            if (vo == null)
                o.Env.RaiseTypeError("Cannot read the value of a non-value object.");

            return vo.value.Value;
        }
    }
}
