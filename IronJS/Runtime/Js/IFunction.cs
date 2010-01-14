using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime.Js
{
    public interface IFunction : IObj
    {
        // 8.6.2
        IFrame Frame { get; }   // [[Scope]]
        Lambda Lambda { get; }  // [[Call]]

        // 8.6.2
        IObj Construct();   // [[Construct]]
    }
}
