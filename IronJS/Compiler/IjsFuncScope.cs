/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
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
using EtParam = System.Linq.Expressions.ParameterExpression;

namespace IronJS.Compiler
{
    public class IjsFuncScope
    {
        public IjsFuncScope Parent { get; protected set; }
        public Dictionary<string, EtParam> Variables =
            new Dictionary<string, EtParam>();

        public IjsFuncScope(IjsFuncScope parent)
        {
            Parent = parent;
        }

        public IjsFuncScope Enter()
        {
            return new IjsFuncScope(this);
        }

        public IjsFuncScope Exit()
        {
            return Parent;
        }

        public EtParam this[string name]
        {
            get
            {
                return Variables[name];
            }
            set
            {
                Variables[name] = value;
            }
        }
    }
}
