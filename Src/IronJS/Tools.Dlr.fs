namespace IronJS.Tools.Dlr

open IronJS
open IronJS.Utils

module Restrict =
  
  let notAtAll = 
    System.Dynamic.BindingRestrictions.Empty

  let byExpr expr =
    Restrict.GetExpressionRestriction(expr)

  let byType expr typ =
    Restrict.GetTypeRestriction(expr, typ)

  let byInstance expr instance =
    Restrict.GetInstanceRestriction(expr, instance)

  let rec byArgs (args:MetaObj list) =
    match args with
    | [] -> Restrict.Empty
    | x::xs -> 
      (if x.HasValue && x.Value = null 
          then Restrict.GetInstanceRestriction(x.Expression, Tools.Expr.objDefault)
          else Restrict.GetTypeRestriction(x.Expression, x.LimitType)
      ).Merge(x.Restrictions).Merge(byArgs xs)
