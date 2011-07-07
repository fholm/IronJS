using System;
using System.Text.RegularExpressions;

namespace IronJS.Runtime
{
    public class RegExpObject : CommonObject
    {
        private Regex regExp;
        private bool global;

        public RegExpObject(Environment env, string pattern, RegexOptions options, bool global)
            : base(env, env.Maps.RegExp, env.Prototypes.RegExp)
        {
            this.global = global;
            try
            {
                options = (options | RegexOptions.ECMAScript) & ~RegexOptions.Compiled;
                var key = Tuple.Create(options, pattern);
                this.regExp = env.RegExpCache.Lookup(key, () => new Regex(pattern, options | RegexOptions.Compiled));
            }
            catch (ArgumentException ex)
            {
                env.RaiseSyntaxError<object>(ex.Message);
                return;
            }
        }

        public RegExpObject(Environment env, string pattern)
            : this(env, pattern, RegexOptions.None, false)
        {
        }

        public override string ClassName
        {
            get { return "RegExp"; }
        }

        public bool IgnoreCase
        {
            get
            {
                return (this.regExp.Options & RegexOptions.IgnoreCase) == RegexOptions.IgnoreCase;
            }
        }

        public bool MultiLine
        {
            get
            {
                return (this.regExp.Options & RegexOptions.Multiline) == RegexOptions.Multiline;
            }
        }
    }
}
