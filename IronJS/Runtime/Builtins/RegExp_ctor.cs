using System;
using System.Text.RegularExpressions;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Builtins
{
    public class RegExp_ctor : NativeConstructor
    {
        public IObj RegExp_prototype { get; private set; }

        public RegExp_ctor(Context context)
            : base(context)
        {
            RegExp_prototype = new RegExp_prototype(Context);
            RegExp_prototype.Set("constructor", this);

            this.Set("prototype", RegExp_prototype);
            this.Set("length", 2.0D);
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
            regExpObj.Set("ignoreCase", ignoreCase);
            regExpObj.Set("global", global);
            regExpObj.Set("multiline", multiline);
            regExpObj.Set("source", pattern);
            regExpObj.Set("lastIndex", 0.0D);

            return regExpObj;
        }

        override public object Call(IObj that, object[] args)
        {
            return Construct(args);
        }

        override public IObj Construct(object[] args)
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
    }
}
