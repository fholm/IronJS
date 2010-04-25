module IronJS.Runtime.Binders

//Imports
open IronJS
open IronJS.Aliases
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

type SetMember(name, ignoreCase) =
  inherit SetMemberBinder(name, ignoreCase)
  override x.FallbackSetMember(target, value, error) =
    failwith "SetMember.FallbackSetMember not implemented"

type GetMember(name, ignoreCase) =
  inherit GetMemberBinder(name, ignoreCase)
  override x.FallbackGetMember(target, error) =
    failwith "GetMember.FallbackGetMember not implemented"
