module IronJS.Binders

//Imports
open IronJS
open IronJS.Utils
open System.Dynamic

//Aliases
type MetaObj = System.Dynamic.DynamicMetaObject

type Invoke(ci) =
  inherit InvokeBinder(ci)

  override self.FallbackInvoke(target, args, error) = 
    failwith "Not implemented"

let dynamicInvoke target (args:Et list) =
  Et.Dynamic(
    (*binder*) new Invoke(new CallInfo(args.Length)),
    (*return type*) ClrTypes.Dynamic,
    (*target+args*) target :: args
  ) :> Et