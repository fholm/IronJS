using System;
using System.Collections.Generic;
using System.Text;
using IronJS.Compiler.Ast;

namespace IronJS.Compiler.Tools {
    internal static class AnalyzeTools {
        internal static INode GetVariable(Stack<Function> stack, string name) {
            Stack<Function> traversed = new Stack<Function>();

            IVariable variable;
            foreach(Function function in stack) {
                if (function.Variables.TryGetValue(name, out variable)) {

                } else {
                    traversed.Push(function);
                }
            }

            return new Global2(name);
        }
    }
}
