using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IronJS.Runtime.Js
{
    class RegExpObj : Obj
    {
        public Regex Match { get; set;}

        public RegExpObj()
        {
            Class = ObjClass.RegExp;
        }
    }
}
