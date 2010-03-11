using System;
using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Nodes {
	public class ObjInit : Base {
		public string[] PropertyNames { get; protected set; }

		public ObjInit(Dictionary<string, INode> properties, ITree node)
			: base(NodeType.Object, node) {
			Children = new INode[properties.Count];
			properties.Values.CopyTo(Children, 0);

			PropertyNames = new string[properties.Count];
			properties.Keys.CopyTo(PropertyNames, 0);
		}

		public override Type Type {
			get {
				return Types.Object;
			}
		}
	}
}
