#light
#r "../Dependencies/Antlr3.Runtime.dll"
#r "../Dependencies/Microsoft.Dynamic.dll"
#r "../Dependencies/Microsoft.Scripting.dll"
#r "../Dependencies/Antlr3.Runtime.dll"
#r "../IronJS.CSharp/bin/Debug/IronJS.CSharp.dll"
#load "EtTools.fs"
#load "ClrTypes.fs"
#load "Utils.fs"
#load "Ast.fs"
#load "Compiler.fs"
#load "Runtime.fs"

System.IO.Directory.SetCurrentDirectory("C:\\Users\\Fredrik\\Projects\\IronJS\\Src\\IronJS")

open Ast
open System
open Compiler
open EtTools
open IronJS.CSharp.Parser
open Antlr.Runtime
open Antlr.Runtime.Tree
open System.Linq.Expressions

let jsLexer = new ES3Lexer(new ANTLRFileStream("Testing.js"))
let jsParser = new ES3Parser(new CommonTokenStream(jsLexer))
let program = jsParser.program()

let ast = generator (ct program.Tree)

let clrToJsType x = 
  if x = ClrTypes.Integer then Type.Integer
  elif x = ClrTypes.Double then Type.Double
  elif x = ClrTypes.String then Type.String
  else Type.Dynamic

let rec getVarType (name:string) (scope:Scope) (evaling:string Set) =
  if evaling.Contains(name) then
    Type.None
  else
    let local = scope.Locals.[name]
    
    match local.ForcedType with
    | Some(t) -> clrToJsType(t)
    | None -> 
      match local.UsedAs with
      | Type.Integer 
      | Type.Double  
      | Type.String  
      | Type.Object -> 
        let evalingSet = evaling.Add(name)

        let rec evalUsedWith vars =
          match vars with
          | [] -> Type.None
          | x::xs -> (getVarType x scope evalingSet) ||| (evalUsedWith xs)

        local.UsedWith |> List.ofSeq |> evalUsedWith

      | _ -> Type.Dynamic

let createDelegateType (types:System.Type list) =
  Et.GetFuncType(List.toArray (List.append types [ClrTypes.Dynamic]))

let rec createTypedScope (parms: string list) (inTypes:System.Type list) (scope: Scope) = 
  match parms with
  | [] -> scope
  | x::xs -> 
    let typ, types = match inTypes with 
                     | [] -> typeToClr(Type.Dynamic), [] 
                     | x::xs -> x, xs

    let local = { scope.Locals.[x] with ForcedType = Some(typ) }
    createTypedScope xs types { scope with Locals = scope.Locals.Add(x, local) }

type Context = {
  Locals: Map<string, EtParam>
  Return: LabelTarget
}

let createContext (s:Scope) = {
  Context.Locals =  Map.map (fun k v -> Et.Parameter(v.ForcedType.Value, k)) s.Locals;
  Return = label "~return";
}

let rec etgen node ctx =
  match node with
  | Var(n) -> etgen n ctx 
  | Block(n) -> genBlock n ctx
  | _ -> AstUtils.Empty() :> Et

and genBlock nodes ctx =
  block [for n in nodes -> etgen n ctx]

let compile func (types:System.Type list) =
  match func with 
  | Function(parms, genericScope, name, body) ->

    let typedScope = createTypedScope parms types genericScope
    let funcType = createDelegateType types
    let untypedLocals = typedScope.Locals |> Map.filter (fun k v -> v.ForcedType = None)
    let context = createContext typedScope

    let lambda = 
      Et.Lambda(
        funcType,
        block [etgen body context; labelExpr context.Return],
        [for p in parms -> context.Locals.[p]]
      )

    lambda.Compile()

  | _ -> failwith "Can only compile Function nodes"

(compile ast [ClrTypes.Dynamic] :?> Func<obj, obj>)