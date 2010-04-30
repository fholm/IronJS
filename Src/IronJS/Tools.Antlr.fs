namespace IronJS.Tools

open IronJS
open IronJS.Tools
open IronJS.Aliases
open IronJS.Parser

open Antlr.Runtime
open Antlr.Runtime.Tree

module Antlr =

  let ct (t:obj) = 
    t :?> AntlrToken

  let text (t:AntlrToken) = 
    t.Text

  let hasChild (t:AntlrToken) index =
    t.ChildCount > index

  let child (t:AntlrToken) index = 
    if hasChild t index then ct t.Children.[index] else null

  let children (t:AntlrToken) = 
    if t.Children = null then []
    else
      t.Children
        |> Seq.cast<AntlrToken> 
        |> Seq.toList

  let childrenOf (t:AntlrToken) n = 
    children (child t n)

  let isAssign (t:AntlrToken) = 
    t.Type = ES3Parser.ASSIGN

  let isAnonymous (t:AntlrToken) = 
    t.Type = ES3Parser.FUNCTION && t.ChildCount = 2

  type FileStream = ANTLRFileStream
  type StringStream = ANTLRStringStream
  type TokenStream = CommonTokenStream