module IronJS.Compiler.Analyzer

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Ast.Types
open IronJS.Compiler
open IronJS.Compiler.Helpers.Core

(*Checks if a local always will result in a Dynamic type*)
let private isDynamic (loc:Local) =
  match loc.UsedAs with
  | JsTypes.Double 
  | JsTypes.String
  | JsTypes.Object -> true && loc.InitUndefined
  | _ -> true

(*Checks if a local variable never is assigned to from another variable*)
let private isNotAssignedTo var = var.UsedWith.Count = 0 && var.UsedWithClosure.Count = 0

(*Sets the Expr and UsedAs attributes of a variable*)
let private setType name var typ =
  let expr = Dlr.Expr.param name (match var.ClosureAccess with
                                 | Read | Write -> Constants.strongBoxTypeDef.MakeGenericType(ToClr typ)
                                 | Nothing -> ToClr typ)
  { var with UsedAs = typ; Expr = expr }

(*Get the type of a variable, evaluating it if necessary*)
let private getType name closureType (closure:ClosureMap) (vars:LocalMap) =

  let rec getType name (exclude:string Set) =
    let var = vars.[name]
    if exclude.Contains name then JsTypes.Nothing
    elif not(var.Expr = null) then var.UsedAs 
    else var.UsedWith
          |> Set.map  (fun var -> getType var (exclude.Add name))
          |> Set.fold (fun state typ -> state ||| typ) var.UsedAs
          |> (fun typ -> Set.fold (fun state var -> 
                                    state ||| ToJs (Helpers.Variable.Closure.clrTypeN closureType closure.[var].Index)
                                  ) typ var.UsedWithClosure
             ) //TODO: this is awfully indented, needs to be redone

  getType name Set.empty

(*Analyzes a scope *)
let analyze scope closureType (types:ClrType list) = 

  (*Resolves the type of a variable and updates the map with it*)
  let resolveType name (vars:LocalMap) =
    Map.add name (setType name vars.[name] (getType name closureType scope.Closure vars)) vars

  (*Resolves types of all local variables*)
  let rec resolveTypes locals = 
    match Map.tryFindKey (fun _ var -> var.Expr = null) locals with
    | None       -> locals // All variables have Exprs
    | Some(name) -> resolveTypes (resolveType name locals) // Key found, resolve its type

  { scope with 
      CallingConvention = 
        if types.Length > IronJS.Constants.maxTypedArgs 
          then CallingConvention.Dynamic 
          else CallingConvention.Static

      Locals =
        scope.Locals 
          |> Map.map (fun name var -> 
            if var.IsParameter then
              if var.ParamIndex < types.Length && types.Length < IronJS.Constants.maxTypedArgs
                then { var with UsedAs = var.UsedAs ||| ToJs types.[var.ParamIndex] } // We got an argument for this parameter
                else { setType name var JsTypes.Dynamic with InitUndefined = true; }  // We didn't, means make it dynamic and demote to a normal local
            else 
              if   isDynamic var       then setType name var JsTypes.Dynamic // No need to resolve type, force it here
              elif isNotAssignedTo var then setType name var var.UsedAs      // If it's not assigned from any variables
              else var // Needs to be resolved
            )

          |> resolveTypes
  }
