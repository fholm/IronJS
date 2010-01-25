using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    class String_prototype : ValueObj
    {
        public String_prototype(Context context)
            : base("")
        {
            Context = context;
            Prototype = context.ObjectConstructor.Object_prototype;
            Class = ObjClass.String;

            SetOwn("toString", new String_prototype_toString(Context));
            SetOwn("valueOf", new String_prototype_valueOf(Context));
            SetOwn("charAt", new String_prototype_charAt(Context));
            SetOwn("charCodeAt", new String_prototype_charCodeAt(Context));
            SetOwn("concat", new String_prototype_concat(Context));
            SetOwn("indexOf", new String_prototype_indexOf(Context));
            SetOwn("lastIndexOf", new String_prototype_lastIndexOf(Context));
            SetOwn("match", new String_prototype_match(Context));
            SetOwn("replace", new String_prototype_replace(Context));
            SetOwn("search", new String_prototype_search(Context));
            SetOwn("slice", new String_prototype_slice(Context));
            SetOwn("split", new String_prototype_split(Context));
            SetOwn("substring", new String_prototype_substring(Context));
            SetOwn("toLowerCase", new String_prototype_toLowerCase(Context));
            SetOwn("toLocaleLowerCase", new String_prototype_toLocaleLowerCase(Context));
            SetOwn("toUpperCase", new String_prototype_toUpperCase(Context));
            SetOwn("toLocaleUpperCase", new String_prototype_toLocaleUpperCase(Context));
        }
    }
}
