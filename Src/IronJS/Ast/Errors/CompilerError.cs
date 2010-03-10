using System;

namespace IronJS.Ast {
    public class CompilerError : Error {
        internal CompilerError(string msg, params object[] parms)
            : base(String.Format(msg, parms)) {

        }
    }

    public class AstCompilerError : CompilerError {
        internal AstCompilerError(string msg, params object[] parms)
            : base(msg, parms) {

        }
    }

    public class EtCompilerError : CompilerError {
        internal EtCompilerError(string msg, params object[] parms)
            : base(msg, parms) {

        }
    }
}
