using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting;
using IronJS.Runtime2.Js;

namespace IronJS {
    public class IjsContext {

        #region Fields

        public readonly IjsClosure GlobalClosure;

        #endregion

        #region Properties

        public IjsObj GlobalScope { get { return GlobalClosure.Globals; } }

        #endregion

        #region Constructors

        public IjsContext() {
            GlobalClosure = new IjsClosure(this, new IjsObj());
        }

        #endregion
    }
}
