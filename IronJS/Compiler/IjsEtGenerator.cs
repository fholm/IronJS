/* ****************************************************************************
 *
 * Copyright (c) Fredrik Holmström
 *
 * This source code is subject to terms and conditions of the Microsoft Public License. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Microsoft Public License, please send an email to 
 * dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Microsoft Public License.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System.Collections.Generic;
using System.Reflection;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler
{
    public class IjsEtGenerator
    {
        public MethodInfo Generate(List<Ast.INode> astNodes, IjsContext context)
        {
            return null;
        }

        internal Et Constant<T>(T value)
        {
            return Et.Constant(value);
        }
    }
}
