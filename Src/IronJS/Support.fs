namespace IronJS.Support

open IronJS
open System

module CustomOperators =
  let inline ($) a b = b a
  let inline (==) a b = Object.ReferenceEquals(a, b)

module Attributes = 

  type HiddenMethod() = inherit Attribute()
  type HiddenProperty() = inherit Attribute()
  type HiddenField() = inherit Attribute()

  type JsArgumentsLength(length:int) =
    inherit Attribute()
    member x.Length = length

module Aliases = 
  
  open System.Globalization
  open System.Collections.Generic

  type MutableList<'a> = List<'a>
  type MutableStack<'a> = Stack<'a>
  type MutableDict<'k, 'v> = Dictionary<'k, 'v>
  type MutableSorted<'k, 'v> = SortedDictionary<'k, 'v>
  type MutableSet<'a> = HashSet<'a>
  
  let anyNumber = NumberStyles.Any
  let invariantCulture = CultureInfo.InvariantCulture
  let currentCulture = CultureInfo.CurrentCulture

  let NaN = Double.NaN
  let NegInf = Double.NegativeInfinity 
  let PosInf = Double.PositiveInfinity

module Debug =
  
  let private astPrinters = new Aliases.MutableList<Action<string>>()
  let private exprPrinters = new Aliases.MutableList<Action<string>>()
  
  let registerAstPrinter x = astPrinters.Add x
  let registerExprPrinter x = exprPrinters.Add x

  let printExpr (expr:Dlr.Expr) =
    let expr = expr |> IronJS.Dlr.Utils.debugView 
    for printer in exprPrinters do printer.Invoke(expr)

  let printAst (ast:obj) =
    let ast = sprintf "%A" ast
    for printer in astPrinters do printer.Invoke(ast)

  let registerConsolePrinter () =
    let print = new Action<string>(fun s -> printfn "%s" s)
    print |> registerAstPrinter
    print |> registerExprPrinter