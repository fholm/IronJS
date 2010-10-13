namespace IronJS

open System
open IronJS
open IronJS.Aliases

module Debug =

  let exprPrinters = new MutableList<Action<Dlr.Expr>>()
  let stringPrinters = new MutableList<Action<string>>()

  let printExpr expr =
    #if DEBUG
    for printer in exprPrinters do
      printer.Invoke(expr)
    #else
    ()
    #endif

  let printString string =
    #if DEBUG
    for printer in stringPrinters do
      printer.Invoke(string)
    #else
    ()
    #endif
    

