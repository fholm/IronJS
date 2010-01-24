using System;
using System.Text.RegularExpressions;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Builtins
{
    public class RegExp_ctor : Obj, IFunction
    {
        public IObj RegExp_prototype { get; private set; }

        protected RegExp_ctor(Context context)
        {
            Class = ObjClass.Function;
            Context = context;
            Prototype = context.FunctionConstructor.Function_prototype;

            RegExp_prototype = new RegExp_prototype(Context);
            RegExp_prototype.SetOwnProperty("constructor", this);

            SetOwnProperty("prototype", RegExp_prototype);
            SetOwnProperty("length", 2.0D);
        }

        #region IFunction Members

        public object Call(IObj that, object[] args)
        {
            return Construct(args);
        }

        public IObj Construct(object pattern, object flags)
        {
            var strPattern = JsTypeConverter.ToString(pattern);
            var strFlags = JsTypeConverter.ToString(flags);

            var multiline = strFlags.IndexOf('m') > -1;
            var global = strFlags.IndexOf('g') > -1;
            var ignoreCase = strFlags.IndexOf('i') > -1;

            if (strPattern.IndexOf('^') == 0)
                strPattern = @"\A" + strPattern.Substring(1);

            if (strPattern.IndexOf('$') != -1)
                strPattern = Regex.Replace(strPattern, @"(?=[^\\])\$", multiline ? @"\Z" : @"\z");

            var options = RegexOptions.ECMAScript;

            if (multiline)
                options |= RegexOptions.Multiline;

            if (ignoreCase)
                options |= RegexOptions.IgnoreCase;

            var regExpObj = new RegExpObj();

            regExpObj.Class = ObjClass.RegExp;
            regExpObj.Prototype = RegExp_prototype;
            regExpObj.Context = Context;
            regExpObj.Match = new Regex(strPattern, options);
            regExpObj.SetOwnProperty("ignoreCase", ignoreCase);
            regExpObj.SetOwnProperty("global", global);
            regExpObj.SetOwnProperty("multiline", multiline);
            regExpObj.SetOwnProperty("source", pattern);
            regExpObj.SetOwnProperty("lastIndex", 0.0D);

            return regExpObj;
        }

        public IObj Construct(object[] args)
        {
            if (args.Length <= 0)
                throw new ArgumentException();

            if (args.Length == 1)
                return Construct(
                    JsTypeConverter.ToString(args[0]),
                    ""
                );

            if (args.Length > 1)
                return Construct(
                    JsTypeConverter.ToString(args[0]),
                    JsTypeConverter.ToString(args[1])
                );

            throw new NotImplementedException();
        }

        public bool HasInstance(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDynamicMetaObjectProvider Members

        public Meta GetMetaObject(Et parameter)
        {
            return new IFunctionMeta(parameter, this);
        }

        #endregion

        #region Static

        static public RegExp_ctor Create(Context context)
        {
            return new RegExp_ctor(context);
        }

        #endregion
    }
}
