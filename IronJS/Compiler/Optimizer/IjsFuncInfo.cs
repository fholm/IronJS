using System;
using System.Collections.Generic;
using System.Reflection;
using IronJS.Compiler.Ast;
using IronJS.Extensions;

namespace IronJS.Compiler.Optimizer
{
    public class IjsFuncInfo
    {
        public bool IsLambda { get; set; }
        public bool IsCompiled { get { return CompiledMethod != null; } }
        public bool UsesArgumentsObject { get; set; }
        public Type ExprType { get { return typeof(IjsFunc); } }
        public Type ReturnType { get { return IjsTypes.Dynamic; } }
        public MethodInfo CompiledMethod { get; set; }
        public FuncNode AstNode { get; set; }
        public HashSet<Type> Returns { get; protected set; }
        public HashSet<IdentifierNode> ClosesOver { get; protected set; }
        public Type ClosureType { get; set; }

        public IjsFuncInfo(FuncNode node)
        {
            AstNode = node;
            IsLambda = false;
            CompiledMethod = null;
            UsesArgumentsObject = false;
            Returns = new HashSet<Type>();
            ClosesOver = new HashSet<IdentifierNode>();
        }
    }
}
