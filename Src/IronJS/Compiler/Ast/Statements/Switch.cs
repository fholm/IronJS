using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Tools;

#if CLR2
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast {
	public class Switch : Node, ILabelable {
		public INode Target { get { return Children[0]; } }
		public INode Default { get { return Children[1]; } }
		public string Label { get; protected set; }

		public Switch(INode target, INode _default, List<Tuple<INode, INode>> cases, ITree node)
			: base(NodeType.Switch, node) {
			ContractUtils.RequiresNotNull(cases, "cases");

			Children = new INode[(cases.Count * 2) + 2];
			Children[0] = target;
			Children[1] = _default;

			int offset = 2;
			foreach (Tuple<INode, INode> pair in cases) {
				Children[offset] = pair.Item1;
				Children[offset + 1] = pair.Item2;
				offset += 2;
			}

			Label = null;
		}

		#region ILabelableNode Members

		public void SetLabel(string label) {
			Label = label;
		}

		#endregion
	}
}
