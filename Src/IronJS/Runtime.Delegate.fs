module IronJS.Runtime.Delegate

open IronJS
open IronJS.Utils

(**)
type Delegate<'a> when 'a :> System.Delegate (env, func:'a) =
  inherit Runtime.Core.Object(env)

  member self.Func = func

  interface System.Dynamic.IDynamicMetaObjectProvider with
    member self.GetMetaObject expr = new DelegateMeta<'a>(expr, self) :> MetaObj

(**)
and DelegateMeta<'a> when 'a :> System.Delegate (expr, jsDelegate:Delegate<'a>) =
  inherit Core.ObjectMeta(expr, jsDelegate)

let delegateTypeDef = typedefof<Delegate<_>>
let delegateTypeDefHashCode = delegateTypeDef.GetHashCode()
  
