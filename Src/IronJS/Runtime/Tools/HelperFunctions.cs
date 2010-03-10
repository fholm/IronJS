using System;
using IronJS.Runtime.Js;
using Microsoft.Scripting.Utils;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Runtime.Utils {
	public static class HelperFunctions {
		static public object PrintLine(object obj) {
			Console.WriteLine(obj);
			return obj;
		}

		static public void Timer(IjsFunc proxy) {
		}
	}
}
