using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Compiler.Ast
{
    interface ILabelableNode
    {
        void SetLabel(string label);
        void Init(FunctionScope functionScope);
    }
}
