using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting;
using IronJS.Runtime2.Js;

namespace IronJS {
    public sealed class IjsContext {
        public IjsClosure GlobalClosure { get; private set; }
        public IjsObj GlobalScope { get { return GlobalClosure.Globals; } }

        public IjsContext() {
            GlobalClosure = new IjsClosure(this, new IjsObj());
        }
    }
}
