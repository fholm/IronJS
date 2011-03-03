namespace IronJS.Native

open System
open IronJS
open IronJS.Compiler
open IronJS.Support.Aliases
open IronJS.DescriptorAttrs

(*
//  This module implements the javascript Global object, its functions and values.
// 
//  State (ECMA-262 3rd Edition):
//  15.1.1.1 NaN - DONE
//  15.1.1.2 Infinity - DONE
//  15.1.1.3 undefined - DONE
//  15.1.2.1 eval (x) - DONE
//  15.1.2.2 parseInt (string , radix) - DONE
//  15.1.2.3 parseFloat (string) - DONE
//  15.1.2.4 isNaN (number) - DONE
//  15.1.2.5 isFinite (number) - DONE
//  15.1.3.1 decodeURI (encodedURI) - DONE
//  15.1.3.2 decodeURIComponent (encodedURIComponent) - DONE
//  15.1.3.3 encodeURI (uri) - DONE
//  15.1.3.4 encodeURIComponent (uriComponent) - DONE
//  15.1.5.1 Math - DONE (Native.Math.fs)
*)

module Global =

  let eval (target:Compiler.EvalTarget) =
    match target.Target.Tag with
    | TypeTags.String ->

      let ast = Parsers.Ecma3.parse target.Function.Env target.Target.String
      let scope = {Ast.Scope.New with Closures=target.Closures}
      let tree = Ast.Function(None, scope, ast)
      let levels = Some(target.GlobalLevel, target.ClosureLevel)

      let compiled = 
        Core.compile {
          Ast = Ast.Analyzers.applyDefault tree levels
          TargetMode = TargetMode.Eval
          Delegate = None
          Environment = target.Function.Env
        }

      IronJS.Utils.box (
        compiled.DynamicInvoke(
          target.Function,
          target.This,
          target.LocalScope,
          target.ClosureScope,
          target.DynamicScope))

    | _ -> target.Target

  let parseInt (str:string) = 
    Utils.boxNumber(double (System.Int32.Parse(str)))

  let parseFloat (str:string) = 
    Utils.boxNumber(TypeConverter.ToNumber str)

  let isNaN (number:double) = 
    Utils.boxBool (number = Double.NaN)

  let isFinite (number:double) =
      if    number = Double.NaN               then false
      elif  number = Double.PositiveInfinity  then false
      elif  number = Double.NegativeInfinity  then false
                                              else true

  // These two arrays are copied from the Jint sources
  let private reservedEncoded = 
    [|';'; ','; '/'; '?'; ':'; '@'; '&'; '='; '+'; '$'; '#'|]

  let private reservedEncodedComponent = 
    [|'-'; '_'; '.'; '!'; '~'; '*'; '\''; '('; ')'; '['; ']'|]
    
  let private replaceChar (uri:string) (c:char) =
    uri.Replace(Uri.EscapeDataString(string c), string c)

  let decodeURI (uri:BoxedValue) =
    match uri.Tag with
    | TypeTags.Undefined -> ""
    | _ ->
      let uri = uri |> TypeConverter.ToString
      Uri.UnescapeDataString(uri.Replace('+', ' '))
      
  let decodeURIComponent = 
    decodeURI
  
  let encodeURI (uri:BoxedValue) =
    match uri.Tag with
    | TypeTags.Undefined -> ""
    | _ ->
      let uri = uri |> TypeConverter.ToString |> Uri.EscapeDataString
      let uri = Array.fold replaceChar uri reservedEncoded
      let uri = Array.fold replaceChar uri reservedEncodedComponent
      uri.ToUpperInvariant()
      
  let encodeURIComponent (uri:BoxedValue) =
    match uri.Tag with
    | TypeTags.Undefined -> ""
    | _ ->
      let uri = uri |> TypeConverter.ToString |> Uri.EscapeDataString
      let uri = Array.fold replaceChar uri reservedEncodedComponent
      uri.ToUpperInvariant()

  let setup (env:Environment) =
    let attrs = DontDelete ||| DontEnum

    env.Globals <- env.NewObject()
    env.Globals.Put("NaN", NaN, attrs) //15.1.1.1
    env.Globals.Put("Infinity", PosInf, attrs) //15.1.1.2
    env.Globals.Put("undefined", Undefined.Instance, attrs) //15.1.1.3

    let eval = new Func<Compiler.EvalTarget, BoxedValue>(eval)
    let eval = Utils.createHostFunction env eval
    env.Globals.Put("eval", eval, DontEnum)

    let parseFloat = new Func<string, BoxedValue>(parseFloat)
    let parseFloat = Utils.createHostFunction env parseFloat
    env.Globals.Put("parseFloat", parseFloat, DontEnum)
    
    let parseInt = new Func<string, BoxedValue>(parseInt)
    let parseInt = Utils.createHostFunction env parseInt
    env.Globals.Put("parseInt", parseInt, DontEnum)
    
    let isNaN = new Func<double, BoxedValue>(isNaN)
    let isNaN = Utils.createHostFunction env isNaN
    env.Globals.Put("isNaN", isNaN, DontEnum)
    
    let isFinite = new Func<double, bool>(isFinite)
    let isFinite = Utils.createHostFunction env isFinite
    env.Globals.Put("isFinite", isFinite, DontEnum)

    let decodeURI = new Func<BoxedValue, string>(decodeURI)
    let decodeURI = Utils.createHostFunction env decodeURI
    env.Globals.Put("decodeURI", decodeURI, DontEnum)
    
    let decodeURIComponent = new Func<BoxedValue, string>(decodeURIComponent)
    let decodeURIComponent = Utils.createHostFunction env decodeURIComponent
    env.Globals.Put("decodeURIComponent", decodeURIComponent, DontEnum)
    
    let encodeURI = new Func<BoxedValue, string>(encodeURI)
    let encodeURI = Utils.createHostFunction env encodeURI
    env.Globals.Put("encodeURI", encodeURI, DontEnum)

    let encodeURIComponent = new Func<BoxedValue, string>(encodeURIComponent)
    let encodeURIComponent = Utils.createHostFunction env encodeURIComponent
    env.Globals.Put("encodeURIComponent", encodeURIComponent, DontEnum)
