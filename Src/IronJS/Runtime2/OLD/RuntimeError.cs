using System;
using System.Reflection;
using IronJS.Runtime2.Js;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Runtime
{
    public class RuntimeError : Error
    {
        public RuntimeError(string msg)
            : base(msg)
        {

        }
    }
}
