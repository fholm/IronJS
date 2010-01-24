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

            SetOwnProperty("toString", new String_prototype_toString(Context));
            SetOwnProperty("valueOf", new String_prototype_valueOf(Context));
            SetOwnProperty("charAt", new String_prototype_charAt(Context));
            SetOwnProperty("charCodeAt", new String_prototype_charCodeAt(Context));
            SetOwnProperty("concat", new String_prototype_concat(Context));
            SetOwnProperty("indexOf", new String_prototype_indexOf(Context));
            SetOwnProperty("lastIndexOf", new String_prototype_lastIndexOf(Context));
            SetOwnProperty("match", new String_prototype_match(Context));
            SetOwnProperty("replace", new String_prototype_replace(Context));
            SetOwnProperty("search", new String_prototype_search(Context));
            SetOwnProperty("slice", new String_prototype_slice(Context));
            SetOwnProperty("split", new String_prototype_split(Context));
            SetOwnProperty("substring", new String_prototype_substring(Context));
            SetOwnProperty("toLowerCase", new String_prototype_toLowerCase(Context));
            SetOwnProperty("toLocaleLowerCase", new String_prototype_toLocaleLowerCase(Context));
            SetOwnProperty("toUpperCase", new String_prototype_toUpperCase(Context));
            SetOwnProperty("toLocaleUpperCase", new String_prototype_toLocaleUpperCase(Context));
        }
    }
}
