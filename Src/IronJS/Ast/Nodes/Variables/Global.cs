using IronJS.Ast.Tools;
using IronJS.Runtime.Js;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Nodes {
	using Et = Expression;

    public class Global : Base {
        public string Name { get; protected set; }

        public Global(string name) 
            : base(NodeType.Global, null) {
            Name = name;
        }

		public override Et Compile(Lambda func) {
			return Et.Call(
				CompileTools.Globals(func),
				typeof(Obj).GetMethod("Get"),
				CompileTools.Constant(Name)
			);
		}

		public override string ToString() {
			return base.ToString() + " " + Name;
		}
    }
}
