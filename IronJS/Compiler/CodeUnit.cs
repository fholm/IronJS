using System;
using IronJS.Runtime;
using IronJS.Runtime.Js;

namespace IronJS.Compiler
{
    //TODO: this class needs a new name
    sealed public class CodeUnit
    {
        readonly Action<IObj> _delegate;
        readonly Context _context;

        internal CodeUnit(Action<IObj> delegat, Context context)
        {
            _delegate = delegat;
            _context = context;
        }

        public IObj Run(Action<IObj> setup)
        {
            return _context.Run(_delegate, setup);
        }
    }
}
