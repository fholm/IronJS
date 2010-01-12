using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Compiler.Ast
{
    interface ILabelableNode
    {
        bool IsLabeled { get; }
        void SetLabel(string label);
        void Enter(FunctionScope functionScope);
        void Exit(FunctionScope functionScope);
    }
}
