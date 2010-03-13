using System.Dynamic;

namespace IronJS.Tools {
    using Restrict = BindingRestrictions;
    using MetaObj = DynamicMetaObject;

    public static class RestrictUtils {

        public static Restrict GetTypeRestrictions(MetaObj[] objs) {
            Restrict objRestrict = null;
            Restrict mergedRestrict = Restrict.Empty;

            foreach (MetaObj obj in objs) {
                if (obj.HasValue && obj.Value == null) {
                    objRestrict = Restrict.GetInstanceRestriction(obj.Expression, null);
                } else {
                    objRestrict = Restrict.GetTypeRestriction(obj.Expression, obj.LimitType);
                }

                mergedRestrict = mergedRestrict.Merge(objRestrict);
            }

            return mergedRestrict;
        }
    }
}
