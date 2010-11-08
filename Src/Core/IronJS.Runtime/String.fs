namespace IronJS.Native

open System
open IronJS
open IronJS.DescriptorAttrs
open IronJS.Api.Extensions

module String =

  //----------------------------------------------------------------------------
  let internal constructor' (ctor:IjsFunc) (this:IjsObj) (value:IjsBox) =
    let value = Api.TypeConverter.toString value
    match this with
    | null -> Api.Environment.createString ctor.Env value |> Utils.boxObject
    | _ -> value |> Utils.boxString
    
  //----------------------------------------------------------------------------
  let internal fromCharCode (args:IjsBox array) =
    let buffer = Text.StringBuilder(args.Length)

    for i = 0 to args.Length-1 do
      buffer.Append(Api.TypeConverter.toUInt16 args.[i] |> char) |> ignore

    buffer.ToString()
    
  //----------------------------------------------------------------------------
  let internal toString (func:IjsFunc) (this:IjsObj) =
    this |> Utils.mustBe Classes.String func.Env
    this.Value.Box
    
  //----------------------------------------------------------------------------
  let internal valueOf (func:IjsFunc) (this:IjsObj) = 
    toString func this
    
  //----------------------------------------------------------------------------
  let internal charAt (this:IjsObj) (pos:IjsNum) =
    let value = Api.TypeConverter.toString this
    let index = Api.TypeConverter.toInt32 pos
    if index < 0 || index >= value.Length then "" else value.[index] |> string

  //----------------------------------------------------------------------------
  let internal charCodeAt (this:IjsObj) (pos:IjsNum) =
    let value = Api.TypeConverter.toString this
    let index = Api.TypeConverter.toInt32 pos
    if index < 0 || index >= value.Length then nan else value.[index] |> double
    
  //----------------------------------------------------------------------------
  let internal concat (this:IjsObj) (args:IjsBox array) =
    let buffer = new Text.StringBuilder(Api.TypeConverter.toString this)
    let toString (x:IjsBox) = buffer.Append(Api.TypeConverter.toString x)
    args |> Array.iter (toString >> ignore)
    buffer.ToString()
    
  //----------------------------------------------------------------------------
  let internal indexOf (this:IjsObj) (subString:IjsStr) (index:IjsNum) =
    let value = this |> Api.TypeConverter.toString
    let index = index |> Api.TypeConverter.toInt32
    let index = Math.Min(Math.Max(index, 0), value.Length);
    value.IndexOf(subString, index, StringComparison.Ordinal) |> double
    
  //----------------------------------------------------------------------------
  let internal lastIndexOf (this:IjsObj) (subString:IjsStr) (index:IjsNum) =
    let value = this |> Api.TypeConverter.toString

    let index = 
      if Double.IsNaN index 
        then Int32.MaxValue 
        else Api.TypeConverter.toInteger index

    let index = Math.Min(index, value.Length-1)
    let index = Math.Min(index + subString.Length-1, value.Length-1)
    
    let index = 
      if index < 0 
        then  if value = "" && subString = "" then 0 else -1
        else value.LastIndexOf(subString, index, StringComparison.Ordinal)

    index |> double
      
  //----------------------------------------------------------------------------
  let internal localeCompare (this:IjsObj) (that:IjsObj) =
    let value = this |> Api.TypeConverter.toString
    let that = this |> Api.TypeConverter.toString
    String.Compare(value, that) |> double
    
  //----------------------------------------------------------------------------
  let internal match' (this:IjsObj) (regexp:IjsObj) =
    failwith "Not implemented"
    false
    
  //----------------------------------------------------------------------------
  let internal replace (this:IjsObj) (search:IjsBox) (replace:IjsBox) =
    let value = this |> Api.TypeConverter.toString

    //replace(regex, *)
    if search.Tag >= TypeTags.Object then 
      failwith "Not implemented"
      
    //replace(string, *)
    else
      let search = search |> Api.TypeConverter.toString
      
      if replace.Tag >= TypeTags.Function then 
        //replace(string, function)
        failwith "Not implemented"

      else
        //replace(string, string)
        let replace = replace |> Api.TypeConverter.toString
        let startIndex = value.IndexOf search
        if startIndex = -1 then value
        else
          let endIndex = startIndex + search.Length
          let bufferSize = value.Length + (replace.Length - search.Length)
          let buffer = new Text.StringBuilder(bufferSize);
          buffer.Append(value, 0, startIndex) |> ignore
          buffer.Append(replace) |> ignore
          buffer.Append(value, endIndex, value.Length - endIndex) |> ignore
          buffer.ToString()
          
  //----------------------------------------------------------------------------
  let internal search (this:IjsObj) (search:IjsBox) =
    let value = this |> Api.TypeConverter.toString

    //search(regex)
    if search.Tag >= TypeTags.Object then 
      failwith "Not implemented"
      
    //search(string)
    else
      let search = search |> Api.TypeConverter.toString
      value.IndexOf(search, StringComparison.Ordinal) |> double
      
  //----------------------------------------------------------------------------
  let internal slice (this:IjsObj) (start:IjsNum) (end':IjsBox) =
    let value = this  |> Api.TypeConverter.toString
    let start = start |> Api.TypeConverter.toInteger

    let isUndefined = end'.Tag |> Utils.Box.isUndefined  
    let end' = if isUndefined then start else value.Length

    let start = Math.Min(Math.Max(start, 0), value.Length)
    let end'  = Math.Min(Math.Max(end', 0), value.Length)

    if end' <= start then "" else value.Substring(start, end' - start)
    
  //----------------------------------------------------------------------------
  let internal split (f:IjsFunc) (this:IjsObj) (sep:IjsBox) (limit:IjsBox) =
    let value = this |> Api.TypeConverter.toString

    if sep |> Utils.Box.isRegExp then
      failwith "Not implemented"

    else
      let separator =
        if sep.Tag |> Utils.Box.isUndefined
          then "" else sep |> Api.TypeConverter.toString

      let limit =
        if limit.Tag |> Utils.Box.isUndefined
          then UInt32.MaxValue else limit |> Api.TypeConverter.toUInt32
          
      if separator |> String.IsNullOrEmpty then
        let length = Math.Min(uint32 value.Length, limit)
        let array = Api.Environment.createArray f.Env length
        for i = 0 to value.Length-1 do
          if uint32 i < limit then
            let descr = &array.IndexDense.[i]
            descr.Box.Clr <- string value.[i]
            descr.Box.Tag <- TypeTags.String
            descr.HasValue <- true
        array

      else
        let parts = value.Split([|separator|], StringSplitOptions.None)
        let length = Math.Min(uint32 parts.Length, limit)
        let array = Api.Environment.createArray f.Env length
        for i = 0 to parts.Length-1 do
          if uint32 i < limit then
            let descr = &array.IndexDense.[i]
            descr.Box.Clr <- parts.[i]
            descr.Box.Tag <- TypeTags.String
            descr.HasValue <- true
        array
        
  //----------------------------------------------------------------------------
  let internal substring (this:IjsObj) (start:IjsNum) (end':IjsNum) =
    let value = this |> Api.TypeConverter.toString

    let start = start |> Api.TypeConverter.toInt32
    let start = if start < 0 then Math.Max(start + value.Length, 0) else start

    let end' = end' |> Api.TypeConverter.toInt32
    let end' = Math.Max(Math.Min(end', value.Length-start), 0)

    if end' <= 0 then "" else value.Substring(start, end')
    
  //----------------------------------------------------------------------------
  let internal toLowerCase (this:IjsObj) =
    let value = this |> Api.TypeConverter.toString
    value.ToLowerInvariant()
    
  //----------------------------------------------------------------------------
  let internal toLocaleLowerCase (this:IjsObj) =
    let value = this |> Api.TypeConverter.toString
    value.ToLower()
    
  //----------------------------------------------------------------------------
  let internal toUpperCase (this:IjsObj) =
    let value = this |> Api.TypeConverter.toString
    value.ToUpperInvariant()
    
  //----------------------------------------------------------------------------
  let internal toLocaleUpperCase (this:IjsObj) =
    let value = this |> Api.TypeConverter.toString
    value.ToUpper()
        
  //----------------------------------------------------------------------------
  let createPrototype (env:IjsEnv) objPrototype =
    let prototype = Api.Environment.createObject env
    prototype.Methods.PutValProperty.Invoke(prototype, "length", 0.0)
    prototype.Class <- Classes.String
    prototype.Value.Box.String <- ""
    prototype.Value.Box.Tag <- TypeTags.String
    prototype.Prototype <- objPrototype
    prototype
    
  //----------------------------------------------------------------------------
  let setupConstructor (env:IjsEnv) =
    let ctor = new Func<IjsFunc, IjsObj, IjsBox, IjsBox>(constructor')
    let ctor = Api.HostFunction.create env ctor

    let fromCharCode = new Func<IjsBox array, IjsStr>(fromCharCode)
    let fromCharCode = Api.HostFunction.create env fromCharCode
    ctor.put("fromCharCode", fromCharCode, DontEnum)

    ctor.ConstructorMode <- ConstructorModes.Host
    ctor.put("prototype", env.Prototypes.String, Immutable)

    env.Globals.put("String", ctor)
    env.Constructors <- {env.Constructors with String=ctor}
    
  //----------------------------------------------------------------------------
  let setupPrototype (env:IjsEnv) =
    let proto = env.Prototypes.String;

    proto.put("constructor", env.Constructors.String, DontEnum) // 15.6.4.1

    let toString = new Func<IjsFunc, IjsObj, IjsBox>(toString)
    let toString = Api.HostFunction.create env toString
    proto.put("toString", toString, DontEnum)

    let valueOf = new Func<IjsFunc, IjsObj, IjsBox>(valueOf)
    let valueOf = Api.HostFunction.create env valueOf
    proto.put("valueOf", valueOf, DontEnum)

    let charAt = new Func<IjsObj, IjsNum, IjsStr>(charAt)
    let charAt = Api.HostFunction.create env charAt
    proto.put("charAt", charAt, DontEnum)

    let charCodeAt = new Func<IjsObj, IjsNum, IjsNum>(charCodeAt)
    let charCodeAt = Api.HostFunction.create env charCodeAt
    proto.put("charCodeAt", charCodeAt, DontEnum)

    let concat = new Func<IjsObj, IjsBox array, IjsStr>(concat)
    let concat = Api.HostFunction.create env concat
    proto.put("concat", concat, DontEnum)

    let indexOf = new Func<IjsObj, IjsStr, IjsNum, IjsNum>(indexOf)
    let indexOf = Api.HostFunction.create env indexOf
    proto.put("indexOf", indexOf, DontEnum)

    let lastIndexOf = new Func<IjsObj, IjsStr, IjsNum, IjsNum>(lastIndexOf)
    let lastIndexOf = Api.HostFunction.create env lastIndexOf
    proto.put("lastIndexOf", lastIndexOf, DontEnum)

    let localeCompare = new Func<IjsObj, IjsObj, IjsNum>(localeCompare)
    let localeCompare = Api.HostFunction.create env localeCompare
    proto.put("localeCompare", localeCompare, DontEnum)

    let match' = new Func<IjsObj, IjsObj, IjsBool>(match')
    let match' = Api.HostFunction.create env match'
    proto.put("match", match', DontEnum)

    let replace = new Func<IjsObj, IjsBox, IjsBox, IjsStr>(replace)
    let replace = Api.HostFunction.create env replace
    proto.put("replace", replace, DontEnum)

    let search = new Func<IjsObj, IjsBox, IjsNum>(search)
    let search = Api.HostFunction.create env search
    proto.put("search", search, DontEnum)

    let slice = new Func<IjsObj, IjsNum, IjsBox, IjsStr>(slice)
    let slice = Api.HostFunction.create env slice
    proto.put("slice", slice, DontEnum)

    let split = new Func<IjsFunc, IjsObj, IjsBox, IjsBox, IjsObj>(split)
    let split = Api.HostFunction.create env split
    proto.put("split", split, DontEnum)

    let substring = new Func<IjsObj, IjsNum, IjsNum, IjsStr>(substring)
    let substring = Api.HostFunction.create env substring
    proto.put("substring", substring, DontEnum)

    let toLowerCase = new Func<IjsObj, IjsStr>(toLowerCase)
    let toLowerCase = Api.HostFunction.create env toLowerCase
    proto.put("toLowerCase", toLowerCase, DontEnum)
    
    let toLocaleLowerCase = new Func<IjsObj, IjsStr>(toLocaleLowerCase)
    let toLocaleLowerCase = Api.HostFunction.create env toLocaleLowerCase
    proto.put("toLocaleLowerCase", toLocaleLowerCase, DontEnum)

    let toUpperCase = new Func<IjsObj, IjsStr>(toUpperCase)
    let toUpperCase = Api.HostFunction.create env toUpperCase
    proto.put("toUpperCase", toUpperCase, DontEnum)

    let toLocaleUpperCase = new Func<IjsObj, IjsStr>(toLocaleUpperCase)
    let toLocaleUpperCase = Api.HostFunction.create env toLocaleUpperCase
    proto.put("toLocaleUpperCase", toLocaleUpperCase, DontEnum)
