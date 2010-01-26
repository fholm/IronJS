using IronJS.Runtime.Js;
using IronJS.Runtime.Js.Descriptors;

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

            this.Set("toString", new String_prototype_toString(Context));
            this.Set("valueOf", new String_prototype_valueOf(Context));
            this.Set("charAt", new String_prototype_charAt(Context));
            this.Set("charCodeAt", new String_prototype_charCodeAt(Context));
            this.Set("concat", new String_prototype_concat(Context));
            this.Set("indexOf", new String_prototype_indexOf(Context));
            this.Set("lastIndexOf", new String_prototype_lastIndexOf(Context));
            this.Set("match", new String_prototype_match(Context));
            this.Set("replace", new String_prototype_replace(Context));
            this.Set("search", new String_prototype_search(Context));
            this.Set("slice", new String_prototype_slice(Context));
            this.Set("split", new String_prototype_split(Context));
            this.Set("substring", new String_prototype_substring(Context));
            this.Set("toLowerCase", new String_prototype_toLowerCase(Context));
            this.Set("toLocaleLowerCase", new String_prototype_toLocaleLowerCase(Context));
            this.Set("toUpperCase", new String_prototype_toUpperCase(Context));
            this.Set("toLocaleUpperCase", new String_prototype_toLocaleUpperCase(Context));

        }
    }
}
