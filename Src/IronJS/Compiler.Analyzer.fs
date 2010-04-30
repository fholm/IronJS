module IronJS.Compiler.Analyzer

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Compiler

(*Checks if a local always will result in a Dynamic type*)
let private isDynamic (l:Ast.Types.Variable) =
  match l.UsedAs with
  | Types.Double 
  | Types.Integer
  | Types.Boolean
  | Types.String
  | Types.Undefined
  | Types.Array
  | Types.Function
  | Types.Object -> false
  | _ -> true

(*Checks if a local variable never is assigned to from another variable*)
let private isNotAssignedTo (var:Ast.Types.Variable) = 
     var.UsedWith.Count = 0 
  && var.UsedWithClosure.Count = 0 
  && var.AssignedFrom.Length = 0

(*Sets the Expr and UsedAs attributes of a variable*)
let private setType name (l:Ast.Types.Variable) typ =
  let exprType = if Ast.Variable.isClosedOver l
                   then Type.strongBoxType.MakeGenericType(Utils.Type.jsToClr typ) 
                   else Utils.Type.jsToClr typ

  let expr = Dlr.Expr.param name exprType

  {Ast.Variable.setFlag Ast.Flags.Variable.TypeResolved l with UsedAs = typ }

(*Get the type of a variable, evaluating it if necessary*)
let private getType name closureType (closure:Ast.Types.ClosureMap) (vars:Ast.Types.LocalMap) =

  let excluded = ref Set.empty

  let rec getExprType' expr = 
    match expr with
    | Ast.BinaryOp(op, left, right) -> 
      match op with
      | Ast.Add -> getExprType' left ||| getExprType' right
      | _ -> failwith "not supported"
    | Ast.Variable(name, _) -> getLocalType' name
    | Ast.Invoke(_, _) -> Types.Dynamic
    | _ -> failwith "not supported"

  and getLocalType' name =
    let var = vars.[name]

    if (!excluded).Contains name then Types.Nothing
    else  
      if Ast.Variable.typeIsResolved var then var.UsedAs 
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

let private handleMissingArgument (name:string) (var:Ast.Types.Variable) =
  let removedParam = Ast.Variable.delFlag Ast.Flags.Variable.Parameter var
  let removedParam = Ast.Variable.delFlag Ast.Flags.Variable.NeedProxy removedParam
  {Ast.Variable.setFlag Ast.Flags.Variable.InitToUndefined removedParam with UsedAs = var.UsedAs ||| Types.Undefined}

(*Analyzes a scope*)
let analyze (scope:Ast.Types.Scope) closureType (types:ClrType list) = 

  (*Resolves the type of a variable and updates the map with it*)
  let resolveType name (vars:Ast.Types.LocalMap) =
    Map.add name (setType name vars.[name] (getType name closureType scope.Closures vars)) vars

  (*Resolves types of all local variables*)
  let rec resolveTypes locals = 
    match Map.tryFindKey (fun _ (var:Ast.Types.Variable) -> not (var.Flags.Contains Ast.Flags.Variable.TypeResolved)) locals with
    | None       -> locals // All variables have Exprs
    | Some(name) -> resolveTypes (resolveType name locals) // Key found, resolve its type

  { scope with 
      ArgTypes = Array.ofList types

      Variables =
        scope.Variables 
          |> Map.map (fun name l -> 
            if Ast.Variable.isParameter l then
              if l.Index < types.Length
                then {l with UsedAs = l.UsedAs ||| Utils.Type.clrToJs types.[l.Index]} // We got an argument for this parameter
                else handleMissingArgument name l // We didn't, means make it dynamic
            else 
              if   isDynamic l       then setType name l Types.Dynamic // No need to resolve type, force it here
              elif isNotAssignedTo l then setType name l l.UsedAs      // If it's not assigned to from any variables
              else l // Needs to be resolved
            )

          |> resolveTypes
  }
