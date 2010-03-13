using System;
using System.Dynamic;
using IronJS.Ast.Nodes;
using IronJS.Runtime.Jit.Tools;
using IronJS.Tools;
using Microsoft.Scripting.Utils;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Runtime.Js {
    using MetaObj = DynamicMetaObject;

	public class Closure {
        public readonly Lambda Ast;
        public readonly ClosureCtx Context;
        public readonly Type ContextType;

		public Closure(Lambda ast, ClosureCtx ctx) {
			Ast = ast;
			Context = ctx;
            ContextType = ctx.GetType();
		}

        public Delegate GetDelegate(MetaObj[] args) {
            return Context.Runtime.Jit.Compile(
                Ast, ArrayUtils.Insert(
                    ContextType, MetaObjTools.GetTypes(args)
                )
            );
        }
    }
}
