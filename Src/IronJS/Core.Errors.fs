namespace IronJS
  
open System

type Error(msg) = inherit Exception(msg)
type AstError(msg) = inherit Error(msg)
type ParserError(msg) = inherit Error(msg)
type CompilerError(msg) = inherit Error(msg)
type RuntimeError(msg) = inherit Error(msg)

module Errors =

  let ast msg = raise (AstError msg)
  let generic msg = raise (Error msg)
  let parser msg = raise (ParserError msg)
  let compiler msg = raise (CompilerError msg)
  let runtime msg = raise (RuntimeError msg)

  module Generic = 
    let invalidTypeCode (tc:uint32) =
      generic (sprintf "Invalid typecode %d" tc)

    let noConversion (from:System.Type) (to':System.Type) =
      generic (sprintf "No conversion from %s to %s exists" from.Name to'.Name)
    
    let notImplemented () =
      raise (NotImplementedException "IronJS is currently missing this feature")