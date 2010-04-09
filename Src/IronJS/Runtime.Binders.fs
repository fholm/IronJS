module IronJS.Runtime.Binders

//Imports
open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Runtime
open System.Dynamic

type Invoke(callInfo) =
  inherit InvokeBinder(callInfo)

  override self.FallbackInvoke(target, args, error) = 
    failwith "Invoke.FallbackInvoke not implemented"

type Convert(typ, explicit) =
  inherit ConvertBinder(typ, explicit)

  override x.FallbackConvert(target, error) = 
    failwith "Convert.FallbackConvert not implemented"