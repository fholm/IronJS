using System;
using IronJS.Runtime;
using Microsoft.FSharp.Collections;

namespace IronJS.Runtime
{
    using DynamicScope = FSharpList<Tuple<int, CommonObject>>;

    public delegate Delegate FunctionCompiler(FunctionObject self, Type type);
    public delegate Object GlobalCode(FunctionObject self, CommonObject @this);
    public delegate Object EvalCode(
        FunctionObject self,
        CommonObject @this,
        BoxedValue[] privateScope,
        BoxedValue[] sharedScope,
        DynamicScope scope
    );
}
