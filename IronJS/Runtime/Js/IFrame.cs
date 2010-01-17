using System;

namespace IronJS.Runtime.Js
{
    public interface IObj
    {
        IObj Enter();
        IObj Exit();
        object Pull(object key, GetType type);
        object Push(object key, object value, VarType type);
    }
}
