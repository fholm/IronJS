using System;

namespace IronJS.Runtime
{
    public class Context
    {
        internal int ObjectCounter;

        internal Js.Obj Object { get; private set; }
        internal Js.Obj ObjectPrototype { get; private set; }

        internal Js.Obj Function { get; private set; }
        internal Js.Obj FunctionPrototype { get; private set; }

        public Js.Obj Globals { get; private set; }

        public Context()
        {
            Object = new Js.Obj("__Object__", this);
            ObjectPrototype = new Js.Obj("__Object.prototype__", this);

            Function = new Js.Obj("__Function__", this);
            FunctionPrototype = new Js.Obj("__Function.prototype__", this);

            Globals = new Js.Obj("__Globals__", this);
            Globals.Class = "Object";
            Globals.Prototype = ObjectPrototype;

            InitObjectPrototype();
            InitFunctionPrototype();
            InitFunction();
            InitObject();

#if DEBUG
            InitDebug();
#endif
        }

        public Js.Obj CreateFunctionObject()
        {
            return CreateFunctionObject("__function#" + (ObjectCounter++) + "__");
        }

        public Js.Obj CreateFunctionObject(string objName)
        {
            var newObj = new Js.Obj(objName, this);

            newObj.Class = "Function";
            newObj.Prototype = FunctionPrototype;

            return newObj;
        }

        public Js.Obj CreateObject()
        {
            return CreateObject("__object#" + (ObjectCounter++) + "__");
        }

        public Js.Obj CreateObject(string objName)
        {
            var newObj = new Js.Obj(objName, this);

            newObj.Class = "Object";
            newObj.Prototype = ObjectPrototype;

            return newObj;
        }

        void InitDebug()
        {
            var attrs =
                Js.PropertyAttrs.ReadOnly |
                Js.PropertyAttrs.DontDelete |
                Js.PropertyAttrs.DontEnum;

            //AssignFunction(Globals, "print", "__Builtins.print__", new Func<Js.Obj, Js.Obj, object, object>(BuiltIns.Print), attrs);
            AssignFunction(Globals, "inspect", "__Builtins.inspect__", new Func<Js.Obj, Js.Obj, object, object>(BuiltIns.Inspect), attrs);
        }

        void InitFunctionPrototype()
        {
            // Attributes for the built
            // in functions on Function.prototype
            var attrs =
                Js.PropertyAttrs.ReadOnly |
                Js.PropertyAttrs.DontDelete |
                Js.PropertyAttrs.DontEnum;

            // FunctionPrototype internal properties
            FunctionPrototype.Class = "Function";
            FunctionPrototype.Prototype = ObjectPrototype;

            // FunctionPrototype userland properties
            FunctionPrototype.Properties["constructor"] = new Js.Property("constructor", Function);

            // 15.3.4.2
            Func<Js.Obj, Js.Obj, string> toStringFunc = (arguments, that) => { return "[object Function]"; };
            var toString = AssignFunction(FunctionPrototype, "toString", "__Function.prototype.toString__", toStringFunc, attrs);
            toString.Put("length", 0, 0);

            // 15.3.4.3
            Action<Js.Obj, Js.Obj> applyFunc = (arguments, that) => { throw new NotImplementedException(); };
            var apply = AssignFunction(FunctionPrototype, "apply", "__Function.prototype.apply__", applyFunc, attrs);
            apply.Put("length", 2, 0);

            // 15.3.4.4
            Action<Js.Obj, Js.Obj> callFunc = (arguments, that) => { throw new NotImplementedException(); };
            var call = AssignFunction(FunctionPrototype, "call", "__Function.prototype.call__", callFunc, attrs);
            call.Put("length", 1, 0);

            // FunctionPrototype call
            Func<Js.Obj, Js.Obj, object> functionPrototypeCall = (arguments, that) =>
            {
                return Js.Undefined.Instance;
            };

            FunctionPrototype.Call = functionPrototypeCall;
        }

