using System;
using IronJS.Runtime;
using IronJS.Runtime.Js;

namespace IronJS.Compiler
{
    //TODO: this class needs a new name
    sealed public class CodeUnit
    {
        readonly Action<IFrame> Delegate;
        readonly Context Context;

        internal CodeUnit(Action<IFrame> delegat, Context context)
        {
            Delegate = delegat;
            Context = context;
        }

        public void Run()
        {
            Context.Run(Delegate);
        }
    }
}
