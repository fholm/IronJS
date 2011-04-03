using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DebugConsole
{
    class CallbackWriter : System.IO.TextWriter
    {
        Action<string> callback;

        public CallbackWriter(Action<string> callback)
        {
            this.callback = callback;
        }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }

        public override void Write(string value)
        {
            callback(value);
        }

        public override void WriteLine(string value)
        {
            Write(value + "\n");
        }
    }
}
