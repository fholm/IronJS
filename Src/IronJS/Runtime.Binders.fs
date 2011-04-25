namespace IronJS.Runtime.Binders

open IronJS
open System.Dynamic
open System.Reflection

type GetMemberBinder(name:obj, env:Env) =
  inherit System.Dynamic.GetMemberBinder(name.ToString(), false)

  static let flags =
    BindingFlags.Static ||| 
    BindingFlags.Instance ||| 
    BindingFlags.Public

  override x.FallbackGetMember(target, error) =
    let members = target.LimitType.GetMember(x.Name, flags)
    let restriction = Dlr.Br.GetTypeRestriction(target.Expression, target.LimitType)

    if members.Length > 0 then
      let member' = members.[0]
      let casted = Dlr.cast member'.DeclaringType target.Expression
      new Dlr.MetaObject(Dlr.Expr.MakeMemberAccess(casted, member') |> Dlr.castT<obj>, restriction)

    else
      new Dlr.MetaObject(Dlr.propertyStaticT<Undefined> "Instance" |> Dlr.castT<obj>, restriction)
