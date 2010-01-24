using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    class Function_prototype : NativeFunction
    {
        public Function_prototype(Context context)
            : base(context, null)
        {
            SetOwnProperty("toString", new Function_prototype_toString(context, this));
            SetOwnProperty("apply", new Function_prototype_apply(context, this));
            SetOwnProperty("call", new Function_prototype_call(context, this));
        }

        public override object Call(Js.IObj that, object[] args)
        {
            return Js.Undefined.Instance;
        }
    }
}
