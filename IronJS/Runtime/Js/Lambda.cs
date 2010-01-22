using System.Reflection;

namespace IronJS.Runtime.Js
{
    public class Lambda
    {
        static public readonly ConstructorInfo Ctor1Arg =
            typeof(Lambda).GetConstructor(new[] { typeof(LambdaType), typeof(string[]) });

        public LambdaType Delegate { get; protected set; }
        public string[] Params { get; protected set; }

        public Lambda(LambdaType func, string[] parms)
        {
            Delegate = func;
            Params = parms;
        }
    }
}
