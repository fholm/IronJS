namespace IronJS.Native

open IronJS

module Utils =

  let mustBe (class':Class) (env:IjsEnv) (o:IjsObj) =
    if o.Class <> class' then
      let className = class' |> Classes.getName
      let error = sprintf "Object is not an instance of %s" className
      Api.Environment.raiseTypeError env error


