using IronJS.Runtime.Js;

namespace IronJS {
    public sealed class Context {
        public Closure GlobalClosure { get; private set; }
        public Obj GlobalScope { get { return GlobalClosure.Globals; } }

        public Context() {
            GlobalClosure = new Closure(this, new Obj());
        }
    }
}
