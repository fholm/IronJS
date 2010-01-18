using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime.Js
{
    public interface IScope
    {
        IScope Enter(IObj obj);
        IScope Enter();
        IScope Exit();

        object Local(object name, object value);
        object Global(object name, object value);

        object Pull(object name);
    }
}
