namespace IronJS

open System

module Error =
  
  module SourceCodePrinter = 
  
    open System
    open System.Text

    let private splitLines (input:string) = 
      let input = (string input).Replace("\r\n", "\n").Replace("\r", "\n")
      RegularExpressions.Regex.Split(input, "\n")

    let private lineNum (pad:int) (input:int) = 
      (string input).PadLeft(pad, '0')

    let private makeArrow (times:int) =
      let result = new StringBuilder(times)
      result.Insert(0, "-", times).ToString() + "^"
  
    let sprintError (above:int, below:int) (line:int, column:int) (source:string) =
      let source = source |> splitLines
      let result = ref ""

      if line <= source.Length && line > 0 then

        let text = source.[line - 1]
        let nrLength = (line+below |> string).Length
      
        // Deal with lines above the error one
        for i = 1 to above do
          let prevLine = line - i

          if prevLine >= 1 then
            let num = prevLine |> lineNum nrLength
            let text = sprintf "%s: %s\n" num source.[prevLine-1] 
            result := text + !result

        // Error line and arrow
        let arrow = nrLength + column + 1 |> makeArrow
        let num = line |> lineNum nrLength
        let text = sprintf "%s: %s\n%s\n" num text arrow
        result := !result + text

        // Deal with lines below the error one
        for i = 1 to below do
          let nextLine = line + i

          if nextLine <= source.Length then
            let num = nextLine |> lineNum nrLength
            let text = sprintf "%s: %s\n" num source.[nextLine-1] 
            result := !result + text

      !result

  /// Base class of all IronJS exceptions
  [<AbstractClass>]
  type Error(msg) = 
    inherit Exception(msg)

  /// Represents an error raised from the 
  /// lexer, parser, analyzer or compiler
  type CompileError(msg, pos, source, path) = 
    inherit Error(msg)

    member x.Position = match pos with Some pos -> pos | _ -> 0, 0 
    member x.SourceCode = match source with Some source -> source | _ -> "" 
    member x.Path = match path with Some path -> path | _ -> ""

    static member Raise(msg) = 
      raise(CompileError(msg, None, None, None))

    static member Raise(msg, pos) = 
      raise(CompileError(msg, Some pos, None, None))

    static member Raise(msg, pos, source) = 
      raise(CompileError(msg, Some pos, Some source, None))

    static member Raise(msg, pos, source, path) = 
      raise(CompileError(msg, Some pos, Some source, Some path))

  /// Represents an error raised during runtime
  type RuntimeError(msg) = 
    inherit Error(msg)

    static member Raise(msg) = 
      raise(RuntimeError(msg))
  
  let notImplemented() = raise <| NotImplementedException()
  let invalidTypeTag (tag:uint32) = sprintf "Invalid type tag %i" tag
  let missingVariable = sprintf "Missing variable '%s'"