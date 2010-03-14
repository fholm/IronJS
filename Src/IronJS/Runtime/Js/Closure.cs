using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;
using IronJS.Ast.Nodes;
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

        public readonly StrongBox<Obj>[] ObjVars;
        public readonly StrongBox<bool>[] BoolVars;
        public readonly StrongBox<long>[] LongVars;
        public readonly StrongBox<double>[] DoubleVars;
        public readonly StrongBox<string>[] StringVars;
        public readonly StrongBox<object>[] DynamicVars;

		public Closure(Lambda ast, ClosureCtx ctx, Dictionary<string, Type> varTypes) {
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
