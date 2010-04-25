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
  | Ast.JsTypes.Integer
  | Ast.JsTypes.Function
  | Ast.JsTypes.Object -> true && loc.InitUndefined
  | _ -> true

(*Checks if a local variable never is assigned to from another variable*)
let private isNotAssignedTo (var:Ast.Local) = 
     var.UsedWith.Count = 0 
  && var.UsedWithClosure.Count = 0 
  && var.AssignedFrom.Length = 0

(*Sets the Expr and UsedAs attributes of a variable*)
let private setType name (var:Ast.Local) typ =
  let exprType = if var.ClosedOver 
                   then Constants.strongBoxTypeDef.MakeGenericType(Utils.Type.jsToClr typ) 
                   else Utils.Type.jsToClr typ

  let expr = Dlr.Expr.param name exprType

  {Ast.Utils.setLocalFlag Ast.LocalFlags.TypeResolved var with UsedAs = typ }

(*Get the type of a variable, evaluating it if necessary*)
let getType name closureType (closure:Ast.ClosureMap) (vars:Ast.LocalMap) =

  let excluded = ref Set.empty

  let rec getExprType' expr = 
    match expr with
    | Ast.BinaryOp(left, op, right) -> 
      match op with
      | Ast.Add -> getExprType' left ||| getExprType' right
      | _ -> failwith "not supported"
    | Ast.Local(name, _) -> getLocalType' name
    | _ -> failwith "not supported"

  and getLocalType' name =
    let var = vars.[name]

    if (!excluded).Contains name then Ast.JsTypes.Nothing
    else  
      if var.TypeResolved then var.UsedAs 
      else  
        excluded := (!excluded).Add name

        let evaledWithClosures =
          Set.fold (fun state var -> 
                 state ||| Utils.Type.clrToJs (Variables.Closure.clrTypeN closureType closure.[var].Index)
               ) var.UsedAs var.UsedWithClosure

        // Combine UsedAs + UsedWithClosure types with UsedWith types
        let evaledWithVariables = 
          var.UsedWith 
            |> Set.map  (fun var -> getLocalType' var)
            |> Set.fold (fun state typ -> state ||| typ) evaledWithClosures

        // Eval any expression values we're assigned to from
        let result = 
          var.AssignedFrom 
            |> Seq.ofList
            |> Seq.map  (fun expr -> getExprType' expr)
            |> Seq.fold (fun state typ -> state ||| typ) evaledWithVariables

        excluded := (!excluded).Remove name
        result

  getLocalType' name

let private handleMissingArgument (name:string) (var:Ast.Local) =
  let removedParam = Ast.Utils.removeLocalFlag Ast.LocalFlags.Parameter var
  {Ast.Utils.setLocalFlag Ast.LocalFlags.InitToUndefined removedParam with UsedAs = var.UsedAs ||| Ast.JsTypes.Undefined}

(*Analyzes a scope*)
let analyze (scope:Ast.Scope) closureType (types:ClrType list) = 

  (*Resolves the type of a variable and updates the map with it*)
  let resolveType name (vars:Ast.LocalMap) =
    Map.add name (setType name vars.[name] (getType name closureType scope.Closure vars)) vars

  (*Resolves types of all local variables*)
  let rec resolveTypes locals = 
    match Map.tryFindKey (fun _ (var:Ast.Local) -> not (var.Flags.Contains Ast.LocalFlags.TypeResolved)) locals with
    | None       -> locals // All variables have Exprs
    | Some(name) -> resolveTypes (resolveType name locals) // Key found, resolve its type

  { scope with 
      ArgTypes = Array.ofList types

      Locals =
        scope.Locals 
          |> Map.map (fun name var -> 
            if var.IsParameter then
              if var.Index < types.Length
                then {var with UsedAs = var.UsedAs ||| Utils.Type.clrToJs types.[var.Index]} // We got an argument for this parameter
                else handleMissingArgument name var // We didn't, means make it dynamic
            else 
              if   isDynamic var       then setType name var Ast.JsTypes.Dynamic // No need to resolve type, force it here
              elif isNotAssignedTo var then setType name var var.UsedAs      // If it's not assigned to from any variables
              else var // Needs to be resolved
            )

          |> resolveTypes
  }
