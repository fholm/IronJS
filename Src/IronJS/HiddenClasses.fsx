#light
#r "../Dependencies/Antlr3.Runtime.dll"
#r "../Dependencies/Microsoft.Dynamic.dll"
#r "../Dependencies/Microsoft.Scripting.dll"
#r "../Dependencies/Antlr3.Runtime.dll"
#r "../IronJS.CSharp/bin/Debug/IronJS.CSharp.dll"
#load "Fsi.fs"
#load "Monads.fs"
#load "Operators.fs"
#load "Constants.fs"
#load "Utils.fs"
#load "Tools.fs"
#load "Tools.Dlr.Expr.fs"
#load "Tools.Dlr.Restrict.fs"
#load "Tools.Js.fs"
#load "Tools.Type.fs"
#load "Ast.Types.fs"
#load "Ast.Helpers.fs"
#load "Ast.Analyzer.fs"
#load "Ast.fs"
#load "Runtime.fs"
#load "Runtime.Function.fs"
#load "Runtime.Environment.fs"
#load "Runtime.Helpers.fs"
#load "Runtime.Binders.fs"
#load "Runtime.Closures.fs"
#load "Compiler.Types.fs"
#load "Compiler.Helpers.fs"
#load "Compiler.Helpers.Variables.fs"
#load "Compiler.Helpers.Object.fs"
#load "Compiler.Helpers.ExprGen.fs"
#load "Compiler.Helpers.Closure.fs"
#load "Compiler.Analyzer.fs"
#load "Compiler.ExprGen.fs"
#load "Compiler.fs"

open IronJS
open System
open System.Collections.Generic

type VarDict = Dictionary<string, int>

type Class =
  val mutable Vars : VarDict
  val mutable SubClasses : Dictionary<string, Class>

  new(vars:VarDict) = { 
    Vars = vars
    SubClasses = new Dictionary<string, Class>()
  }

  member x.SubClass propertyName = 
    let success, value = x.SubClasses.TryGetValue propertyName
    if success 
      then value
      else let newVars = new VarDict(x.Vars)
           newVars.Add(propertyName, newVars.Count)
           let newClass = new Class(newVars)
           x.SubClasses.Add(propertyName, newClass)
           newClass

type Obj =
  val mutable Class : Class
  val mutable Properties : obj array

  new(cls:Class) = {
    Class = cls
    Properties = Array.zeroCreate 4
  }

  member x.Set name (value:obj) = 
    let success, index = x.Class.Vars.TryGetValue name
    if success 
      then  x.Properties.[index] <- value
            index

      else  x.Class <- x.Class.SubClass name
            if x.Class.Vars.Count > x.Properties.Length 
              then  let newProps = Array.zeroCreate (x.Properties.Length * 2)
                    Array.Copy(x.Properties, newProps, x.Properties.Length)
                    x.Properties <- newProps
            x.Set name value

  member x.Get name =
    let success, index = x.Class.Vars.TryGetValue name
    if success 
      then x.Properties.[index], index
      else Runtime.Core.Undefined.Instance :> obj, -1

  member x.Has name = x.Class.Vars.ContainsKey name

let baseClass = new Class(new VarDict())