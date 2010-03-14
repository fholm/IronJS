using IronJS.Ast.Nodes;
using IronJS.Runtime.Js;
using IronJS.Tools;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Tools {
	using AstUtils = Utils;
	using Et = Expression;

	internal static partial class CompileTools {
		internal static Et Assign(Lambda func, INode target, Et value) {
			Global global = target as Global;
			if (global != null) {
				return Et.Call(
					Globals(func), 
					typeof(Obj).GetMethod("Set"), 
					CompileTools.Constant(global.Name), 
					value
				);
			}

            Local local = target as Local;
            if (local != null) {
                if (local.NodeType == NodeType.Local) {
                    return AssignLocal(local.Compile(func), value);
                }
            }
            
		    return AstUtils.Empty();
		}

        internal static Et AssignLocal(Et local, Et value) {
            if (AstTools.IsStrongBox(local)) {
                return Et.Assign(
                    Et.Field(local, "Value"), value
                );
            }

            return Et.Assign(local, value);
        }

		internal static Et Constant(object obj) {
			return Et.Constant(obj);
		}

		internal static Et Globals(Lambda func) {
			return Et.Field(func.Children[1].Compile(func), "Globals");
		}

		internal static Et Runtime(Lambda func) {
			return Et.Field(func.Children[1].Compile(func), "Runtime");
		}
	}
}
