using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime
{
    using ParameterStorage = Tuple<ParameterStorageType, int>;

    public class FunctionMetaData
    {
        Dictionary<Type, Delegate> delegateCache =
            new Dictionary<Type, Delegate>();

        public string Name;

        public ulong Id { get; private set; }
        public string Source { get; private set; }
        public FunctionCompiler Compiler { get; private set; }
        public FunctionType FunctionType { get; private set; }
        public ParameterStorage[] ParameterStorage { get; private set; }

        public FunctionMetaData(ulong id, FunctionType functionType, FunctionCompiler compiler, ParameterStorage[] parameterStorage)
        {
            Id = id;
            FunctionType = functionType;
            Compiler = compiler;
            ParameterStorage = parameterStorage;
        }

        public FunctionMetaData(ulong id, FunctionCompiler compiler, ParameterStorage[] storage)
            : this(id, FunctionType.UserDefined, compiler, storage)
        {

        }

        public FunctionMetaData(ulong id, FunctionType type, FunctionCompiler compiler)
            : this(id, type, compiler, new ParameterStorage[0])
        {

        }

        public Delegate GetDelegate(FunctionObject function, Type delegateType)
        {
            Delegate compiled;

            if (!delegateCache.TryGetValue(delegateType, out compiled))
            {
                delegateCache[delegateType] = compiled = Compiler(function, delegateType);
            }

            return compiled;
        }

        public T GetDelegate<T>(FunctionObject function) where T : class
        {
            return (T) (object) GetDelegate(function, typeof(T));
        }
    }
}
