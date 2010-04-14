namespace IronJS.Runtime.Helpers

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Runtime

module Core = 

  let isObject (typ:ClrType) = 
    typ = Runtime.Object.TypeDef || typ.IsSubclassOf(Runtime.Object.TypeDef)