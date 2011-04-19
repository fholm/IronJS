namespace IronJS.Native

open System
open System.Text.RegularExpressions

open IronJS
open IronJS.Support.Aliases
open IronJS.Support.CustomOperators
open IronJS.DescriptorAttrs

///
module internal String =

  ///
  let private constructor' (ctor:FO) (this:CO) (args:Args) =
    let value = 
      match args.Length with
      | 0 -> ""
      | _ -> args.[0] |> TC.ToString

    match this with
    | null -> ctor.Env.NewString(value) |> BV.Box
    | _ -> value |> BV.Box
    
  ///
  let private fromCharCode (args:Args) =
    let buffer = Text.StringBuilder(args.Length)

    for i = 0 to args.Length-1 do
      buffer.Append(TC.ToUInt16 args.[i] |> char) |> ignore

    buffer.ToString()
    
  ///
  let setup (env:Env) =
    //
    let ctor = VariadicFunction(constructor') 
    let ctor = ctor $ Utils.createConstructor env (Some 1)

    //
    ctor.MetaData.Name <- "String"

    //
    let fromCharCode = new Func<Args, string>(fromCharCode) $ Utils.createFunction env (Some 1)
    ctor.Put("fromCharCode", fromCharCode, DontEnum)

    //
    ctor.Put("prototype", env.Prototypes.String, Immutable)

    //
    env.Globals.Put("String", ctor, DontEnum)
    env.Constructors <- {env.Constructors with String=ctor}

  ///
  module Prototype = 

    ///
    let private toString (func:FO) (this:CO) =
      this.CheckType<SO>()
      this |> ValueObject.GetValue
    
    ///
    let private valueOf (func:FO) (this:CO) = 
      toString func this
    
    /// These steps are outlined in the ECMA-262, Section 15.5.4.4
    let private charAt (_:FO) (this:CO) (pos:double) =
      let S = this |> TC.ToString
      let position = pos |> TC.ToInteger
      let size = S.Length
      if position < 0 || position >= size then "" else S.[position] |> string

    /// These steps are outlined in the ECMA-262, Section 15.5.4.5
    let private charCodeAt (_:FO) (this:CO) (pos:double) =
      let S = this |> TC.ToString
      let position = pos |> TC.ToInteger
      let size = S.Length
      if position < 0 || position >= size then nan else S.[position] |> double

    ///
    let private concat (_:FO) (this:CO) (args:Args) =
      let buffer = new Text.StringBuilder(TC.ToString(this))
      let toString (x:BoxedValue) = buffer.Append(TC.ToString(x))
      args |> Array.iter (toString >> ignore)
      buffer.ToString()

    ///
    let private indexOf (_:FO) (this:CO) (subString:string) (index:double) =
      let value = this |> TC.ToString
      let index = index |> TC.ToInt32
      let index = Math.Min(Math.Max(index, 0), value.Length);
      value.IndexOf(subString, index, StringComparison.Ordinal) |> double

    /// These steps are outlined in the ECMA-262, Section 15.5.4.8
    let private lastIndexOf (_:FO) (this:CO) (searchString:BV) (position:BV) =
      let S = this |> TC.ToString
      let searchStr = searchString |> TC.ToString
      let numPos = position |> TC.ToNumber
      let pos =
        if numPos <> numPos
          then Int32.MaxValue
          else numPos |> TC.ToInteger
      let len = S.Length
      let start = Math.Min(Math.Max(pos, 0), len)
      let searchLen = searchStr.Length
      let mutable k = start
      let mutable found = false
      while k >= 0 && not found do
        found <- true
        let mutable j = 0
        while found && j < searchLen do
          if k+searchLen > len || (S.[k+j] <> searchStr.[j]) then
            found <- false
          else
            j <- j + 1
        if not found then
          k <- k - 1
      k |> double

    ///
    let private localeCompare (_:FO) (this:CO) (that:string) =
      let this = TC.ToString(this)
      String.Compare(this, that) $ double

    let private toRegExp (env:Env) (regexp:BV) =
      if regexp.IsRegExp then
        regexp.Object.CastTo<RO>()
      else
        let S = if regexp.IsUndefined then "" else regexp |> TC.ToString
        env.NewRegExp(S) :?> RO

    let private match' (f:FO) (this:CO) (regexp:BV) =
      let S = this |> TC.ToString
      let rx = regexp |> toRegExp f.Env
      let global' = rx.Global
      let exec = RegExp.Prototype.exec
      if not global' then
        exec f rx (S |> BV.Box)
      else
        rx.Put("lastIndex", 0 |> BV.Box)
        let A = f.Env.NewArray()
        let mutable previousLastIndex = 0
        let mutable n = 0
        let mutable lastMatch = true
        while lastMatch do
          let result = exec f rx (S |> BV.Box)
          if result.IsNull then lastMatch <- false
          else
            let thisIndex = rx.Get("lastIndex") |> TC.ToInt32
            if thisIndex = previousLastIndex then
              previousLastIndex <- thisIndex + 1
              rx.Put("lastIndex", previousLastIndex |> BV.Box)
            else
              previousLastIndex <- thisIndex
            let matchStr = result.Array.Get(0).String
            A.Put(uint32 n, matchStr)
            n <- n + 1
        if n = 0 then
          Environment.BoxedNull
        else
          A |> BV.Box

    let private replaceTokens =
      new Regex(@"[^$]+|\$\$|\$&|\$`|\$'|\$\d\d|\$\d|\$", RegexOptions.Compiled)

    let private evaluateReplacement (matched:string) (before:unit->string) (after:unit->string) (replacement:string) (groups:GroupCollection) =
      if replacement.Contains("$") then

        let tokens : seq<Capture> = replaceTokens.Matches replacement |> Seq.cast
        let tokens = tokens |> Seq.map (fun m -> m.Value)
        Seq.fold (fun (s:string) (t:string) ->
          let r =
            match t with
            | _ when not (t.StartsWith("$")) -> t
            | "$$" -> "$"
            | "$0" -> "$0"
            | "$00" -> "$00"
            | "$&" -> matched
            | "$`" -> before()
            | "$'" -> after()
            | _ ->
              let subPatternIndex = t.Substring 1 |> int
              if groups <> null && subPatternIndex < groups.Count
                then groups.[subPatternIndex].Value
                else ""
          s + r) "" tokens

      else
        replacement

    let private memoize f =
      let cache = ref null
      fun () ->
        if !cache |> FSharp.Utils.notNull then !cache
        else
          let value = f()
          cache := value
          value

    ///
    let private replace (_:FO) (this:CO) (search:BV) (replace:BV) =
      let value = this |> TC.ToString

      //replace(regex, _)
      if search.IsRegExp then 
        let search = search.Object.CastTo<RO>()
        let count = if search.Global then Int32.MaxValue else 1

        //replace(regex, function)
        if replace.IsFunction then

          let matchEval (m:Match) =
            if search.Global then
              search.Put("lastIndex", m.Index + 1 |> double)

            let params' = MutableList<BV>()

            for g in m.Groups do
              if g.Success 
                then params'.Add(g.Value |> BV.Box)
                else params'.Add(Undefined.Boxed)

            let args = params'.ToArray()
            let this = this.Env.Globals

            replace.Func.Call(this, args) |> TC.ToString
        
          //Run regex on our input, using matchEval for replacement
          let result = search.RegExp.Replace(value, MatchEvaluator matchEval, count)
          if search.Global then search.Put("lastIndex", 0.0)
          result

        //replace(regex, string)
        else
          let replace = replace |> TC.ToString

          let matchEval (m:Match) =
            let before = memoize (fun () -> value.Substring(0, m.Index))
            let after = memoize (fun () -> value.Substring(Math.Min(value.Length - 1, m.Index + m.Length)))
            evaluateReplacement m.Value before after replace m.Groups

          let result = search.RegExp.Replace(value, MatchEvaluator matchEval, count)
          if search.Global then search.Put("lastIndex", 0.0)
          result
      
      //replace(string, _)
      else
        let search = search |> TC.ToString
        let index = value.IndexOf search

        if index > -1 then
      
          //replace(string, function)
          if replace.IsFunction then 
            let replace = replace.Func.Call(this.Env.Globals, search, double index, value) |> TC.ToString
            value.Substring(0, index) + replace + value.Substring(index + search.Length)

          //replace(string, string)
          else
            let before = value.Substring(0, index)
            let after = value.Substring(index + search.Length)
            let replace = replace |> TC.ToString
            let replace = evaluateReplacement search (fun () -> before) (fun () -> after) replace null
            before + replace + after

        else
          value
          
    ///
    let private search (_:FO) (this:CO) (search:BV) =
      let value = this |> TC.ToString

      //search(regex)
      if search.Tag >= TypeTags.Object then 
        let regexp = search |> toRegExp this.Env
        let m = regexp.RegExp.Match(value)
        if m |> FSharp.Utils.notNull && m.Success 
          then m.Index |> double
          else 0.0
      
      //search(string)
      else
        let search = search |> TC.ToString
        value.IndexOf(search, StringComparison.Ordinal) |> double

    /// These steps are outlined in the ECMA-262, Section 15.5.4.13
    let private slice (_:FO) (this:CO) (start:double) (end':BoxedValue) =
      let S = this |> TC.ToString
      let len = S.Length
      let intStart = start |> TC.ToInteger
      let intEnd = if end'.IsUndefined then len else end' |> TC.ToInteger
      let from = if intStart < 0 then Math.Max(len + intStart, 0) else Math.Min(intStart, len)
      let to' = if intEnd < 0 then Math.Max(len + intEnd, 0) else Math.Min(intEnd, len)
      let span = Math.Max(to' - from, 0)
      S.Substring(from, span)

    ///
    let private split (f:FO) (this:CO) (separator:BV) (limit:BV) =
      let value = this |> TC.ToString
    
      let limit =
        if limit.IsUndefined
          then Int32.MaxValue 
          else limit |> TC.ToInt32

      let parts = 
        if separator.IsRegExp then
          let separator = separator.Object.CastTo<RO>()
          separator.RegExp.Split(value) |> Seq.truncate limit |> Seq.toArray

        else
          let separator =
            if separator.IsUndefined
              then "" 
              else separator |> TC.ToString

          value.Split([|separator|], StringSplitOptions.None) |> Seq.truncate limit |> Seq.toArray

      let array = f.Env.NewArray(parts.Length |> uint32)
      for i = 0 to parts.Length-1 do
        array.Put(uint32 i, parts.[i])

      array

    /// These steps are outlined in the ECMA-262, Section 15.5.4.15
    let private substring (_:FO) (this:CO) (start:double) (end':BV) =
      let S = this |> TC.ToString
      let len = S.Length
      let intStart = start |> TC.ToInteger
      let intEnd = if end'.IsUndefined then len else end' |> TC.ToInteger
      let finalStart = Math.Min(Math.Max(intStart, 0), len)
      let finalEnd = Math.Min(Math.Max(intEnd, 0), len)
      let from = Math.Min(finalStart, finalEnd)
      let to' = Math.Max(finalStart, finalEnd)
      S.Substring(from, to' - from)

    ///
    let private toLowerCase (_:FO) (this:CO) =
      let value = this |> TypeConverter.ToString
      value.ToLowerInvariant()
    
    ///
    let private toLocaleLowerCase (_:FO) (this:CO) =
      let value = this |> TypeConverter.ToString
      value.ToLower()
    
    ///
    let private toUpperCase (_:FO) (this:CO) =
      let value = this |> TypeConverter.ToString
      value.ToUpperInvariant()
    
    ///
    let private toLocaleUpperCase (_:FO) (this:CO) =
      let value = this |> TypeConverter.ToString
      value.ToUpper()
        
    ///
    let create (env:Env) ownPrototype =
      let prototype = env.NewString()
      prototype.Prototype <- ownPrototype
      prototype
    
    ///
    let setup (env:Env) =
      let proto = env.Prototypes.String;

      proto.Put("constructor", env.Constructors.String, DontEnum)

      let toString = Function(toString) $ Utils.createFunction env (Some 0)
      proto.Put("toString", toString, DontEnum)

      let valueOf = Function(valueOf) $ Utils.createFunction env (Some 0)
      proto.Put("valueOf", valueOf, DontEnum)

      let charAt = FunctionReturn<double, string>(charAt) $ Utils.createFunction env (Some 1)
      proto.Put("charAt", charAt, DontEnum)

      let charCodeAt = FunctionReturn<double, double>(charCodeAt)  $ Utils.createFunction env (Some 1)
      proto.Put("charCodeAt", charCodeAt, DontEnum)

      let concat = FunctionReturn<Args, string>(concat) $ Utils.createFunction env (Some 1)
      proto.Put("concat", concat, DontEnum)

      let indexOf = FunctionReturn<string, double, double>(indexOf) $ Utils.createFunction env (Some 1)
      proto.Put("indexOf", indexOf, DontEnum)

      let lastIndexOf = FunctionReturn<BV, BV, double>(lastIndexOf) $ Utils.createFunction env (Some 1)
      proto.Put("lastIndexOf", lastIndexOf, DontEnum)

      let localeCompare = FunctionReturn<string, double>(localeCompare) $ Utils.createFunction env (Some 1)
      proto.Put("localeCompare", localeCompare, DontEnum)

      let match' = Function<BV>(match') $ Utils.createFunction env (Some 1)
      proto.Put("match", match', DontEnum)

      let replace = FunctionReturn<BV, BV, string>(replace) $ Utils.createFunction env (Some 2)
      proto.Put("replace", replace, DontEnum)

      let search = FunctionReturn<BV, double>(search) $ Utils.createFunction env (Some 1)
      proto.Put("search", search, DontEnum)

      let slice = FunctionReturn<double, BV, string>(slice) $ Utils.createFunction env (Some 2)
      proto.Put("slice", slice, DontEnum)

      let split = FunctionReturn<BV, BV, CO>(split) $ Utils.createFunction env (Some 2)
      proto.Put("split", split, DontEnum)

      let substring = FunctionReturn<double, BV, string>(substring) $ Utils.createFunction env (Some 2)
      proto.Put("substring", substring, DontEnum)

      let toLowerCase = FunctionReturn<string>(toLowerCase) $ Utils.createFunction env (Some 0)
      proto.Put("toLowerCase", toLowerCase, DontEnum)
    
      let toLocaleLowerCase = FunctionReturn<string>(toLocaleLowerCase) $ Utils.createFunction env (Some 0)
      proto.Put("toLocaleLowerCase", toLocaleLowerCase, DontEnum)

      let toUpperCase = FunctionReturn<string>(toUpperCase) $ Utils.createFunction env (Some 0)
      proto.Put("toUpperCase", toUpperCase, DontEnum)

      let toLocaleUpperCase = FunctionReturn<string>(toLocaleUpperCase) $ Utils.createFunction env (Some 0)
      proto.Put("toLocaleUpperCase", toLocaleUpperCase, DontEnum)
