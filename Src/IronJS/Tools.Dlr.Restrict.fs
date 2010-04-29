namespace IronJS.Tools.Dlr

open IronJS
open IronJS.Aliases
open IronJS.Tools

(*Tools for working with the System.Dynamic.BindingRestrictions class*)
module Restrict =

  type private BindingRestrictions = System.Dynamic.BindingRestrictions
  
  let notAtAll = System.Dynamic.BindingRestrictions.Empty
  let byExpr expr = BindingRestrictions.GetExpressionRestriction(expr)
  let byType expr typ = BindingRestrictions.GetTypeRestriction(expr, typ)
  let byInstance expr instance = BindingRestrictions.GetInstanceRestriction(expr, instance)

  let argRestrict (a:MetaObj) =
    let restriction = 
      if a.HasValue && a.Value = null 
        then BindingRestrictions.GetInstanceRestriction(a.Expression, Dlr.Expr.null')
        else BindingRestrictions.GetTypeRestriction(a.Expression, a.LimitType)

    a.Restrictions.Merge(restriction)

  let byArgs (args:MetaObj seq) =
    Seq.fold (fun (s:Restrict) a -> s.Merge(argRestrict a)) BindingRestrictions.Empty args