using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime
{
    public class HostFunctionObject<T> : FunctionObject
        where T : class
    {
        public T Delegate;

        public HostFunctionObject(Environment env, T delegateFunction, FunctionMetaData metaData)
            : base(env, metaData, env.Maps.Function)
        {
            Delegate = delegateFunction;
        }
    }
}
