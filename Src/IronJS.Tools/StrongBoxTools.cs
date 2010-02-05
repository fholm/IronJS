using System;
using System.Collections.Generic;
using System.Text;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif


namespace IronJS.Tools
{
	using Et = Expression;

	public static class StrongBoxTools
	{
		public static Et MakeAssign(Et strongBox, Et value)
		{
			return Et.Assign(Et.Field(strongBox, "Value"), value);
		}
	}
}
