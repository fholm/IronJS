namespace IronJS.Support

open System

type ExposeClassAttribute() = 
  inherit Attribute()

type ExposeMemberAttribute(asName:string) = 
  inherit Attribute()
  member x.AsName = asName

module Aliases = 
  
  open System.Globalization
  open System.Collections.Generic
  open System.Collections.Concurrent

  type MutableList<'a> = List<'a>
  type MutableStack<'a> = Stack<'a>
  type MutableDict<'k, 'v> = Dictionary<'k, 'v>
  type MutableSorted<'k, 'v> = SortedDictionary<'k, 'v>
  type MutableSet<'a> = HashSet<'a>
  type Pair<'a, 'b> = Tuple<'a, 'b>
  type ConcurrentMutableDict<'k, 'v> = ConcurrentDictionary<'k, 'v>
  type AntlrToken = Antlr.Runtime.Tree.CommonTree

  let anyNumber = NumberStyles.Any
  let invariantCulture = CultureInfo.InvariantCulture

  let NaN = Double.NaN
  let NegInf = Double.NegativeInfinity 
  let PosInf = Double.PositiveInfinity

module Debug =

  let private printers = 
    new Aliases.MutableList<Action<string>>()

  let internal print x =
    for printer in printers do
      printer.Invoke (sprintf "%A" x)

  let registerPrinter x = 
    printers.Add x

type Error(msg) = inherit Exception(msg)
type CompilerError(msg) = inherit Error(msg)
type RuntimeError(msg) = inherit Error(msg)

module Errors =

  let error msg = raise (Error msg)
  let compiler msg = raise (CompilerError msg)
  let runtime msg = raise (RuntimeError msg)

  let notImplemented () = 
    runtime "IronJS is currently missing this feature"

  let invalidTypeTag (tag:uint32) = 
    runtime (sprintf "Invalid type tag %i" tag)

  let noConversion (from:System.Type) (to':System.Type) = 
    runtime (sprintf "No conversion from %s to %s exists" from.Name to'.Name)
    
  let noBreakTargetAvailable () =
    compiler "No unlabeled break target available"

  let noContinueTargetAvailable () =
    compiler "No unlabeled continue target available"

  let missingLabel label =
    compiler (sprintf "Missing label %s" label)

  let topNodeInFunctionMustBeLocalScope () =
    let error = 
      "The top Ast.Tree node in a "
      + "Ast.Tree.Function node must "
      + "be an Ast.Tree.LocalScope node"

    compiler error

  let invalidCaseNodeType () =
    compiler "Switch case nodes must be either Ast.Case or Ast.Default"

  let emptyScopeChain () = 
    compiler "Empty scope chain"

  let missingVariable n = 
    compiler (sprintf "Missing variable '%s'" n)

  let variableIndexOutOfRange () = 
    compiler "Active index larger then indexes array length"
      
  let shouldBeForToken () =
    compiler "Token should be FORSTEP or FORITER"

  let shouldBeDefaultOrCase () =
    compiler "Should be CASE or DEFAULT"

  let syntaxError line col =
    compiler (sprintf "Syntax Error at line %i after column %i" line col)

  let noParserForToken (token:Aliases.AntlrToken) =
    let name = Xebic.ES3.ES3Parser.tokenNames.[token.Type]
    compiler (sprintf "No parser for token %s (%i)" name token.Type)

  let emptyChildrenList () =
    compiler "No children exists for node"

  let invalidRegexModifier c =
    compiler (sprintf "Invalid regex modifier '%c'" c)