namespace IronJS.Tools.Dlr

open IronJS
open IronJS.Aliases
open IronJS.Tools

(*Tools for working with the System.Dynamic.BindingRestrictions class*)
module Restrict =

  type private BR = System.Dynamic.BindingRestrictions
  
  let notAtAll = System.Dynamic.BindingRestrictions.Empty
  let byExpr expr = BR.GetExpressionRestriction(expr)
  let byType expr typ = BR.GetTypeRestriction(expr, typ)
  let byInstance expr instance = BR.GetInstanceRestriction(expr, instance)

  let argRestrict (a:MetaObj) =
    let restriction = 
      if a.HasValue && a.Value = null 
        then BR.GetInstanceRestriction(a.Expression, Dlr.Expr.null')
        else BR.GetTypeRestriction(a.Expression, a.LimitType)

    a.Restrictions.Merge(restriction)

  let byArgs (args:MetaObj seq) =
    Seq.fold (fun (s:BR) a -> s.Merge(argRestrict a)) BR.Empty args