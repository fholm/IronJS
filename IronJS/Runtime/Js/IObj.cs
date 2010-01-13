using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime.Js
{
    //TODO: need support for 'Host' here in object class
    public enum ObjClass { Object, Function, Boolean, Number, String }

    public interface IObj
    {
        // 8.6.2
        ObjClass Class { get; } // [[Class]]
        IObj Prototype { get; } // [[Prototype]]
        object Value { get; }   // [[Value]]

        // implementation specific
        Context Context { get; }

        // 8.6.2
        object Get(object name);                // [[Get]]
        object Put(object name, object value);  // [[Put]]
        bool CanPut(object name);               // [[CanPut]]
        bool HasProperty(object name);          // [[HasProperty]]
        bool Delete(object name);               // [[Delete]]
        object DefaultValue(ValueHint hint);    // [[DefaultValue]] 
        IObj Construct();                       // [[Construct]]

        // implementation specific
        bool HasOwnProperty(object name);
        object SetOwnProperty(object name);
        object GetOwnProperty(object name);
    }
}
