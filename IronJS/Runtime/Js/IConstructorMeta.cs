using System.Dynamic;
using IronJS.Runtime.Utils;
using Microsoft.Scripting.Utils;
using AstUtils = Microsoft.Scripting.Ast.Utils;

using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Js
{
    class IConstructorMeta : IFunctionMeta
    {
        public IConstructorMeta(Et parameter, IConstructor constructor)
            : base(parameter, constructor)
        {

        }

        public override Meta BindCreateInstance(CreateInstanceBinder binder, Meta[] args)
        {
            return new Meta(
                Et.Call(
                    EtUtils.Cast<IConstructor>(
                        this.Expression
                    ),
                    IConstructorMethods.MiConstruct,
                    AstUtils.NewArrayHelper(
                        typeof(object),
                        DynamicUtils.GetExpressions(args)
                    )
                ),
                RestrictUtils.BuildCallRestrictions(
                    this,
                    args,
                    RestrictFlag.Type
                )
            );
        }
    }
}
