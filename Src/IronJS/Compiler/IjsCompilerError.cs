using System;

namespace IronJS.Compiler {
    public class IjsCompilerError : Error {
        internal IjsCompilerError(string msg, params object[] parms)
            : base(String.Format(msg, parms)) {

        }
    }

    public class AstCompilerError : IjsCompilerError {
        internal AstCompilerError(string msg, params object[] parms)
            : base(msg, parms) {

        }
    }

    public class EtCompilerError : IjsCompilerError {
        internal EtCompilerError(string msg, params object[] parms)
            : base(msg, parms) {

        }
    }
}
