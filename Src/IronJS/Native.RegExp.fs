namespace IronJS.Native

// Removes the warning for using 
// "constructor" as an identifier
#nowarn "46"

open IronJS

module RegExp =

  open FSharpOperators

  let private constructor' (f:FO) (o:CO) (pattern:BV) (options:BV) =
    let pattern = pattern |> TC.ToString
    let options = 
      match options.Tag with
      | TypeTags.Undefined -> ""
      | _ when options.IsNull -> ""
      | _ -> options |> TC.ToString

    f.Env.NewRegExp(pattern, options) |> BV.Box

  let private toString (f:FO) (this:CO) =
    let this = this.CastTo<RO>()
    let source = this.Get("source") |> TC.ToString
    let result = ref ("/" + source + "/")

    if this.Global then result := !result + "g"
    if this.MultiLine then result := !result + "m"
    if this.IgnoreCase then result := !result + "i"

    !result |> BV.Box

  let internal exec (f:FO) (this:CO) (input:string) : BV =
    let ro = this.CastTo<RO>()
    let matches = ro.RegExp.Matches(input)
    match matches.Count with
    | 0 -> BoxedConstants.Null
    | _ ->
      if ro.Global then
        let a = f.Env.NewArray(matches.Count |> uint32)
        for i = 0 to (matches.Count-1) do
          a.Put(uint32 i, matches.[i].Value)

        a |> BV.Box

      else
        let groups = matches.[0].Groups
        let a = f.Env.NewArray(groups.Count |> uint32)
        for i = 0 to (groups.Count-1) do
          a.Put(uint32 i, groups.[i].Value)

        a |> BV.Box

  let private test (f:FO) (this:CO) (input:string) =
    (exec f this input).Clr = null |> BV.Box
  
  let internal createPrototype (env:Environment) objPrototype =
    let prototype = env.NewObject()
    prototype.Prototype <- objPrototype
    prototype

  let internal setupConstructor (env:Environment) =
    let ctor = new JsFunc<BV, BV>(constructor')
    let ctor = ctor |> Utils.createHostFunction env

    ctor?prototype <- env.Prototypes.RegExp
    ctor.ConstructorMode <- ConstructorModes.Host

    env.Constructors <- {env.Constructors with RegExp=ctor}
    env.Globals?RegExp <- ctor

  let internal setupPrototype (env:Environment) =
    let create func = func |> Utils.createHostFunction env
    let proto = env.Prototypes.RegExp

    proto?constructor <- env.Constructors.RegExp
    proto?toString <- (JsFunc(toString) |> create)
    proto?exec <- (JsFunc<string>(exec) |> create)
    proto?test <- (JsFunc<string>(test) |> create)
