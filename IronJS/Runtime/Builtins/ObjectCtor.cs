using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    public class ObjectCtor : Function
    {
        public IObj Object_prototype { get; protected set; }

        protected ObjectCtor(Context context)
                                            //TODO: Refactor IFunction so this isn't neccessary
            : base(context.BuiltinsGlobals, new Lambda(new LambdaType(x => null)))
        {
            Context = context;

            //TODO: this might not be a good idea sicne the 'prototype' property
            // might become replaced during runtime. Need to check Spec if it's
            // marked as ReadOnly.
            Object_prototype = CreatePrototype(context);
            Put("prototype", Object_prototype);
        }

        public override IObj Construct()
        {
            var obj = Context.CreateObject();

            obj.Prototype = Object_prototype;

            return obj;
        }

        static public ObjectCtor Create(Context context)
        {
            return new ObjectCtor(context);
        }

        static protected IObj CreatePrototype(Context context)
        {
            var obj = new Obj();

            obj.Class = ObjClass.Object;
            obj.Prototype = null;
            obj.Context = context;

            return obj;
        }
    }
}
