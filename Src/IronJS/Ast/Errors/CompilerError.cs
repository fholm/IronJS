using System;

namespace IronJS.Ast {
    public class CompilerError : Error {
        internal CompilerError(string msg, params object[] parms)
            : base(String.Format(msg, parms)) {

        }
    }

	public class AstError : CompilerError {
        internal AstError(string msg, params object[] parms)
            : base(msg, parms) {

        }
    }
}
