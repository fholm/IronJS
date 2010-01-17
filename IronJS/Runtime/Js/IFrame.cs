using System;

namespace IronJS.Runtime.Js
{
    public interface IFrame
    {
        IFrame Enter();
        IFrame Exit();
        object Pull(object key, GetType type);
        object Push(object key, object value, VarType type);
    }
}
