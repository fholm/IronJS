namespace IronJS
  
open System

exception Error of string
exception ParserError of string
exception CompilerError of string
exception RuntimeError of string

module Errors =
  let generic msg = raise (Error(msg))
  let parser msg = raise (ParserError(msg))
  let compiler msg = raise (CompilerError(msg))
  let runtime msg = raise (RuntimeError(msg))

  module Generic = 
    let invalidTypeCode (tc:int16) =
      generic (sprintf "Invalid typecode %d" tc)

    let noConversion (from:System.Type) (to':System.Type) =
      generic (sprintf "No conversion from %s to %s exists" from.Name to'.Name)
    
    let notImplemented () =
      raise (NotImplementedException("Missing feature"))