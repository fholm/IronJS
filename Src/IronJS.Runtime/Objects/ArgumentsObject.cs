using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime.Objects
{
    using ArgLink = Tuple<ParameterStorageType, int>;

    public class ArgumentsObject : CommonObject
    {
        public BoxedValue[] PrivateScope;
        public BoxedValue[] SharedScope;
        public ArgLink[] LinkMap;
        public bool LinkIntact = true;

        public ArgumentsObject(
            Environment env, 
            ArgLink[] linkMap, 
            BoxedValue[] privateScope, 
            BoxedValue[] sharedScope)
            : base(env, env.Maps.Base, env.Prototypes.Object)
        {
            PrivateScope = privateScope;
            SharedScope = sharedScope;
            LinkMap = linkMap;
        }

        public static ArgumentsObject CreateForVariadicFunction(
            FunctionObject f, 
            BoxedValue[] privateScope, 
            BoxedValue[] sharedScope, 
            BoxedValue[] variadicArgs)
        {
            var x = 
                new ArgumentsObject(
                    f.Env, 
                    f.MetaData.ParameterStorage,
                    privateScope, 
                    sharedScope
                );

            x.CopyLinkedValues();
            x.Put("constructor", f.Env.Constructors.Object, (uint)0xffffff08);
            x.Put("length", (double)variadicArgs.Length, 2);
            x.Put("callee", f, 2);

            if (!object.ReferenceEquals(variadicArgs, null))
            {
                var i = f.MetaData.ParameterStorage.Length;
                for (; i < variadicArgs.Length; i++)
                {
                    x.Put((uint)i, variadicArgs[i]);
                }
            }

            return x;
        }

        public static ArgumentsObject CreateForFunction(
            FunctionObject f,
            BoxedValue[] privateScope,
            BoxedValue[] sharedScope,
            int namedArgsPassed,
            BoxedValue[] extraArgs
        )
        {
            var length = namedArgsPassed + extraArgs.Length;
            var storage = 
                f.MetaData.ParameterStorage
                    .Take(namedArgsPassed)
                    .ToArray();

            var x = 
                new ArgumentsObject(
                    f.Env, 
                    storage, 
                    privateScope, 
                    sharedScope
                );

            x.CopyLinkedValues();
            x.Put("constructor", f.Env.Constructors.Object);
            x.Put("length", (double)length, DescriptorAttrs.DontEnum);
            x.Put("callee", f, DescriptorAttrs.DontEnum);

            for (var i = 0; i < extraArgs.Length; ++i)
            {
                x.Put((uint)(i + namedArgsPassed), extraArgs[i]);
            }

            return x;
        }

        public void CopyLinkedValues()
        {
            for (var i = 0; i < LinkMap.Length; ++i)
            {
                var link = LinkMap[i];
                switch (link.Item1)
                {
                    case ParameterStorageType.Private:
                        base.Put((uint)i, PrivateScope[link.Item2]);
                        break;

                    case ParameterStorageType.Shared:
                        base.Put((uint)i, SharedScope[link.Item2]);
                        break;
                }
            }
        }

        public override void Put(uint index, BoxedValue value)
        {
            var ii = (int)index;

            if (LinkIntact && ii < LinkMap.Length)
            {
                var link = LinkMap[ii];
                switch (link.Item1)
                {
                    case ParameterStorageType.Private:
                        PrivateScope[link.Item2] = value;
                        break;

                    case ParameterStorageType.Shared:
                        SharedScope[link.Item2] = value;
                        break;
                }
            }

            base.Put(index, value);
        }

        public override void Put(uint index, double value)
        {
            Put(index, BoxedValue.Box(value));
        }

        public override void Put(uint index, object value, uint tag)
        {
            Put(index, BoxedValue.Box(value, tag));
        }

        public override BoxedValue Get(uint index)
        {
            var ii = (int)index;

            if (LinkIntact && ii < LinkMap.Length)
            {
                var link = LinkMap[ii];
                switch (link.Item1)
                {
                    case ParameterStorageType.Private:
                        return PrivateScope[link.Item2];

                    case ParameterStorageType.Shared:
                        return SharedScope[link.Item2];
                }
            }

            return base.Get(index);
        }

        public override bool Has(uint index)
        {
            return 
                (LinkIntact && (int)index < LinkMap.Length) 
                || base.Has(index);
        }

        public override bool Delete(uint index)
        {
            var ii = (int)index;

            if(LinkIntact && ii < LinkMap.Length)
            {
                CopyLinkedValues();
                LinkIntact = false;
                PrivateScope = null;
                SharedScope = null;
            }

            return base.Delete(index);
        }
    }
}
