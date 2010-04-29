namespace IronJS.Tools

open IronJS
open IronJS.Tools
open IronJS.Aliases
open IronJS.Parser

open Antlr.Runtime
open Antlr.Runtime.Tree

module Antlr =

  let internal ct (tree:obj) = tree :?> AntlrToken
  let internal hasChild (tree:AntlrToken) index = tree.ChildCount > index
  let internal child (tree:AntlrToken) index = if hasChild tree index then ct tree.Children.[index] else null
  let internal children (tree:AntlrToken) = InterOp.toList<AntlrToken> tree.Children
  let internal childrenOf (tree:AntlrToken) n = children (child tree n)
  let internal isAssign (tree:AntlrToken) = tree.Type = ES3Parser.ASSIGN
  let internal isAnonymous (tree:AntlrToken) = tree.Type = ES3Parser.FUNCTION && tree.ChildCount = 2

  type FileStream = ANTLRFileStream
  type StringStream = ANTLRStringStream
  type TokenStream = CommonTokenStream