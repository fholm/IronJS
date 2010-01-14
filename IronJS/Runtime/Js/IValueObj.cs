using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime.Js
{
    interface IValueObj : IObj
    {
        // 8.6.2
        object Value { get; } // [[Value]]
    }
}
