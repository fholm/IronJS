module IronJS.Compiler.Analyzer

open IronJS
open IronJS.Utils
open IronJS.Types
open IronJS.Ast.Types

let willBeDynamic (loc:Local) =
  match loc.UsedAs with
  | JsTypes.Double 
  | JsTypes.String
  | JsTypes.Object -> true && loc.InitUndefined
  | _ -> true

let makeWithExpr (name:string) (loc:Local) (typ:JsTypes) =
  let expr = EtTools.param name (match loc.ClosureAccess with
                                 | Read | Write -> StrongBoxType.MakeGenericType(ToClr typ)
                                 | None -> ToClr typ)
  { loc with UsedAs = typ; Expr = expr }
  

let demoteParameter name (loc:Local) =
  { makeWithExpr name loc JsTypes.Dynamic with ParamIndex = -1; InitUndefined = true; }

let rec resolveType name (exclude:string Set) (locals:Map<string,Local> ref) =
  (!locals).[name].UsedWith
    |> Set.map (fun var -> getType var exclude locals)
    |> Set.fold (fun typ state -> typ ||| state) (!locals).[name].UsedAs

and getType name (exclude:string Set) (locals:Map<string,Local> ref) =
  let local = (!locals).[name]
  if exclude.Contains name then JsTypes.None
  else
    if not(local.Expr = null) then local.UsedAs 
    else
      let typ = (resolveType name (exclude.Add name) locals)
      locals := Map.add name (makeWithExpr name local typ) !locals
      typ

let analyze (scope:Scope) (types:ClrType list) = 
    scope.Locals 
      |> Map.map (fun name var -> 
        if var.IsParameter then
          if var.ParamIndex < types.Length 
            then { var with UsedAs = var.UsedAs ||| ToJs types.[var.ParamIndex] } // We got an argument for this parameter
            else demoteParameter name var // We didn't, means make it dynamic and demote to a normal local
        else 
          if willBeDynamic var
            then makeWithExpr name var JsTypes.Dynamic // No need to resolve type, force it here
            elif var.UsedWith.Count = 0 
              then makeWithExpr name var var.UsedAs // If it's not assigned from any variables
              else var // Needs to be resolved
        )
      |> ref
      |> (fun locals -> 
        fix0 (fun next -> 
          match Map.tryFindKey (fun _ v -> v.Expr = null) !locals with
          | Option.None -> !locals
          | Option.Some(name) -> 
            let typ = (resolveType name Set.empty locals)
            let local = makeWithExpr name (!locals).[name] typ
            locals := (!locals).Add(name, local)
            next()
        )
      )