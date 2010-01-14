using System;
namespace IronJS.Runtime.Js
{
    //TODO: Might need to add Arg() here for conveniance
    public interface IFrame
    {
        IFrame Enter();
        IFrame Exit();
        object Pull(object key, GetType type);
        object Push(object key, object value, VarType type);
    }
}
