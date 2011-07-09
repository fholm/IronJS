using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.FSharp.Collections;
#if !CLR2
using System.Dynamic;
#endif

namespace IronJS.Runtime
{
    using DynamicScope = FSharpList<Tuple<int, CommonObject>>;

    public class FunctionObject : CommonObject
    {
        public FunctionMetaData MetaData;
        public DynamicScope DynamicScope;
        public BoxedValue[] SharedScope;
        public readonly BoxedValue[] ReusablePrivateScope;

        public FunctionObject(Environment env, ulong id, BoxedValue[] sharedScope, DynamicScope dynamicScope)
            : base(env, env.Maps.Function, env.Prototypes.Function)
        {
            MetaData = env.GetFunctionMetaData(id);
            SharedScope = sharedScope;
            DynamicScope = dynamicScope;
        }

        public FunctionObject(Environment env, FunctionMetaData metaData, Schema schema) :
            base(env, schema, env.Prototypes.Function)
        {
            MetaData = metaData;
            SharedScope = new BoxedValue[0];
            DynamicScope = DynamicScope.Empty;
        }

        public FunctionObject(Environment env)
            : base(env)
        {
            MetaData = env.GetFunctionMetaData(0UL);
            SharedScope = null;
            DynamicScope = DynamicScope.Empty;
        }

        public override string ClassName
        {
            get
            {
                return "Function";
            }
        }

        public string Name
        {
            get
            {
                return MetaData.Name;
            }
        }

#if !CLR2
        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            var boxedArgs = args.Select(x => BoxedValue.Box(x)).ToArray();
            result = Call(Env.Globals, args).UnboxObject();
            return true;
        }
#endif

        public CommonObject InstancePrototype
        {
            get
            {
                var prototype = Get("prototype");
                switch (prototype.Tag)
                {
                    case TypeTags.Function:
                    case TypeTags.Object:
                        return prototype.Object;

                    default:
                        return Env.Prototypes.Object;
                }
            }
        }

        public CommonObject NewInstance()
        {
            CommonObject o = Env.NewObject();
            o.Prototype = InstancePrototype;
            return o;
        }

        public bool HasInstance(CommonObject v)
        {
            var o = Get("prototype");

            if (!o.IsObject)
                return Env.RaiseTypeError<bool>("prototype property is not an object");

            v = (v != null) ? v.Prototype : null;

            while (v != null)
            {
                if (Object.ReferenceEquals(o.Object, v))
                    return true;

                v = v.Prototype;
            }

            return false;
        }

        public BoxedValue Call(CommonObject @this)
        {
            return
                MetaData
                    .GetDelegate<Func<FunctionObject, CommonObject, BoxedValue>>(this)
                    .Invoke(this, @this);
        }

        public BoxedValue Call<T0>(CommonObject @this, T0 a0)
        {
            return
                MetaData
                    .GetDelegate<Func<FunctionObject, CommonObject, T0, BoxedValue>>(this)
                    .Invoke(this, @this, a0);
        }

        public BoxedValue Call<T0, T1>(CommonObject @this, T0 a0, T1 a1)
        {
            return
                MetaData
                    .GetDelegate<Func<FunctionObject, CommonObject, T0, T1, BoxedValue>>(this)
                    .Invoke(this, @this, a0, a1);
        }

        public BoxedValue Call<T0, T1, T2>(CommonObject @this, T0 a0, T1 a1, T2 a2)
        {
            return
                MetaData
                    .GetDelegate<Func<FunctionObject, CommonObject, T0, T1, T2, BoxedValue>>(this)
                    .Invoke(this, @this, a0, a1, a2);
        }

        public BoxedValue Call<T0, T1, T2, T3>(CommonObject @this, T0 a0, T1 a1, T2 a2, T3 a3)
        {
            return
                MetaData
                    .GetDelegate<Func<FunctionObject, CommonObject, T0, T1, T2, T3, BoxedValue>>(this)
                    .Invoke(this, @this, a0, a1, a2, a3);
        }

        public BoxedValue Call(CommonObject @this, BoxedValue[] args)
        {
            return
                MetaData
                    .GetDelegate<Func<FunctionObject, CommonObject, BoxedValue[], BoxedValue>>(this)
                    .Invoke(this, @this, args);
        }

        public BoxedValue Construct()
        {
            switch (MetaData.FunctionType)
            {
                case FunctionType.NativeConstructor:
                    return Call(null);

                case FunctionType.UserDefined:
                    var o = NewInstance();
                    return PickReturnObject(Call(o), o);

                default:
                    return Env.RaiseTypeError<BoxedValue>();
            }
        }

        public BoxedValue Construct<T0>(T0 a0)
        {
            switch (MetaData.FunctionType)
            {
                case FunctionType.NativeConstructor:
                    return Call(null, a0);

                case FunctionType.UserDefined:
                    var o = NewInstance();
                    return PickReturnObject(Call(o, a0), o);

                default:
                    return Env.RaiseTypeError<BoxedValue>();
            }
        }

        public BoxedValue Construct<T0, T1>(T0 a0, T1 a1)
        {
            switch (MetaData.FunctionType)
            {
                case FunctionType.NativeConstructor:
                    return Call(null, a0, a1);

                case FunctionType.UserDefined:
                    var o = NewInstance();
                    return PickReturnObject(Call(o, a0, a1), o);

                default:
                    return Env.RaiseTypeError<BoxedValue>();
            }
        }

        public BoxedValue Construct<T0, T1, T2>(T0 a0, T1 a1, T2 a2)
        {
            switch (MetaData.FunctionType)
            {
                case FunctionType.NativeConstructor:
                    return Call(null, a0, a1, a2);

                case FunctionType.UserDefined:
                    var o = NewInstance();
                    return PickReturnObject(Call(o, a0, a1, a2), o);

                default:
                    return Env.RaiseTypeError<BoxedValue>();
            }
        }

        public BoxedValue Construct<T0, T1, T2, T3>(T0 a0, T1 a1, T2 a2, T3 a3)
        {
            switch (MetaData.FunctionType)
            {
                case FunctionType.NativeConstructor:
                    return Call(null, a0, a1, a2, a3);

                case FunctionType.UserDefined:
                    var o = NewInstance();
                    return PickReturnObject(Call(o, a0, a1, a2, a3), o);

                default:
                    return Env.RaiseTypeError<BoxedValue>();
            }
        }

        public BoxedValue Construct(BoxedValue[] args)
        {
            switch (MetaData.FunctionType)
            {
                case FunctionType.NativeConstructor:
                    return Call(null, args);

                case FunctionType.UserDefined:
                    var o = NewInstance();
                    return PickReturnObject(Call(o, args), o);

                default:
                    return Env.RaiseTypeError<BoxedValue>();
            }
        }

        public BoxedValue PickReturnObject(BoxedValue r, CommonObject o)
        {
            switch (r.Tag)
            {
                case TypeTags.Function: return BoxedValue.Box(r.Func);
                case TypeTags.Object: return BoxedValue.Box(r.Object);
                default: return BoxedValue.Box(o);
            }
        }
    }
}
