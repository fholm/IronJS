using System;
using System.Dynamic;
using System.Linq.Expressions;

namespace IronJS.Runtime
{
    public class Undefined : DynamicObject
    {
        private static readonly Undefined instance;
        private static readonly BoxedValue boxed;

        static Undefined()
        {
            instance = new Undefined();
            boxed = BoxedValue.Box(instance, TypeTags.Undefined);
        }

        private Undefined()
        {
        }

        public static Undefined Instance
        {
            get { return instance; }
        }

        public static BoxedValue Boxed
        {
            get { return boxed; }
        }

        public override string ToString()
        {
            return "undefined";
        }

        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
            switch (binder.Operation)
            {
                case ExpressionType.Add:
                    result = Operators.add(Undefined.boxed, BoxedValue.Box(arg)).UnboxObject();
                    return true;
            }

            return base.TryBinaryOperation(binder, arg, out result);
        }

        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            switch (binder.Operation)
            {
                case ExpressionType.UnaryPlus:
                    result = Operators.plus(Undefined.boxed).UnboxObject();
                    return true;
                case ExpressionType.Negate:
                    result = Operators.minus(Undefined.boxed).UnboxObject();
                    return true;
            }

            return base.TryUnaryOperation(binder, out result);
        }
    }
}
