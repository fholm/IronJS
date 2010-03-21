module Binders

//Imports
open System.Dynamic

//Aliases
type MetaObj = System.Dynamic.DynamicMetaObject

type Invoke(ci) =
  inherit InvokeBinder(ci)

  override self.FallbackInvoke(target, args, error) = 
    failwith "Not implemented"