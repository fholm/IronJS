using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime.Js
{
    class Table
    {
        readonly List<Lambda> Functions =
             new List<Lambda>();

        public int Push(Lambda func)
        {
            Functions.Add(func);
            return Functions.Count - 1;
        }

        public Lambda Pull(int i)
        {
            return Functions[i];
        }
    }
}
