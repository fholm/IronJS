module IronJS.Ast.Helpers

open IronJS
open IronJS.Tools
open IronJS.Utils
open IronJS.Monads
open IronJS.Ast.Types
open IronJS.CSharp.Parser
open Antlr.Runtime
open Antlr.Runtime.Tree

let ct (tree:obj) = tree :?> AstTree
let child (tree:AstTree) index = (ct tree.Children.[index])
let children (tree:AstTree) = toList<AstTree> tree.Children
let childrenOf (tree:AstTree) n = children (child tree n)
let isAssign (tree:AstTree) = tree.Type = ES3Parser.ASSIGN
let isAnonymous (tree:AstTree) = tree.Type = ES3Parser.FUNCTION && tree.ChildCount = 2
let setClosure (scope:Scope) (name:string) (clos:Closure) = { scope with Closure = scope.Closure.Add(name, clos) }
let cleanString = function | null | "" -> "" | s  -> if s.[0] = '"' then s.Trim('"') else s.Trim('\'')
let hasClosure (scope:Scope) name = scope.Closure.ContainsKey name
let hasLocal (scope:Scope) name = scope.Locals.ContainsKey name
let setLocal (scope:Scope) (name:string) (loc:Local) = { scope with Locals = scope.Locals.Add(name, loc) }

let createClosure (scope:Scope) name isLocalInParent = 
  if scope.Closure.ContainsKey name 
    then scope 
    else setClosure scope name { Index = scope.Closure.Count; IsLocalInParent = isLocalInParent }

let createScope (tree:AstTree) =
  let rec createLocals parms index =
    match parms with
    | []       -> Map.empty
    | name::xs -> Map.add name { newLocal with ParamIndex = index } (createLocals xs (index+1))

  { newScope with Locals = createLocals [for c in (childrenOf tree 0) -> c.Text] 0 }

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
let setNeedsArguments (scope:Scope) =
  if scope.Arguments 
    then scope
    else { scope with Arguments = true }

let getVariable name = state {
  let! s = getState
  match s with
  | [] -> return Global(name)
  | x::xs when hasLocal x name -> return Local(name)
  | x::xs when hasClosure x name -> return Closure(name)
  | _ -> 
    if List.exists (fun s -> hasLocal s name) s then

      let rec updateScopes s =
        match s with
        | []    ->  s
        | x::xs ->  if hasLocal x name 
                      then setAccessRead x name :: xs
                      else createClosure x name (hasLocal xs.Head name) :: updateScopes xs

      do! setState (updateScopes s)

      return Closure(name)
    else
      return Global(name)}

let createLocal name = state {
  let! s = getState
  match s with
  | []    -> ()
  | x::xs -> do! setState (setLocal x name newLocal :: xs) }  

let enterScope t = state {
  let! s = getState
  do! setState (createScope t :: s)}

let exitScope() = state {
  let! s = getState
  match s with
  | x::xs -> do! setState xs
             return x
  | _     -> return failwith "Couldn't exit scope"}

let usedAs name typ = state {
  let! s = getState
  match s with
  | []    -> failwith "Global scope"
  | x::xs -> let l  = x.Locals.[name]
             let x' = setLocal x name { l with UsedAs = l.UsedAs ||| typ }
             do! setState(x'::xs)}

let usedWith name rname = state {
  let! s = getState
  match s with
  | []    -> failwith "Global scope"
  | x::xs -> let l  = x.Locals.[name]
             let x' = setLocal x name { l with UsedWith = l.UsedWith.Add(rname) }
             do! setState(x'::xs)}

let usedWithClosure name rname = state {
  let! s = getState
  match s with
  | []    -> failwith "Global scope"
  | x::xs -> let l  = x.Locals.[name]
             let x' = setLocal x name { l with UsedWithClosure = l.UsedWithClosure.Add(rname) }
             do! setState(x'::xs)}