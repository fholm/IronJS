using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime.Js
{
    class Table
    {
        readonly List<Function> Functions =
             new List<Function>();

        public int Push(Function func)
        {
            Functions.Add(func);
            return Functions.Count - 1;
        }

        public Function Pull(int i)
        {
            return Functions[i];
        }
    }
}
