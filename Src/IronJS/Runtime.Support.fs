namespace IronJS.Runtime

open IronJS
open IronJS.Aliases
open IronJS.Tools

open System.Dynamic
open System.Collections.Generic
open System.Runtime.InteropServices

type Undefined() =
  static let instance = new Undefined()
  static member Instance = instance
  static member InstanceExpr = Dlr.Expr.constant instance

type DelegateCell(astId:int, closureId:int, delegateType:ClrType) =
  let hashCode = 37 * (37 * astId + closureId) + delegateType.GetHashCode()

  member self.AstId = astId
  member self.ClosureId = closureId
  member self.DelegateType = delegateType

  override self.GetHashCode() = hashCode
  override self.Equals obj = 
    match obj with
    | :? DelegateCell as cell -> 
         self.AstId = cell.AstId
      && self.ClosureId = self.ClosureId
      && self.DelegateType = self.DelegateType
    | _ -> false