        void InitFunction()
        {
            // Function internal properties
            Function.Class = "Function";
            Function.Prototype = FunctionPrototype;

            // Function userland properties
            Function.Properties["length"] = new Js.Property("length", 1);
            Function.Properties["prototype"] = 
                new Js.Property(
                    "prototype", 
                    FunctionPrototype, 
                    Js.PropertyAttrs.ReadOnly | 
                    Js.PropertyAttrs.DontEnum | 
                    Js.PropertyAttrs.DontDelete
                );

            // Function call
            Func<Js.Obj, Js.Obj, object> functionCall = (arguments, that) =>
            {
                that.Class = "Function";
                return null;
            };

            Function.Call = functionCall;

            // Function in global scope
            Globals.Properties["Function"] = 
                new Js.Property(
                    "Function", 
                    Function,
                    Js.PropertyAttrs.DontDelete |
                    Js.PropertyAttrs.ReadOnly
                );
        }

        void InitObjectPrototype()
        {
            ObjectPrototype.Properties["constructor"] = new Js.Property("constructor", Object);

            // Attributes for the built in
            // functions on Object.prototype
            var attrs =
                Js.PropertyAttrs.ReadOnly |
                Js.PropertyAttrs.DontDelete |
                Js.PropertyAttrs.DontEnum;

            // 15.2.4.2
            Func<Js.Obj, Js.Obj, string> toStringFunc = (arguments, that) => "[object " + that.Class + "]";
            AssignFunction(ObjectPrototype, "toString", "__Object.prototype.toString__", toStringFunc, attrs);

            // 15.2.4.3
            Func<Js.Obj, Js.Obj, string> toLocaleString = (arguments, that) => "[object " + that.Class + "]";
            AssignFunction(ObjectPrototype, "toLocaleString", "__Object.prototype.toLocaleString__", toLocaleString, attrs);

            // 15.2.4.4
            Func<Js.Obj, Js.Obj, object> valueOf = (arguments, that) => that;
            AssignFunction(ObjectPrototype, "valueOf", "__Object.prototype.valueOf__", valueOf, attrs);

            // 15.2.4.5
            Func<Js.Obj, Js.Obj, object, bool> hasOwnProperty = (arguments, that, name) => that.Properties.ContainsKey(name);
            AssignFunction(ObjectPrototype, "hasOwnProperty", "__Object.prototype.hasOwnProperty__", hasOwnProperty, attrs);

            // 15.2.4.6
            Func<Js.Obj, Js.Obj, object, bool> isPrototypeOf = (arguments, that, other) =>
            {
                if (!(other is Js.Obj))
                    return false;

                var otherProto = ((Js.Obj)other).Prototype;

                while (otherProto != null)
                {
                    if (object.ReferenceEquals(that, otherProto))
                        return true;

                    otherProto = otherProto.Prototype;
                }

                return false;
            };
            AssignFunction(ObjectPrototype, "isPrototypeOf", "__Object.prototype.isPrototypeOf__", isPrototypeOf, attrs);

            // 15.2.4.7
            Func<Js.Obj, Js.Obj, object, bool> propertyIsEnumerable = (arguments, that, name) =>
            {
                Js.Property prop;

                if (that.Properties.TryGetValue(name, out prop))
                    return prop.NotHasAttr(Js.PropertyAttrs.DontEnum);

                return false;
            };
            AssignFunction(ObjectPrototype, "propertyIsEnumerable", "__Object.prototype.propertyIsEnumerable__", propertyIsEnumerable, attrs);
        }

        void InitObject()
        {
            Func<Js.Obj, Js.Obj, object> call = (arguments, that) =>
            {
                return null;
            };

            Object.Call = call;
            Object.Prototype = FunctionPrototype;

            Object.Properties["length"] = new Js.Property("length", 1);
            Object.Properties["prototype"] = new Js.Property("prototype", ObjectPrototype);

            // Set global
            Globals.Properties["Object"] = 
                new Js.Property(
                    "Object", 
                    Object, 
                    Js.PropertyAttrs.ReadOnly | 
                    Js.PropertyAttrs.DontDelete
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyName"></param>
        /// <param name="objName"></param>
        /// <param name="func"></param>
        /// <param name="attrs"></param>
        Js.Obj AssignFunction(Js.Obj target, string propertyName, string objName, Delegate func, Js.PropertyAttrs attrs)
        {
            var funcObj = CreateFunctionObject(objName);

            funcObj.Call = func;

            target.Properties[propertyName] = 
                new Js.Property(
                    propertyName, 
                    funcObj, 
                    attrs
                );

            return funcObj;
        }
    }
}
