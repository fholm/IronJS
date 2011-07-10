using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime
{
    public static class Operators
    {
        /// <summary>
        /// Implements the unary `typeof` operator.
        /// </summary>
        public static string typeOf(BoxedValue value)
        {
            if (value.IsNumber)
            {
                return "number";
            }

            if (value.IsNull)
            {
                return "object";
            }

            return TypeTags.GetName(value.Tag);
        }

        /// <summary>
        /// Implements the unary `!` operator.
        /// </summary>
        public static bool not(BoxedValue value)
        {
            return !TypeConverter.ToBoolean(value);
        }

        /// <summary>
        /// Implements the unary `~` operator.
        /// </summary>
        public static double bitCmpl(BoxedValue value)
        {
            return (double)~TypeConverter.ToInt32(TypeConverter.ToNumber(value));
        }

        /// <summary>
        /// Implements the unary `+` operator.
        /// </summary>
        public static BoxedValue plus(BoxedValue value)
        {
            return BoxedValue.Box(TypeConverter.ToNumber(value));
        }

        /// <summary>
        /// Implements the unary `-` operator.
        /// </summary>
        public static BoxedValue minus(BoxedValue o)
        {
            return BoxedValue.Box((double)(TypeConverter.ToNumber(o) * -1.0));
        }

        /// <summary>
        /// Implements the binary `in` operator.
        /// </summary>
        public static bool @in(Environment env, BoxedValue l, BoxedValue r)
        {
            if (!r.IsObject)
            {
                return env.RaiseTypeError<bool>("Right operand is not a object");
            }
            uint index = 0;
            if (TypeConverter.TryToIndex(l, out index))
            {
                return r.Object.Has(index);
            }
            string name = TypeConverter.ToString(l);
            return r.Object.Has(name);
        }

        /// <summary>
        /// Implements the binary `instanceof` operator.
        /// </summary>
        public static bool instanceOf(Environment env, BoxedValue l, BoxedValue r)
        {
            if (!r.IsFunction)
            {
                return env.RaiseTypeError<bool>("Right operand is not a function");
            }
            if (!l.IsObject)
            {
                return false;
            }
            return r.Func.HasInstance(l.Object);
        }

        /// <summary>
        /// Supports the binary comparison operators.
        /// </summary>
        private static bool Compare(BoxedValue l, BoxedValue r, bool rightToLeft, Func<string, string, bool> stringCompare, Func<double, double, bool> numberCompare)
        {
            if ((l.Tag == TypeTags.String || l.Tag == TypeTags.SuffixString) &&
                (r.Tag == TypeTags.String || r.Tag == TypeTags.SuffixString))
            {
                return stringCompare(
                    l.Clr.ToString(),
                    r.Clr.ToString());
            }

            if (l.IsNumber && r.IsNumber)
            {
                return numberCompare(
                    l.Number,
                    r.Number);
            }

            BoxedValue lPrim, rPrim;
            if (rightToLeft)
            {
                rPrim = TypeConverter.ToPrimitive(r, DefaultValueHint.Number);
                lPrim = TypeConverter.ToPrimitive(l, DefaultValueHint.Number);
            }
            else
            {
                lPrim = TypeConverter.ToPrimitive(l, DefaultValueHint.Number);
                rPrim = TypeConverter.ToPrimitive(r, DefaultValueHint.Number);
            }


            if ((lPrim.Tag == TypeTags.String || lPrim.Tag == TypeTags.SuffixString) &&
                (rPrim.Tag == TypeTags.String || rPrim.Tag == TypeTags.SuffixString))
            {
                return stringCompare(
                    lPrim.Clr.ToString(),
                    rPrim.Clr.ToString());
            }

            var lNum = TypeConverter.ToNumber(lPrim);
            var rNum = TypeConverter.ToNumber(rPrim);
            return numberCompare(
                lNum,
                rNum);
        }

        /// <summary>
        /// Implements the binary `&lt;` operator.
        /// </summary>
        public static bool lt(BoxedValue l, BoxedValue r)
        {
            return Compare(l, r, false,
                (a, b) => string.CompareOrdinal(a, b) < 0,
                (a, b) => a < b);
        }

        /// <summary>
        /// Implements the binary `&lt;=` operator.
        /// </summary>
        public static bool ltEq(BoxedValue l, BoxedValue r)
        {
            return Compare(l, r, true,
                (a, b) => string.CompareOrdinal(a, b) <= 0,
                (a, b) => a <= b);
        }

        /// <summary>
        /// Implements the binary `&gt;` operator.
        /// </summary>
        public static bool gt(BoxedValue l, BoxedValue r)
        {
            return Compare(l, r, true,
                (a, b) => string.CompareOrdinal(a, b) > 0,
                (a, b) => a > b);
        }

        /// <summary>
        /// Implements the binary `&gt;=` operator.
        /// </summary>
        public static bool gtEq(BoxedValue l, BoxedValue r)
        {
            return Compare(l, r, false,
                (a, b) => string.CompareOrdinal(a, b) >= 0,
                (a, b) => a >= b);
        }

        /// <summary>
        /// Implements the binary `==` operator.
        /// </summary>
        public static bool eq(BoxedValue l, BoxedValue r)
        {
            if (Operators.same(l, r))
            {
                return true;
            }

            if (l.IsNull && r.IsUndefined ||
                l.IsUndefined && r.IsNull)
            {
                return true;
            }

            if (l.IsNumber && r.IsString)
            {
                return l.Number == TypeConverter.ToNumber(r);
            }

            if (l.IsString && r.IsNumber)
            {
                return TypeConverter.ToNumber(l) == r.Number;
            }

            if (l.Tag == TypeTags.Bool)
            {
                var newL = BoxedValue.Box(TypeConverter.ToNumber(l));
                return Operators.eq(newL, r);
            }

            if (r.Tag == TypeTags.Bool)
            {
                var newR = BoxedValue.Box(TypeConverter.ToNumber(r));
                return Operators.eq(l, newR);
            }

            if (l.Tag >= TypeTags.Object)
            {
                if (r.Tag == TypeTags.SuffixString || r.Tag == TypeTags.String || r.IsNumber)
                {
                    var newL = TypeConverter.ToPrimitive(l.Object, DefaultValueHint.None);
                    return Operators.eq(newL, r);
                }

                return false;
            }

            if (r.Tag >= TypeTags.Object)
            {
                if (l.Tag == TypeTags.SuffixString || l.Tag == TypeTags.String || l.IsNumber)
                {
                    var newR = TypeConverter.ToPrimitive(r.Object, DefaultValueHint.None);
                    return Operators.eq(l, newR);
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// Implements the binary `!=` operator.
        /// </summary>
        public static bool notEq(BoxedValue l, BoxedValue r)
        {
            return !Operators.eq(l, r);
        }

        /// <summary>
        /// Implements the binary `===` operator.
        /// </summary>
        public static bool same(BoxedValue l, BoxedValue r)
        {
            if (l.IsNumber && r.IsNumber)
            {
                return l.Number == r.Number;
            }

            if ((l.Tag == TypeTags.String || l.Tag == TypeTags.SuffixString) &&
                (r.Tag == TypeTags.String || r.Tag == TypeTags.SuffixString))
            {
                return l.Clr.ToString() == r.Clr.ToString();
            }

            if (l.Tag == r.Tag)
            {
                switch (l.Tag)
                {
                    case TypeTags.Undefined: return true;
                    case TypeTags.Bool: return l.Bool == r.Bool;
                    case TypeTags.Clr:
                    case TypeTags.Function:
                    case TypeTags.Object: return object.ReferenceEquals(l.Clr, r.Clr);
                    default: return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Implements the binary `!==` operator.
        /// </summary>
        public static bool notSame(BoxedValue l, BoxedValue r)
        {
            return !Operators.same(l, r);
        }

        /// <summary>
        /// Implements the binary `+` operator.
        /// </summary>
        public static BoxedValue add(BoxedValue l, BoxedValue r)
        {
            if (l.Tag == TypeTags.SuffixString)
            {
                var newString = SuffixString.Concat(
                    l.SuffixString,
                    TypeConverter.ToString(TypeConverter.ToPrimitive(r)));

                return BoxedValue.Box(newString);
            }

            if (l.Tag == TypeTags.String ||
                r.Tag == TypeTags.String ||
                r.Tag == TypeTags.SuffixString)
            {
                var newString = SuffixString.Concat(
                    TypeConverter.ToString(TypeConverter.ToPrimitive(l)),
                    TypeConverter.ToString(TypeConverter.ToPrimitive(r)));

                return BoxedValue.Box(newString);
            }

            var lPrim = TypeConverter.ToPrimitive(l);
            var rPrim = TypeConverter.ToPrimitive(r);

            if (lPrim.Tag == TypeTags.SuffixString)
            {
                var newString = SuffixString.Concat(
                    lPrim.SuffixString,
                    TypeConverter.ToString(rPrim));

                return BoxedValue.Box(newString);
            }

            if (lPrim.Tag == TypeTags.String ||
                rPrim.Tag == TypeTags.String ||
                rPrim.Tag == TypeTags.SuffixString)
            {
                var newString = SuffixString.Concat(
                    TypeConverter.ToString(lPrim),
                    TypeConverter.ToString(rPrim));

                return BoxedValue.Box(newString);
            }

            return BoxedValue.Box(TypeConverter.ToNumber(lPrim) + TypeConverter.ToNumber(rPrim));
        }
    }
}
