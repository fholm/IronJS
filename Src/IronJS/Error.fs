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

  /// Partially applied sprintError function
  /// that prints the default 3 lines above and
  /// 0 lines after for IronJS
  let sprintError = 
    SourceCodePrinter.sprintError (3, 0)

  /// Base class of all IronJS exceptions
  [<AbstractClass>]
  type Error(msg) = 
    inherit Exception(msg)

  /// Represents an error raised from the 
  /// lexer, parser, analyzer or compiler
  type CompileError(msg, pos, source, path) = 
    inherit Error(msg)

    member x.Position = match pos with Some pos -> pos | _ -> 0, 0 
    member x.SourceCode = match source with Some source -> source | _ -> "<unknown>" 
    member x.Path = match path with Some path -> path | _ -> "<unknown>"

    static member Raise(msg) = 
      raise(CompileError(msg, None, None, None))

    static member Raise(msg, pos) = 
      let msg = sprintf "%s\nOn line %i, column %i" msg (fst pos) (snd pos)
      raise(CompileError(msg, Some pos, None, None))

    static member Raise(msg, pos, source) = 
      let source = source |> sprintError pos
      let msg = sprintf "%s\nOn line %i, column %i\n%s" msg (fst pos) (snd pos) source
      raise(CompileError(msg, Some pos, Some source, None))

    static member Raise(msg, pos, source, path) = 
      let source = source |> sprintError pos
      let msg = sprintf "%s\nOn line %i, column %i in %s\n%s" msg (fst pos) (snd pos) path source
      raise(CompileError(msg, Some pos, Some source, Some path))

  /// Represents an error raised during runtime
  type RuntimeError(msg) = 
    inherit Error(msg)

    static member Raise(msg) = 
      raise(RuntimeError(msg))
  
  // This is the full list of error messages
  // that can be thrown from IronJS

  let notImplemented() = raise <| NotImplementedException()
  let shouldNotHappen() = RuntimeError.Raise("Should not happen")
  let invalidTypeTag (tag:uint32) = sprintf "Invalid type tag '%i'" tag
  let missingVariable = sprintf "Missing variable '%s'"
  let astMustBeNamedFunction = "The AST top-node must be a named function"
  let invalidSimplePunctuation = sprintf "Invalid punctuation '%c'"
  let invalidHexDigit = sprintf "Invalid hex digit '%c'"
  let invalidOctalDigit = sprintf "Invalid octal digit '%c'"
  let newlineIn = sprintf "Unexpected line terminator in %s"
  let unexpectedEnd = "Unexpected end of source input"
  let unrecognizedInput = sprintf "Unrecognized input '%c'"
  let cantCallClrFunctionsInWith = "Can't call CLR functions or methods within dynamic with-blocks"
  let variableIndexOutOfRange = "Variable active index is out of range"
  let missingContinue = "No continue target available"
  let missingBreak = "No break target available"
  let missingLabel = sprintf "No label named '%s' available"
  let missingNoConversion (a:Type) (b:Type) = sprintf "No conversion from %s to type %s exists" a.Name b.Name

