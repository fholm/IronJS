namespace IronJS.Compiler
{
    public interface IjsCloseableVar : IjsIVar
    {
        bool IsClosedOver { get; set; }
    }
}
