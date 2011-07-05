using System;

namespace IronJS.Runtime
{
    public class Undefined
    {
        private static readonly Undefined instance;
        private static readonly BoxedValue boxed;

        static Undefined()
        {
            instance = new Undefined();
            boxed = BoxedValue.Box(instance, TypeTags.Undefined);
        }

        private Undefined()
        {
        }

        public static Undefined Instance
        {
            get { return instance; }
        }

        public static BoxedValue Boxed
        {
            get { return boxed; }
        }

        public override string ToString()
        {
            return "undefined";
        }
    }
}
