using System;

namespace IronJS.Runtime.Objects
{
    public class MathObject : CommonObject
    {
        public MathObject(Environment env)
            : base(env, env.Maps.Base, env.Prototypes.Object)
        {
        }

        public override string ClassName
        {
            get { return "Math"; }
        }
    }
}
