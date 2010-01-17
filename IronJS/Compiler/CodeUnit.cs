using System;
using IronJS.Runtime;
using IronJS.Runtime.Js;

namespace IronJS.Compiler
{
    //TODO: this class needs a new name
    sealed public class CodeUnit
    {
        readonly Action<IFrame> _delegate;
        readonly Context _context;

        internal CodeUnit(Action<IFrame> delegat, Context context)
        {
            _delegate = delegat;
            _context = context;
        }

        public IFrame Run(Action<IFrame> setup)
        {
            return _context.Run(_delegate, setup);
        }
    }
}
