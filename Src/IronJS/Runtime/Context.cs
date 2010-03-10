/* ***************************************************************************************
 *
 * Copyright (c) Fredrik Holmström
 *
 * This source code is subject to terms and conditions of the Microsoft Public License. 
 * A copy of the license can be found in the License.html file at the root of this 
 * distribution. If you cannot locate the  Microsoft Public License, please send an 
 * email to fredrik.johan.holmstrom@gmail.com. By using this source code in any fashion, 
 * you are agreeing to be bound by the terms of the Microsoft Public License.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************************/
using IronJS.Runtime.Js;
using IronJS.Runtime.Jit;

namespace IronJS {
    public sealed class Context {
        public Closure GlobalClosure { get; private set; }
        public Obj GlobalScope { get { return GlobalClosure.Globals; } }
		public Compiler Compiler { get; private set; }

        public Context() {
            GlobalClosure = new Closure(this, new Obj());
			Compiler = new Compiler();
        }
    }
}
