using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    class RegExp_prototype_test: NativeFunction
    {
        public RegExp_prototype_test(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            if (!(that is RegExpObj))
                throw new ShouldThrowTypeError();

            return (that.Get("exec") as IFunction).Call(that, args) != null;
        }
    }
}
