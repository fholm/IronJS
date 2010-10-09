namespace IronJS
  
open System

type ParserError(msg)   = inherit Exception(msg)
type CompilerError(msg) = inherit Exception(msg)
type RuntimeError(msg)  = inherit Exception(msg)

module Errors =
  let parser msg = raise (ParserError(msg))
  let compiler msg = raise (CompilerError(msg))
  let runtime msg = raise (RuntimeError(msg))
