namespace IronJS.Tools

open IronJS
open IronJS.Tools
open IronJS.Aliases
open IronJS.Monads
open IronJS.Parser

open Antlr.Runtime
open Antlr.Runtime.Tree

module Antlr =

  let internal ct (tree:obj) = tree :?> AstTree
  let internal hasChild (tree:AstTree) index = tree.ChildCount > index
  let internal child (tree:AstTree) index = if hasChild tree index then ct tree.Children.[index] else null
  let internal children (tree:AstTree) = InterOp.toList<AstTree> tree.Children
  let internal childrenOf (tree:AstTree) n = children (child tree n)
  let internal isAssign (tree:AstTree) = tree.Type = ES3Parser.ASSIGN
  let internal isAnonymous (tree:AstTree) = tree.Type = ES3Parser.FUNCTION && tree.ChildCount = 2

  type FileStream = ANTLRFileStream
  type StringStream = ANTLRStringStream
  type TokenStream = CommonTokenStream