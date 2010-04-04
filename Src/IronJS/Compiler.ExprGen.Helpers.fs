module Compiler.ExprGen.Helpers

open IronJS
open IronJS.Utils
open IronJS.Runtime.Binders
open System.Dynamic

let dynamicInvoke target (args:Et list) =
  Et.Dynamic(
    (*binder*) new Invoke(new CallInfo(args.Length)),
    (*return type*) Constants.clrDynamic,
    (*target+args*) target :: args
  ) :> Et