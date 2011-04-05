namespace IronJS.Native

open System
open System.Text.RegularExpressions

open IronJS
open IronJS.Support.Aliases
open IronJS.DescriptorAttrs

module String =

  //----------------------------------------------------------------------------
  let internal constructor' (ctor:FO) (this:CO) (args:Args) =
    let value = 
      match args.Length with
      | 0 -> ""
      | _ -> args.[0] |> TC.ToString

    match this with
    | null -> ctor.Env.NewString(value) |> BV.Box
    | _ -> value |> BV.Box
    
  //----------------------------------------------------------------------------
  let internal fromCharCode (args:Args) =
    let buffer = Text.StringBuilder(args.Length)

    for i = 0 to args.Length-1 do
      buffer.Append(TypeConverter.ToUInt16 args.[i] |> char) |> ignore

    buffer.ToString()

  //----------------------------------------------------------------------------
  let internal toString (func:FO) (this:CO) =
    this.CheckType<SO>()
    this |> ValueObject.GetValue
    
  //----------------------------------------------------------------------------
  let internal valueOf (func:FO) (this:CO) = 
    toString func this
    
  // These steps are outlined in the ECMA-262, Section 15.5.4.4
  let internal charAt (this:CO) (pos:double) =
    let S = this |> TC.ToString
    let position = pos |> TC.ToInteger
    let size = S.Length
    if position < 0 || position >= size then "" else S.[position] |> string

  // These steps are outlined in the ECMA-262, Section 15.5.4.5
  let internal charCodeAt (this:CO) (pos:double) =
    let S = this |> TC.ToString
    let position = pos |> TC.ToInteger
    let size = S.Length
    if position < 0 || position >= size then nan else S.[position] |> double

  //----------------------------------------------------------------------------
  let internal concat (this:CO) (args:Args) =
    let buffer = new Text.StringBuilder(TypeConverter.ToString this)
    let toString (x:BoxedValue) = buffer.Append(TypeConverter.ToString x)
    args |> Array.iter (toString >> ignore)
    buffer.ToString()
    
  //----------------------------------------------------------------------------
  let internal indexOf (this:CO) (subString:string) (index:double) =
    let value = this |> TypeConverter.ToString
    let index = index |> TypeConverter.ToInt32
    let index = Math.Min(Math.Max(index, 0), value.Length);
    value.IndexOf(subString, index, StringComparison.Ordinal) |> double
    
  //----------------------------------------------------------------------------
  let internal lastIndexOf (this:CO) (subString:string) (index:double) =
    let value = this |> TypeConverter.ToString

    let index = 
      if Double.IsNaN index 
        then Int32.MaxValue 
        else TypeConverter.ToInteger index

    let index = Math.Min(index, value.Length-1)
    let index = Math.Min(index + subString.Length-1, value.Length-1)
    
    let index = 
      if index < 0 
        then  if value = "" && subString = "" then 0 else -1
        else value.LastIndexOf(subString, index, StringComparison.Ordinal)

    index |> double
      
  //----------------------------------------------------------------------------
  let internal localeCompare (this:CO) (that:CO) =
    let value = this |> TypeConverter.ToString
    let that = this |> TypeConverter.ToString
    String.Compare(value, that) |> double
    
  //----------------------------------------------------------------------------
  let private toRegExp (env:Env) (regexp:BV) =
    match regexp.Tag with
    | TypeTags.String -> env.NewRegExp(regexp.String) :?> RO
    | _ -> regexp.Object.CastTo<RO>()
    
  let internal match' (f:FO) (this:CO) (regexp:BV) =
    let regexp = regexp |> toRegExp f.Env
    RegExp.exec f regexp (this |> TC.ToString |> BV.Box)

  let private replacePattern =
    new Regex(@"\$\$|\$&|\$`|\$'|\$\d{1,2}", RegexOptions.Compiled)

  let private evaluateReplacement (matched:string) (before:string) (after:string) (replacement:string) (groups:GroupCollection) =
    if replacement.Contains("$") then
      
      replacePattern.Replace(replacement, MatchEvaluator(fun m ->
        match m.Value with
        | "$$" -> "$"
        | "$&" -> matched
        | "$`" -> before
        | "$'" -> after
        | _ ->
          match m.Value.Substring 1 |> int with
          | 0 -> m.Value
          | subPatternIndex  -> 
            if subPatternIndex < groups.Count && groups <> null
              then groups.[subPatternIndex].Value
              else "$" + string subPatternIndex
      ))

    else
      replacement

    
  //----------------------------------------------------------------------------
  let internal replace (this:CO) (search:BV) (replace:BV) =
    let value = this |> TC.ToString

    //replace(regex, _)
    if search.IsRegExp then 
      let search = search |> toRegExp this.Env
      let count = if search.Global then Int32.MaxValue else 1
      let lastIndex = search.Get("lastIndex") |> TC.ToInt32
      let lastIndex = if search.Global then 0 else Math.Max(0, lastIndex-1)
      if search.Global then search.Put("lastIndex", 0.0)

      //replace(regex, function)
      if replace.IsFunction then

        let matchEval (m:Match) =
          if not search.Global then
            search.Put("lastIndex", m.Index + 1 |> double)

          let params' = MutableList<BV>()

          for g in m.Groups do
            if g.Success 
              then params'.Add(g.Value |> BV.Box)
              else params'.Add(Undefined.Boxed)

          let args = params'.ToArray()
          let this = this.Env.Globals
          Utils.invoke replace.Func this args |> TC.ToString
        
        //Run regex on our input, using matchEval for replacement
        search.RegExp.Replace(value, MatchEvaluator matchEval, count, lastIndex)

      //replace(regex, string)
      else
        let replace = replace |> TC.ToString

        let matchEval (m:Match) =
          if not search.Global then
            search.Put("lastIndex", m.Index + 1 |> double)

          let before = value.Substring(0, m.Index)
          let after = value.Substring(Math.Min(value.Length - 1, m.Index + m.Length))
          evaluateReplacement m.Value before after replace m.Groups

        search.RegExp.Replace(value, MatchEvaluator matchEval, count, lastIndex)
      
    //replace(string, _)
    else
      let search = search |> TC.ToString
      let index = value.IndexOf search

      if index > -1 then
      
        //replace(string, function)
        if replace.IsFunction then 
          let replace = replace.Func.Call(this.Env.Globals, search, index, value) |> TC.ToString
          value.Substring(0, index) + replace + value.Substring(index + search.Length)

        //replace(string, string)
        else
          let before = value.Substring(0, index)
          let after = value.Substring(index + value.Length)
          let replace = replace |> TC.ToString
          let replace = evaluateReplacement search before after replace null
          before + replace + after

      else
        value
          
  //----------------------------------------------------------------------------
  let internal search (this:CO) (search:BoxedValue) =
    let value = this |> TypeConverter.ToString

    //search(regex)
    if search.Tag >= TypeTags.Object then 
      let regexp = search |> toRegExp this.Env
      let m = regexp.RegExp.Match(value)
      if m |> FSharp.Utils.notNull && m.Success 
        then m.Index |> double
        else 0.0
      
    //search(string)
    else
      let search = search |> TypeConverter.ToString
      value.IndexOf(search, StringComparison.Ordinal) |> double

  // These steps are outlined in the ECMA-262, Section 15.5.4.13
  let internal slice (this:CO) (start:double) (end':BoxedValue) =
    let S = this |> TC.ToString
    let len = S.Length
    let intStart = start |> TC.ToInteger
    let intEnd = if end'.IsUndefined then len else end' |> TC.ToInteger
    let from = if intStart < 0 then Math.Max(len + intStart, 0) else Math.Min(intStart, len)
    let to' = if intEnd < 0 then Math.Max(len + intEnd, 0) else Math.Min(intEnd, len)
    let span = Math.Max(to' - from, 0)
    S.Substring(from, span)

  //----------------------------------------------------------------------------
  let internal split (f:FO) (this:CO) (separator:BV) (limit:BV) =
    let value = this |> TC.ToString
    
    let limit =
      if limit.IsUndefined
        then Int32.MaxValue 
        else limit |> TC.ToInt32

    let parts = 
      if separator.IsRegExp then
        let separator = separator.Object.CastTo<RO>()
        separator.RegExp.Split(value, limit)

      else
        let separator =
          if separator.IsUndefined
            then "" 
            else separator |> TC.ToString

        value.Split([|separator|], limit, StringSplitOptions.None)

    let array = f.Env.NewArray(parts.Length |> uint32)
    for i = 0 to parts.Length-1 do
      array.Put(uint32 i, parts.[i])

    array

  // These steps are outlined in the ECMA-262, Section 15.5.4.15
  let internal substring (this:CommonObject) (start:double) (end':BV) =
    let S = this |> TC.ToString
    let len = S.Length
    let intStart = start |> TC.ToInteger
    let intEnd = if end'.IsUndefined then len else end' |> TC.ToInteger
    let finalStart = Math.Min(Math.Max(intStart, 0), len)
    let finalEnd = Math.Min(Math.Max(intEnd, 0), len)
    let from = Math.Min(finalStart, finalEnd)
    let to' = Math.Max(finalStart, finalEnd)
    S.Substring(from, to' - from)

  //----------------------------------------------------------------------------
  let internal toLowerCase (this:CO) =
    let value = this |> TypeConverter.ToString
    value.ToLowerInvariant()
    
  //----------------------------------------------------------------------------
  let internal toLocaleLowerCase (this:CO) =
    let value = this |> TypeConverter.ToString
    value.ToLower()
    
  //----------------------------------------------------------------------------
  let internal toUpperCase (this:CO) =
    let value = this |> TypeConverter.ToString
    value.ToUpperInvariant()
    
  //----------------------------------------------------------------------------
  let internal toLocaleUpperCase (this:CO) =
    let value = this |> TypeConverter.ToString
    value.ToUpper()
        
  //----------------------------------------------------------------------------
  let createPrototype (env:Environment) objPrototype =
    let prototype = env.NewString()
    prototype.Put("length", 0.0)
    prototype.Prototype <- objPrototype
    prototype
    
  //----------------------------------------------------------------------------
  let setupConstructor (env:Environment) =
    let ctor = new Func<FO, CO, Args, BV>(constructor')
    let ctor = Utils.createHostFunction env ctor

    let fromCharCode = new Func<BoxedValue array, string>(fromCharCode)
    let fromCharCode = Utils.createHostFunction env fromCharCode
    ctor.Put("fromCharCode", fromCharCode, DontEnum)

    ctor.Put("prototype", env.Prototypes.String, Immutable)

    env.Globals.Put("String", ctor, DontEnum)
    env.Constructors <- {env.Constructors with String=ctor}
    
  //----------------------------------------------------------------------------
  let setupPrototype (env:Environment) =
    let proto = env.Prototypes.String;

    proto.Put("constructor", env.Constructors.String, DontEnum) // 15.6.4.1

    let toString = new Func<FunctionObject, CommonObject, BoxedValue>(toString)
    let toString = Utils.createHostFunction env toString
    proto.Put("toString", toString, DontEnum)

    let valueOf = new Func<FunctionObject, CommonObject, BoxedValue>(valueOf)
    let valueOf = Utils.createHostFunction env valueOf
    proto.Put("valueOf", valueOf, DontEnum)

    let charAt = new Func<CommonObject, double, string>(charAt)
    let charAt = Utils.createHostFunction env charAt
    proto.Put("charAt", charAt, DontEnum)

    let charCodeAt = new Func<CommonObject, double, double>(charCodeAt)
    let charCodeAt = Utils.createHostFunction env charCodeAt
    proto.Put("charCodeAt", charCodeAt, DontEnum)

    let concat = new Func<CommonObject, BoxedValue array, string>(concat)
    let concat = Utils.createHostFunction env concat
    proto.Put("concat", concat, DontEnum)

    let indexOf = new Func<CommonObject, string, double, double>(indexOf)
    let indexOf = Utils.createHostFunction env indexOf
    proto.Put("indexOf", indexOf, DontEnum)

    let lastIndexOf = new Func<CommonObject, string, double, double>(lastIndexOf)
    let lastIndexOf = Utils.createHostFunction env lastIndexOf
    proto.Put("lastIndexOf", lastIndexOf, DontEnum)

    let localeCompare = new Func<CommonObject, CommonObject, double>(localeCompare)
    let localeCompare = Utils.createHostFunction env localeCompare
    proto.Put("localeCompare", localeCompare, DontEnum)

    let match' = JsFunc<BV>(match')
    let match' = Utils.createHostFunction env match'
    proto.Put("match", match', DontEnum)

    let replace = new Func<CommonObject, BoxedValue, BoxedValue, string>(replace)
    let replace = Utils.createHostFunction env replace
    proto.Put("replace", replace, DontEnum)

    let search = new Func<CommonObject, BoxedValue, double>(search)
    let search = Utils.createHostFunction env search
    proto.Put("search", search, DontEnum)

    let slice = new Func<CommonObject, double, BoxedValue, string>(slice)
    let slice = Utils.createHostFunction env slice
    proto.Put("slice", slice, DontEnum)

    let split = new Func<FunctionObject, CommonObject, BoxedValue, BoxedValue, CommonObject>(split)
    let split = Utils.createHostFunction env split
    proto.Put("split", split, DontEnum)

    let substring = new Func<CommonObject, double, BV, string>(substring)
    let substring = Utils.createHostFunction env substring
    proto.Put("substring", substring, DontEnum)

    let toLowerCase = new Func<CommonObject, string>(toLowerCase)
    let toLowerCase = Utils.createHostFunction env toLowerCase
    proto.Put("toLowerCase", toLowerCase, DontEnum)
    
    let toLocaleLowerCase = new Func<CommonObject, string>(toLocaleLowerCase)
    let toLocaleLowerCase = Utils.createHostFunction env toLocaleLowerCase
    proto.Put("toLocaleLowerCase", toLocaleLowerCase, DontEnum)

    let toUpperCase = new Func<CommonObject, string>(toUpperCase)
    let toUpperCase = Utils.createHostFunction env toUpperCase
    proto.Put("toUpperCase", toUpperCase, DontEnum)

    let toLocaleUpperCase = new Func<CommonObject, string>(toLocaleUpperCase)
    let toLocaleUpperCase = Utils.createHostFunction env toLocaleUpperCase
    proto.Put("toLocaleUpperCase", toLocaleUpperCase, DontEnum)
