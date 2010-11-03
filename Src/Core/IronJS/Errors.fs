namespace IronJS
  
open System

exception Error of string
exception AstError of string
exception ParserError of string
exception CompilerError of string
exception RuntimeError of string

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
      raise (NotImplementedException "Missing feature")

  module Compiler =
    let binaryFailed op (l:uint32) (r:uint32) =
      compiler (sprintf "Failed to compile operator %A for types %i %i" op l r)