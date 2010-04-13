namespace IronJS.Runtime.Helpers

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Runtime

type Globals =
  static member GetMi = typeof<Globals>.GetMethod("Get")
  static member Get(name:string) = new obj()
  static member SetMi = typeof<Globals>.GetMethod("Set")
  static member Set(name:string, value:obj) = value

module Core =
  
  let isObject (typ:ClrType) = 
    typ = Runtime.Object.TypeDef || typ.IsSubclassOf(Runtime.Object.TypeDef)