namespace IronJS.Tools.Dlr

open IronJS
open IronJS.Utils
open IronJS.Tools

(*Tools for working with the System.Dynamic.BindingRestrictions class*)
module Restrict =

  type private BindingRestrictions = System.Dynamic.BindingRestrictions
  
  let notAtAll = System.Dynamic.BindingRestrictions.Empty
  let byExpr expr = BindingRestrictions.GetExpressionRestriction(expr)
  let byType expr typ = BindingRestrictions.GetTypeRestriction(expr, typ)
  let byInstance expr instance = BindingRestrictions.GetInstanceRestriction(expr, instance)

  let rec byArgs (args:MetaObj list) =
    match args with
    | [] -> BindingRestrictions.Empty
    | x::xs -> 
      (if x.HasValue && x.Value = null 
          then BindingRestrictions.GetInstanceRestriction(x.Expression, Dlr.Expr.dynamicDefault)
          else BindingRestrictions.GetTypeRestriction(x.Expression, x.LimitType)
      ).Merge(x.Restrictions).Merge(byArgs xs)
