namespace IronJS.Native

open IronJS

module RegExp =

  let private constructor' (f:FO) (this:CO) (pattern:BV) (options:BV) =
    let pattern = pattern |> TypeConverter.ToString
    let options = options |> TypeConverter.ToString
    f.Env.NewRegExp(pattern, options) |> BV.Box

  let private toString (f:FO) (this:CO) =
    let this = this.CastTo<RO>()
    let source = this.Get("source") |> TypeConverter.ToString
    let result = ref ("/" + source + "/")

    if this.Global then result := !result + "g"
    if this.MultiLine then result := !result + "m"
    if this.IgnoreCase then result := !result + "i"

    result |> BV.Box

  let private exec (f:FO) (this:CO) (input:string) =
    BV.Box input

  let private test (f:FO) (this:CO) (input:string) =
    (exec f this input).Clr = null |> BV.Box
  
  let internal createPrototype (env:Environment) prototype =
    let prototype = env.NewObject()
    prototype.Class <- Classes.Object
    prototype.Prototype <- prototype
    prototype

  let internal setupConstructor (env:Environment) =
    let ctor = new JsFunc<BV, BV>(constructor')
    let ctor = ctor |> Utils.createHostFunction env
    ctor.Put("prototype", env.Prototypes.RegExp, DescriptorAttrs.Immutable)
    env.Constructors <- {env.Constructors with RegExp=ctor}

  let internal setupPrototype (env:Environment) =
    let proto = env.Prototypes.RegExp

    proto.Put("constructor", env.Constructors.RegExp)

    let toString = new JsFunc(toString)
    let toString = toString |> Utils.createHostFunction env
    proto.Put("toString", toString)

    let exec = new JsFunc<string>(exec)
    let exec = exec |> Utils.createHostFunction env
    proto.Put("exec", exec)

    let test = new JsFunc<string>(test)
    let test = test |> Utils.createHostFunction env
    proto.Put("test", test)
