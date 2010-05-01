namespace IronJS.Compiler.Utils

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Compiler

open System.Dynamic

module Assign = 

    let private expandStrongBox (expr:Et) =
      if Type.isStrongBox expr.Type then (Dlr.Expr.field expr "Value") else expr

    let value ctx (left:Et) (right:Et) =
      let l = expandStrongBox left
      let r = expandStrongBox right

      if l.Type = r.Type then Dlr.Expr.assign l r
      else
        if l.Type = typeof<Runtime.Box> 
          then Utils.Box.assign ctx left right
          else
            // Special cases
            failwith "Not supported"

