module IronJS.Ast.Helpers

open IronJS
open IronJS.Utils
open IronJS.Ast.Types
open IronJS.CSharp.Parser
open Antlr.Runtime
open Antlr.Runtime.Tree

let ct (tree:obj) = tree :?> AstTree
let child (tree:AstTree) index = (ct tree.Children.[index])
let children (tree:AstTree) = toList<AstTree> tree.Children
let isAssign (tree:AstTree) = tree.Type = ES3Parser.ASSIGN
let isAnonymous (tree:AstTree) = tree.Type = ES3Parser.FUNCTION && tree.ChildCount = 2
let setClosure (scope:Scope) (name:string) (clos:Closure) = { scope with Closure = scope.Closure.Add(name, clos) }
let cleanString = function | null | "" -> "" | s  -> if s.[0] = '"' then s.Trim('"') else s.Trim('\'')
let hasClosure (scope:Scope) name = scope.Closure.ContainsKey name
let hasLocal (scope:Scope) name = scope.Locals.ContainsKey name
let setLocal (scope:Scope) (name:string) (loc:Local) = { scope with Locals = scope.Locals.Add(name, loc) }

let setAccessRead (scope:Scope) name = 
  let local = scope.Locals.[name]
  setLocal scope name (match local.ClosureAccess with
                       | Read | Write -> local
                       | Nothing -> { local with ClosureAccess = Read })

let setAccessWrite (scope:Scope) name =
  let local = scope.Locals.[name]
  setLocal scope name (match local.ClosureAccess with
                       | Write -> local
                       | Nothing | Read -> { local with ClosureAccess = Write })

let addUsedWithClosure leftName rightName (scopes:Scopes) =
  scopes := 
    match !scopes with
    | [] -> []
    | scope::xs ->
      let local = scope.Locals.[leftName]
      setLocal scope leftName { local with UsedWithClosure = local.UsedWithClosure.Add(rightName) } :: xs

let addUsedWith leftName rightName (scopes:Scopes) =
  scopes := 
    match !scopes with
    | [] -> []
    | scope::xs ->
      let local = scope.Locals.[leftName]
      setLocal scope leftName { local with UsedWith = local.UsedWith.Add(rightName) } :: xs

let addUsedAs name typ (scopes:Scopes) =
  scopes := 
    match !scopes with
    | [] -> []
    | scope::xs ->
      let local = scope.Locals.[name]
      setLocal scope name { local with UsedAs = local.UsedAs ||| typ } :: xs

let setNeedsArguments (scope:Scope) =
  if scope.Arguments 
    then scope
    else { scope with Arguments = true }

let createClosure (scope:Scope) name isLocalInParent = 
  if scope.Closure.ContainsKey name 
    then scope 
    else setClosure scope name { Index = scope.Closure.Count; IsLocalInParent = isLocalInParent }

let createLocal (scopes:Scopes) name =
  match !scopes with
  | [] -> ()
  | scope::xs -> scopes := setLocal scope name newLocal :: xs

let getVariable (scopes:Scopes) name =
  match !scopes with
  | [] -> Global(name)
  | scope::xs when name = "arguments" -> 
    scopes := (setNeedsArguments scope) :: xs
    Arguments

  | scope::xs when hasLocal scope name -> Local(name)
  | scope::xs when hasClosure scope name -> Closure(name)
  | _ -> 
    if List.exists (fun scope -> hasLocal scope name) !scopes then

      let rec updateScopes scopes =
        match scopes with
        | [] -> scopes
        | x::xsScopes ->
          if hasLocal x name 
            then setAccessRead x name :: xsScopes
            else createClosure x name (hasLocal xsScopes.Head name) :: updateScopes xsScopes

      scopes := updateScopes !scopes
      Closure(name)
    else
      Global(name)

let createScope (tree:AstTree) =
  let parms = [for c in (children (child tree 0)) -> c.Text]

  let rec doAdd (parms:string list) (locals:Map<string, Local>) (n:int) =
    match parms with
    | [] -> locals
    | name::xs -> (doAdd xs locals (n+1)).Add(name, { newLocal with ParamIndex = n });

  { newScope with Locals = (doAdd parms Map.empty 0) }