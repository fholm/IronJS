namespace IronJS.Runtime.Js
{
    public interface IDescriptor<T> where T : IObj
    {
        T Owner { get; }

        bool IsEnumerable { get; }
        bool IsReadOnly { get; }
        bool IsDeletable { get; }

        object Get();
        object Set(object value);
    }
}
