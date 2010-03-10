/*
 * GENERATED CODE
 **/
using System;
using IronJS.Tools;
using IronJS.Runtime.Js.Proxies;

#if CLR2
using Microsoft.Scripting.Ast;  
#else
using System.Linq.Expressions;
#endif
 
namespace IronJS.Ast.Tools {
    internal static partial class CompileTools{
		static Type BuildCallProxyType(Expression[] args){
			Type type = null;

			if(args.Length == 1) 
				type = typeof(IjsCall<>);
			else if(args.Length == 2) 
				type = typeof(IjsCall<,>);
			else if(args.Length == 3) 
				type = typeof(IjsCall<,,>);
			else if(args.Length == 4) 
				type = typeof(IjsCall<,,,>);
			else if(args.Length == 5) 
				type = typeof(IjsCall<,,,,>);
			else if(args.Length == 6) 
				type = typeof(IjsCall<,,,,,>);
			else if(args.Length == 7) 
				type = typeof(IjsCall<,,,,,,>);
			else if(args.Length == 8) 
				type = typeof(IjsCall<,,,,,,,>);
			else if(args.Length == 9) 
				type = typeof(IjsCall<,,,,,,,,>);
			else if(args.Length == 10) 
				type = typeof(IjsCall<,,,,,,,,,>);
			else if(args.Length == 11) 
				type = typeof(IjsCall<,,,,,,,,,,>);
			else if(args.Length == 12) 
				type = typeof(IjsCall<,,,,,,,,,,,>);
			else if(args.Length == 13) 
				type = typeof(IjsCall<,,,,,,,,,,,,>);
			else if(args.Length == 14) 
				type = typeof(IjsCall<,,,,,,,,,,,,,>);
			else if(args.Length == 15) 
				type = typeof(IjsCall<,,,,,,,,,,,,,,>);
			else
				throw new NotImplementedException("Currently you can't call function with more then 15 arguments");

			return type.MakeGenericType(
				IEnumerableTools.Map(args, delegate(Expression expr) { return expr.Type; })
			);
		}
    }
}