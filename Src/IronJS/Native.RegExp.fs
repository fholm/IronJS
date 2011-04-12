namespace IronJS.Native

// Removes the warning for using 
// "constructor" as an identifier
#nowarn "46"

open IronJS
open IronJS.DescriptorAttrs
open IronJS.Support.CustomOperators

module internal RegExp =

  /// These steps are outlined in the ECMA-262, Section 15.10.4.1
  let private constructor' (f:FO) (o:CO) (pattern:BV) (flags:BV) =
    if pattern.IsRegExp then

      if flags.IsUndefined then
        pattern

      else
        f.Env.RaiseTypeError("When specifying a RegExp as the first argument to the RegExp constructor, it is invalid to specify flags.")

    else
      let p = if pattern.IsUndefined then "" else TC.ToString(pattern)
      let fl = if flags.IsUndefined then "" else TC.ToString(flags)

      f.Env.NewRegExp(p, fl) |> BV.Box

  ///
  let setup (env:Env) =
    let ctor = Function<BV, BV>(constructor') $ Utils.createConstructor env (Some 2)
    ctor.Put("prototype", env.Prototypes.RegExp, Immutable)
    ctor.MetaData.Name <- "RegExp"

    env.Constructors <- {env.Constructors with RegExp=ctor}
    env.Globals.Put("RegExp", ctor, DontEnum)

  ///
  module Prototype =

    ///
    let private toString (f:FO) (this:CO) =
      let this = this.CastTo<RO>()
      let source = this.Get("source") |> TC.ToString
      let result = ref ("/" + source + "/")

      if this.Global then result := !result + "g"
      if this.MultiLine then result := !result + "m"
      if this.IgnoreCase then result := !result + "i"

      !result |> BV.Box

    /// These steps are outlined in the ECMA-262, Section 15.10.6.2
    let internal exec (f:FO) (this:CO) (input:BV) : BV =
      
      // Step 1
      let R = this.CastTo<RO>()

      // Step 2
      let S = TC.ToString(input)

      // Step 3
      let length = S.Length

      // Step 4
      let lastIndex = R.Get("lastIndex")

      // Step 5
      let mutable i = TC.ToInteger(lastIndex)

      // Step 6
      let global' = R.Global  // INFO: The method of retrieving this value differs from the spec.

      // Step 7
      if not global' then i <- 0

      // Step 8 is not needed
      // Step 9, using .NET's implementation.
      if i < 0 || i > length then
        R.Put("lastIndex", 0.0)
        Environment.BoxedNull

      else
        let r = R.RegExp.Match(S, i)
        if not r.Success then
          R.Put("lastIndex", 0.0)
          Environment.BoxedNull

        else
          // Step 10
          let e = r.Index + r.Length

          // Step 11
          if global' then R.Put("lastIndex", e |> BV.Box)

          // Step 12
          // We subtract 1, because .NET's implementation 
          // adds the whole string as capture group #0.
          let n = r.Groups.Count - 1  

          // Step 13
          let A = f.Env.NewArray()

          // Step 14
          let matchIndex = r.Index

          // Steps 15, 16, & 17
          A.Put("index", matchIndex |> BV.Box)
          A.Put("input", S |> BV.Box)
          A.Put("length", (n+1) |> BV.Box)

          // Steps 18 & 19
          A.Put(uint32 0, r.Value |> BV.Box)

          // Step 20
          // We use a starting index of 1, even though .NET is 0-based, 
          // because the zeroth item is the whole capture.
          for i = 1 to n do  
            let g = r.Groups.[i]
            A.Put(uint32 i, if g.Success then g.Value |> BV.Box else Undefined.Boxed)

          // Step 21
          A |> BV.Box

    ///
    let private test (f:FO) (this:CO) (input:BV) =
      let result = (exec f this input).Clr 
      result <> null |> BV.Box
  
    ///
    let create (env:Env) objPrototype =
      let prototype = env.NewObject()
      prototype.Prototype <- objPrototype
      prototype

    ///
    let setup (env:Env) =
      let proto = env.Prototypes.RegExp

      //
      proto.Put("constructor", env.Constructors.RegExp, DontEnum)

      //
      let toString = Function(toString) $ Utils.createFunction env (Some 0)
      proto.Put("toString", toString, DontEnum)

      //
      let exec = Function<BV>(exec)  $ Utils.createFunction env (Some 1)
      proto.Put("exec", exec, DontEnum)

      //
      let test = Function<BV>(test)  $ Utils.createFunction env (Some 1)
      proto.Put("test", test, DontEnum)
