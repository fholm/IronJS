using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime.Js
{
    interface IObj
    {
        object Get(object name);
        object Put(object name, object value);
        object CanPut(object name);
        object HasProperty(object name);
        object Delete(object name);
        object DefaultValue(ValueHint hint);
    }
}
