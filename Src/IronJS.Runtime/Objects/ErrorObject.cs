using System;

namespace IronJS.Runtime.Objects
{
    public class ErrorObject : CommonObject
    {
        public ErrorObject(Environment env)
            : base(env, env.Maps.Base, env.Prototypes.Error)
        {
        }

        public override string ClassName
        {
            get { return "Error"; }
        }
    }
}
