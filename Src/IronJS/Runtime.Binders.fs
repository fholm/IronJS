module IronJS.Runtime.Binders

//Imports
open IronJS
open IronJS.Utils
open System.Dynamic

type Invoke(callInfo) =
  inherit InvokeBinder(callInfo)

  override self.FallbackInvoke(target, args, error) = 
    failwith "Not implemented"