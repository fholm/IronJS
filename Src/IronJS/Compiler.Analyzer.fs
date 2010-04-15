module IronJS.Compiler.Analyzer

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Compiler

(*Checks if a local always will result in a Dynamic type*)
let private isDynamic (loc:Ast.Local) =
  match loc.UsedAs with
  | Ast.JsTypes.Double 
  | Ast.JsTypes.String
  | Ast.JsTypes.Object -> true && loc.InitUndefined
  | _ -> true

(*Checks if a local variable never is assigned to from another variable*)
let private isNotAssignedTo (var:Ast.Local) = var.UsedWith.Count = 0 && var.UsedWithClosure.Count = 0

(*Sets the Expr and UsedAs attributes of a variable*)
let private setType name (var:Ast.Local) typ =
  let expr = Dlr.Expr.param name (match var.ClosureAccess with
                                  | Ast.Read | Ast.Write -> Constants.strongBoxTypeDef.MakeGenericType(Utils.Type.ToClr typ)
                                  | Ast.Nothing -> Utils.Type.ToClr typ)

  { var with UsedAs = typ; Expr = expr }

(*Get the type of a variable, evaluating it if necessary*)
let private getType name closureType (closure:Ast.ClosureMap) (vars:Ast.LocalMap) =

  let rec getType' name (exclude:string Set) =
    let var = vars.[name]

    if  exclude.Contains name 
      then  Ast.JsTypes.Nothing
      else  if not(var.Expr = null) 
              then  var.UsedAs 
              else  let evaledWithClosures =
                      Set.fold (fun state var -> 
                             state ||| Utils.Type.ToJs (Utils.Variable.Closure.clrTypeN closureType closure.[var].Index)
                           ) var.UsedAs var.UsedWithClosure

                    // Combine UsedAs + UsedWithClosure types with UsedWith types
                    var.UsedWith
                      |> Set.map  (fun var -> getType' var (exclude.Add name))
                      |> Set.fold (fun state typ -> state ||| typ) evaledWithClosures

  getType' name Set.empty

(*Analyzes a scope*)
let analyze (scope:Ast.Scope) closureType (types:ClrType list) = 
  (*Resolves the type of a variable and updates the map with it*)
  let resolveType name (vars:Ast.LocalMap) =
    Map.add name (setType name vars.[name] (getType name closureType scope.Closure vars)) vars

  (*Resolves types of all local variables*)
  let rec resolveTypes locals = 
    match Map.tryFindKey (fun _ (var:Ast.Local) -> var.Expr = null) locals with
    | None       -> locals // All variables have Exprs
    | Some(name) -> resolveTypes (resolveType name locals) // Key found, resolve its type

  { scope with 
      Locals =
        scope.Locals 
          |> Map.map (fun name var -> 
            if var.IsParameter then
              if var.ParamIndex < types.Length
                then { var with UsedAs = var.UsedAs ||| Utils.Type.ToJs types.[var.ParamIndex] } // We got an argument for this parameter
                else setType name var Ast.JsTypes.Dynamic // We didn't, means make it dynamic
            else 
              if   isDynamic var       then setType name var Ast.JsTypes.Dynamic // No need to resolve type, force it here
              elif isNotAssignedTo var then setType name var var.UsedAs      // If it's not assigned to from any variables
              else var // Needs to be resolved
            )

          |> resolveTypes
  }
