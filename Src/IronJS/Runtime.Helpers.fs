namespace IronJS.Runtime.Helpers

open IronJS
open IronJS.Utils
open IronJS.Tools

module Core =
  
  let isObject (typ:ClrType) = 
    typ.IsSubclassOf(Runtime.Core.objectTypeDef) || typ = Runtime.Core.objectTypeDef
