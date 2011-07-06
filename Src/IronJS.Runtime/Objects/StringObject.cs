using System;

namespace IronJS.Runtime.Objects
{
    public class StringObject : ValueObject
    {
        public StringObject(Environment env)
            : base(env, env.Maps.String, env.Prototypes.String)
        {
        }

        public override string ClassName
        {
            get { return "String"; }
        }

        public override BoxedValue Get(uint i)
        {
            var a = (int)i;
            var s = this.Value.Value.String;

            if (this.Value.HasValue && a < s.Length)
            {
                return BoxedValue.Box(s[a].ToString());
            }

            return Undefined.Boxed;
        }

        public override BoxedValue Get(string s)
        {
            int i;
            if (Int32.TryParse(s, out i))
            {
                if (i >= 0)
                {
                    return this.Get((uint)i);
                }

                return Undefined.Boxed;
            }

            return base.Get(s);
        }
    }
}
