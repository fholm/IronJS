namespace IronJS.Compiler.Utils

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler
open IronJS.Compiler.Types
open IronJS.Compiler.ExpressionState

module Utils = 

    let expandStrongBox (expr:Et) =
      if Type.isStrongBox expr.Type 
        then Expr.field expr "Value"
        else expr

    let assign ctx (left:Et) (right:Et) =
      let l = expandStrongBox left
      let r = expandStrongBox right

      if l.Type = r.Type then Expr.assign l r
      else
        if l.Type = typeof<Runtime.Box> 
          then Utils.Box.assign ctx left right
          else failwith "Odd case not handled yet"