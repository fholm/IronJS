using System;
using System.Linq;

namespace IronJS.Runtime
{
    using ArgLink = Tuple<ParameterStorageType, int>;

    /// <summary>
    /// A <see cref="CommonObject"/> used as an argument to a <see cref="FunctionObject"/>.
    /// </summary>
    public class ArgumentsObject : CommonObject
    {
        private bool linkIntact = true;


        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentsObject"/> class.
        /// </summary>
        /// <param name="env">The environment.</param>
        /// <param name="linkMap">The link map.</param>
        /// <param name="privateScope">The private scope.</param>
        /// <param name="sharedScope">The shared scope.</param>
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


        /// <summary>
        /// Gets or sets a value indicating whether to keep the link intact.
        /// </summary>
        /// <value>
        ///   <c>true</c> to keep the link intact; otherwise, <c>false</c>.
        /// </value>
        public bool LinkIntact
        {
            get { return this.linkIntact; }
            set { this.linkIntact = value; }
        }

        /// <summary>
        /// Gets or sets the link map.
        /// </summary>
        /// <value>
        /// The link map.
        /// </value>
        public ArgLink[] LinkMap { get; set; }

        /// <summary>
        /// Gets or sets whether the <see cref="ArgumentsObject"/> should be limited
        /// to the private scope or not.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the <see cref="ArgumentsObject"/> should be limited
        /// to the private scope; otherwise <c>false</c>.
        /// </value>
        public BoxedValue[] PrivateScope { get; set; }

        /// <summary>
        /// Gets or sets the shared scope.
        /// </summary>
        /// <value>
        /// The shared scope.
        /// </value>
        public BoxedValue[] SharedScope { get; set; }


        /// <summary>
        /// Creates a <see cref="ArgumentsObject"/> for the specified <see cref="FunctionObject"/>
        /// <paramref name="f"/>.
        /// </summary>
        /// <param name="f">The function for which to create an <see cref="ArgumentsObject"/>.</param>
        /// <param name="privateScope">The private scope.</param>
        /// <param name="sharedScope">The shared scope.</param>
        /// <param name="namedArgsPassed">The number of named arguments that is passed.</param>
        /// <param name="extraArgs">The extra arguments.</param>
        /// <returns>
        /// A <see cref="ArgumentsObject"/> for the specified <see cref="FunctionObject"/>
        /// <paramref name="f"/>.
        /// </returns>
        public static ArgumentsObject CreateForFunction(
            FunctionObject f,
            BoxedValue[] privateScope,
            BoxedValue[] sharedScope,
            int namedArgsPassed,
            BoxedValue[] extraArgs)
        {
            // TODO: This method has no tests. [asbjornu]

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
            x.Put("length", length, DescriptorAttrs.DontEnum);
            x.Put("callee", f, DescriptorAttrs.DontEnum);

            for (var i = 0; i < extraArgs.Length; ++i)
                x.Put((uint)(i + namedArgsPassed), extraArgs[i]);

            return x;
        }


        /// <summary>
        /// Creates a <see cref="ArgumentsObject"/> for the specified variadic
        /// <see cref="FunctionObject"/> <paramref name="f"/>.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <param name="privateScope">The private scope.</param>
        /// <param name="sharedScope">The shared scope.</param>
        /// <param name="variadicArgs">The variadic args.</param>
        /// <returns>
        /// A <see cref="ArgumentsObject"/> for the specified variadic <see cref="FunctionObject"/>
        /// <paramref name="f"/>.
        /// </returns>
        public static ArgumentsObject CreateForVariadicFunction(
            FunctionObject f,
            BoxedValue[] privateScope,
            BoxedValue[] sharedScope,
            BoxedValue[] variadicArgs)
        {
            // TODO: This method has no tests. [asbjornu]

            var x =
                new ArgumentsObject(
                    f.Env,
                    f.MetaData.ParameterStorage,
                    privateScope,
                    sharedScope
                    );

            x.CopyLinkedValues();
            x.Put("constructor", f.Env.Constructors.Object, 0xffffff08);
            x.Put("length", variadicArgs.Length, 2);
            x.Put("callee", f, 2);

            // TODO: R# says this expression will always evaluate to false. Rewrite or remove? [asbjornu]
            if (!ReferenceEquals(variadicArgs, null))
            {
                var i = f.MetaData.ParameterStorage.Length;
                for (; i < variadicArgs.Length; i++)
                    x.Put((uint)i, variadicArgs[i]);
            }

            return x;
        }


        /// <summary>
        /// Deletes the property at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the property to delete.</param>
        /// <returns>
        ///   <c>true</c> if the deletion succeeded; otherwise <c>false</c>.
        /// </returns>
        public override bool Delete(uint index)
        {
            var ii = (int)index;

            if (LinkIntact && ii < LinkMap.Length)
            {
                CopyLinkedValues();
                LinkIntact = false;
                PrivateScope = null;
                SharedScope = null;
            }

            return base.Delete(index);
        }


        /// <summary>
        /// Gets the property at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the property to get.</param>
        /// <returns>
        /// The property at the specified <paramref name="index"/>.
        /// </returns>
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


        /// <summary>
        /// Determines whether the <see cref="ArgumentsObject"/> has a
        /// property at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>
        ///   <c>true</c> if the <see cref="ArgumentsObject"/> has a
        /// property at the specified <paramref name="index"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Has(uint index)
        {
            return
                (LinkIntact && (int)index < LinkMap.Length)
                || base.Has(index);
        }


        /// <summary>
        /// Puts the <paramref name="value"/> at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
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


        /// <summary>
        /// Puts the <paramref name="value"/> at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        public override void Put(uint index, double value)
        {
            Put(index, BoxedValue.Box(value));
        }


        /// <summary>
        /// Puts the <paramref name="value"/> at the specified <paramref name="index"/>
        /// with the provided <paramref name="tag"/>.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        /// <param name="tag">The tag.</param>
        public override void Put(uint index, object value, uint tag)
        {
            Put(index, BoxedValue.Box(value, tag));
        }


        /// <summary>
        /// Copies the linked values.
        /// </summary>
        public void CopyLinkedValues()
        {
            // TODO: Can this method be made private? [asbjornu]
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
    }
}