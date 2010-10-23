namespace IronJS.Compiler

open IronJS

module Errors =

  let noBreakTargetAvailable() =
    Errors.compiler "No unlabeled break target available"

  let noContinueTargetAvailable() =
    Errors.compiler "No unlabeled continue target available"

  let missingLabel label =
    Errors.compiler (sprintf "Missing label %s" label)

  let topNodeInFunctionMustBeLocalScope() =
    let error = 
      "The top Ast.Tree node in a "+
      "Ast.Tree.Function node must "+
      "be an Ast.Tree.LocalScope node"

    Errors.compiler error