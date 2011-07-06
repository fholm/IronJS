using System;

namespace IronJS.Runtime
{
    public class BooleanObject : ValueObject
    {
        public BooleanObject(Environment env)
            : base(env, env.Maps.Boolean, env.Prototypes.Boolean)
        {
        }

        public override string ClassName
        {
            get { return "Boolean"; }
        }
    }
}
