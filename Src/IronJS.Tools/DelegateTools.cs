/*
 * GENERATED CODE
 **/
using System;
 
#if CLR2
using Microsoft.Scripting.Ast;  
using Microsoft.Scripting.Utils;
#else
using System.Linq.Expressions;
#endif
 
namespace IronJS.Tools {
    public static partial class DelegateTools {
		public static Type BuildFuncType(Type[] args) {
			Type type = null;
 
			if(args.Length == 0) 
				type = typeof(Func<>); 
			else if(args.Length == 1) 
				type = typeof(Func<,>); 
			else if(args.Length == 2) 
				type = typeof(Func<,,>); 
			else if(args.Length == 3) 
				type = typeof(Func<,,,>); 
			else if(args.Length == 4) 
				type = typeof(Func<,,,,>); 
			else if(args.Length == 5) 
				type = typeof(Func<,,,,,>); 
			else if(args.Length == 6) 
				type = typeof(Func<,,,,,,>); 
			else if(args.Length == 7) 
				type = typeof(Func<,,,,,,,>); 
			else if(args.Length == 8) 
				type = typeof(Func<,,,,,,,,>); 
			else if(args.Length == 9) 
				type = typeof(Func<,,,,,,,,,>); 
			else if(args.Length == 10) 
				type = typeof(Func<,,,,,,,,,,>); 
			else if(args.Length == 11) 
				type = typeof(Func<,,,,,,,,,,,>); 
			else if(args.Length == 12) 
				type = typeof(Func<,,,,,,,,,,,,>); 
			else if(args.Length == 13) 
				type = typeof(Func<,,,,,,,,,,,,,>); 
			else if(args.Length == 14) 
				type = typeof(Func<,,,,,,,,,,,,,,>); 
			else if(args.Length == 15) 
				type = typeof(Func<,,,,,,,,,,,,,,,>); 
			else
				throw new NotImplementedException("Currently you can't call functions with more then 15 arguments");
 
			return type.MakeGenericType(ArrayUtils.Append(args, typeof(object)));
		}
    }
}