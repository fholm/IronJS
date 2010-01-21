
namespace IronJS.Runtime.Js
{
    public interface IValueObj : IObj
    {
        // 8.6.2
        object Value { get; } // [[Value]]
    }
}
