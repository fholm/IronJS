namespace IronJS.Native

// Removes the warning for using 
// "constructor" as an identifier
#nowarn "46"

open System
open System.Globalization

open IronJS
open IronJS.Support.Aliases
open IronJS.Support.CustomOperators
open IronJS.ExtensionMethods
open IronJS.DescriptorAttrs

module Date =

  open IronJS.FSharpOperators

  module private Formats = 
    let full = "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'zzz"
    //let utc = "ddd, dd MMM yyyy HH':'mm':'ss 'UTC'"
    let date = "ddd, dd MMM yyyy"
    let time = "HH':'mm':'ss 'GMT'zzz"

  type private DT = DateTime
    
  let private ic = invariantCulture :> IFormatProvider
  let private cc = currentCulture:> IFormatProvider

  let private utcZeroDate() = new DT(0L, DateTimeKind.Utc)
  let private localZeroDate() = new DT(0L, DateTimeKind.Local)

  let private invalidDate = 
    DateTime.MinValue

  let private parseDate input culture (result:DT byref) : bool =   
    let none = DateTimeStyles.None 

    if DateTime.TryParse(input, culture, none, &result) |> not then
      if DateTime.TryParseExact(input, Formats.full, culture, none, &result) |> not then
        let mutable temp = utcZeroDate()

        if DateTime.TryParseExact(input, Formats.date, culture, none, &temp) then
          result <- result.AddTicks(temp.Ticks)

        if DateTime.TryParseExact(input, Formats.time, culture, none, &temp) then
          result <- result.AddTicks(temp.Ticks)

        if temp.Ticks > 0L then true
        else
          result <- utcZeroDate()
          false

      else true
    else true
  
  let private constructor' (f:FO) (o:CO) (args:Args) =
    let date = 
      match args.Length with
      | 0 -> f.Env.NewDate(DT.Now.ToLocalTime())
      | 1 -> 
        let value = args.[0].ToPrimitive()

        match value.Tag with
        | TypeTags.String ->
          let value = string value.String
          let mutable date = localZeroDate()

          if parseDate value cc &date || parseDate value ic &date 
            then f.Env.NewDate(date.ToLocalTime())
            else f.Env.NewDate(localZeroDate())

        | _ -> 
          let value = value.ToNumber()
          f.Env.NewDate(value |> DateObject.TicksToDateTime)

      | _ ->
        let mutable date = localZeroDate()

        //Year and Month
        let mutable year = args.[0].ToInt32() - 1
        if year < 100 then year <- year + 1900
        date <- date.AddYears(year)
        date <- date.AddMonths(args.[1].ToInt32())

        if args.Length > 2 then date <- date.AddDays(args.[2].ToNumber() - 1.0)
        if args.Length > 3 then date <- date.AddHours(args.[3].ToNumber())
        if args.Length > 4 then date <- date.AddMinutes(args.[4].ToNumber())
        if args.Length > 5 then date <- date.AddSeconds(args.[5].ToNumber())
        if args.Length > 6 then date <- date.AddMilliseconds(args.[6].ToNumber())

        f.Env.NewDate(date)

    match o with
    | null -> date |> BV.Box
    | _ -> date.CallMember("toString")

  let private parseGeneric input culture =
    let mutable date = utcZeroDate()
    if parseDate input culture &date 
      then DateObject.DateTimeToTicks(date) |> float
      else NaN

  let private parse (f:FO) (o:CO) input = 
    parseGeneric input ic |> BV.Box

  let private parseLocal (f:FO) (o:CO) input = 
    parseGeneric input cc |> BV.Box

  let private utc (f:FO) (o:CO) (args:Args) =
    let mutable failed = false

    for i = 0 to args.Length-1 do
      let a = args.[i]
      let n = a.Number
      if a.IsUndefined then failed <- true
      if a.IsNumber && Double.IsNaN n then failed <- true
      if a.IsNumber && Double.IsInfinity n then failed <- true

    if failed then NaN |> BV.Box
    else
      let date = (constructor' f null args).Object :?> DO
      let ticks = date.Date |> DateObject.DateTimeToTicks |> float 
      let offset = TimeZone.CurrentTimeZone.GetUtcOffset(DT.Now).TotalMilliseconds
      ticks + offset |> BV.Box

  let setup (env:Environment) =
    let ctor = new VariadicFunction(constructor')
    let ctor = ctor $ Utils.createConstructor env (Some 7)

    ctor.MetaData.Name <- "Date"

    let parse = FunctionReturn<string, BV>(parse) $ Utils.createFunction env (Some 1)
    ctor.Put("parse", parse, DontEnum)

    let parseLocal = FunctionReturn<string, BV>(parseLocal) $ Utils.createFunction env (Some 1)
    ctor.Put("parseLocal", parseLocal, DontEnum)

    let utc = VariadicFunction(utc) $ Utils.createFunction env (Some 7)
    ctor.Put("UTC", utc, DontEnum)

    ctor.Put("prototype", env.Prototypes.Date, Immutable)

    env.Globals.Put("Date", ctor, DontEnum)
    env.Constructors <- {env.Constructors with Date=ctor}

  module Prototype =
    
    module private DT =
    
      let setYear (value:double) (d:DT) = 
        let d = new DateTime(int value, d.Month, d.Day, d.Hour, d.Minute, d.Second, DateTimeKind.Local)
        d.AddMilliseconds(d.Millisecond |> double)

      let setMonth (value:double) (d:DT) = d.AddMonths(-d.Month).AddMonths((int value)+1)
      let setDate (value:double) (d:DT) = d.AddDays(-(d.Day |> double)).AddDays(value)
      let setHours (value:double) (d:DT) = d.AddHours(-(d.Hour |> double)).AddHours(value)
      let setMinutes (value:double) (d:DT) = d.AddMinutes(-(d.Minute |> double)).AddMinutes(value)
      let setSeconds (value:double) (d:DT) = d.AddSeconds(-(d.Second |> double)).AddSeconds(value)
      let setMilliseconds (value:double) (d:DT) = 
        d.AddMilliseconds(-(d.Millisecond |> double)).AddMilliseconds(value)

    open IronJS.FSharpOperators

    let private toStringGeneric (o:CO) (format:string) culture =
      o.CastTo<DO>().Date.ToLocalTime().ToString(Formats.full, culture) |> BV.Box

    let private toString (f:FO) (o:CO) = toStringGeneric o Formats.full ic
    let private toDateString (f:FO) (o:CO) = toStringGeneric o Formats.date ic
    let private toTimeString (f:FO) (o:CO) = toStringGeneric o Formats.time ic
    let private toLocaleString (f:FO) (o:CO) = toStringGeneric o Formats.full cc
    let private toLocaleDateString (f:FO) (o:CO) = toStringGeneric o Formats.date cc
    let private toLocaleTimeString (f:FO) (o:CO) = toStringGeneric o Formats.time cc
    let private toUTCString = toString

    let private valueOf (f:FO) (o:CO) =
      let dt = o.CastTo<DO>()
      if dt.HasValidDate then
        dt.Date |> DateObject.DateTimeToTicks |> float |> BV.Box
      else
        nan |> BV.Box

    let private toLocalTime (o:CO) = o.CastTo<DO>().Date.ToLocalTime()
    let private toUTCTime (o:CO) = o.CastTo<DO>().Date.ToUniversalTime()

    let private getTime (f:FO) (o:CO) = valueOf f o
    let private getFullYear (f:FO) (o:CO) = (toLocalTime o).Year |> double |> BV.Box
    let private getUTCFullYear (f:FO) (o:CO) = (toUTCTime o).Year |> double |> BV.Box
    let private getMonth (f:FO) (o:CO) = (toLocalTime o).Month-1 |> double |> BV.Box
    let private getUTCMonth (f:FO) (o:CO) = (toUTCTime o).Month-1 |> double |> BV.Box
    let private getDate (f:FO) (o:CO) = (toLocalTime o).Day |> double |> BV.Box
    let private getUTCDate (f:FO) (o:CO) = (toUTCTime o).Day |> double |> BV.Box
    let private getDay (f:FO) (o:CO) = (toLocalTime o).DayOfWeek |> double |> BV.Box
    let private getUTCDay (f:FO) (o:CO) = (toUTCTime o).DayOfWeek |> double |> BV.Box
    let private getHours (f:FO) (o:CO) = (toLocalTime o).Hour |> double |> BV.Box
    let private getUTCHours (f:FO) (o:CO) = (toUTCTime o).Hour |> double |> BV.Box
    let private getMinutes (f:FO) (o:CO) = (toLocalTime o).Minute |> double |> BV.Box
    let private getUTCMinutes (f:FO) (o:CO) = (toUTCTime o).Minute |> double |> BV.Box
    let private getSeconds (f:FO) (o:CO) = (toLocalTime o).Second |> double |> BV.Box
    let private getUTCSeconds (f:FO) (o:CO) = (toUTCTime o).Second |> double |> BV.Box
    let private getMilliseconds (f:FO) (o:CO) = (toLocalTime o).Millisecond |> double |> BV.Box
    let private getUTCMilliseconds (f:FO) (o:CO) = (toUTCTime o).Millisecond |> double |> BV.Box

    let private getTimezoneOffset (f:FO) (o:CO) =
      let o = o.CastTo<DO>()
      (-TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalMinutes) |> BV.Box

    let private setTime (f:FO) (o:CO) (value:double) =
      let o = o.CastTo<DO>()
      o.Date <- value |> DateObject.TicksToDateTime
      o |> BV.Box

    let private setTimeGeneric extract update cont (f:FO) (o:CO) (args:Args) =
      if args.Length > 0 then
        let value = args.[0] |> TC.ToNumber
        let date:DT = o |> extract
        o.CastTo<DO>().Date <- date |> update value
        match cont with
        | None -> o
        | Some cont -> cont f o (Dlr.ArrayUtils.RemoveFirst args)

      else
        o

    type private Set = FO -> CO -> Args -> CO

    let private setMilliseconds:Set = setTimeGeneric toLocalTime DT.setMilliseconds None
    let private setUTCMilliseconds:Set = setTimeGeneric toUTCTime DT.setMilliseconds None
    let private setSeconds:Set = setTimeGeneric toLocalTime DT.setSeconds (Some setMilliseconds)
    let private setUTCSeconds:Set = setTimeGeneric toUTCTime DT.setSeconds (Some setUTCMilliseconds)
    let private setMinutes:Set = setTimeGeneric toLocalTime DT.setMinutes (Some setSeconds)
    let private setUTCMinutes:Set = setTimeGeneric toUTCTime DT.setMinutes (Some setUTCSeconds)
    let private setHours:Set = setTimeGeneric toLocalTime DT.setHours (Some setMinutes)
    let private setUTCHours:Set = setTimeGeneric toUTCTime DT.setHours (Some setUTCMinutes)
    let private setDate:Set = setTimeGeneric toLocalTime DT.setDate (Some setHours)
    let private setUTCDate:Set = setTimeGeneric toUTCTime DT.setDate (Some setUTCHours)
    let private setMonth:Set = setTimeGeneric toLocalTime DT.setMonth (Some setDate)
    let private setUTCMonth:Set = setTimeGeneric toUTCTime DT.setMonth (Some setUTCDate)
    let private setFullYear:Set = setTimeGeneric toLocalTime DT.setYear (Some setMonth)
    let private setUTCFullYear:Set = setTimeGeneric toUTCTime DT.setYear (Some setUTCMonth)

    let create (env:Env) objPrototype =
      let prototype = env.NewDate(invalidDate)
      prototype.Prototype <- objPrototype
      prototype

    type private SetFunc = Func<FO, CO, Args, CO>

    let setup (env:Env) =
      let proto = env.Prototypes.Date

      proto.Put("constructor", env.Constructors.Date, DontEnum)

      let valueOf = Function(valueOf) $ Utils.createFunction env (Some 0)
      proto.Put("valueOf", valueOf, DontEnum)

      let toString = Function(toString) $ Utils.createFunction env (Some 0)
      proto.Put("toString", toString, DontEnum)

      let toDateString = Function(toDateString) $ Utils.createFunction env (Some 0)
      proto.Put("toDateString", toDateString, DontEnum)

      let toTimeString = Function(toTimeString) $ Utils.createFunction env (Some 0)
      proto.Put("toTimeString", toTimeString, DontEnum)

      let toLocaleString = Function(toLocaleString) $ Utils.createFunction env (Some 0)
      proto.Put("toLocaleString", toLocaleString, DontEnum)

      let toLocaleDateString = Function(toLocaleDateString) $ Utils.createFunction env (Some 0)
      proto.Put("toLocaleDateString", toLocaleDateString, DontEnum)

      let toLocaleTimeString = Function(toLocaleTimeString) $ Utils.createFunction env (Some 0)
      proto.Put("toLocaleTimeString", toLocaleTimeString, DontEnum)

      let toUTCString = Function(toUTCString) $ Utils.createFunction env (Some 0)
      proto.Put("toUTCString", toUTCString, DontEnum)

      let getTime = Function(getTime) $ Utils.createFunction env (Some 0)
      proto.Put("getTime", getTime, DontEnum)

      let getFullYear = Function(getFullYear) $ Utils.createFunction env (Some 0)
      proto.Put("getFullYear", getFullYear, DontEnum)

      let getUTCFullYear = Function(getUTCFullYear) $ Utils.createFunction env (Some 0)
      proto.Put("getUTCFullYear", getUTCFullYear, DontEnum)

      let getMonth = Function(getMonth) $ Utils.createFunction env (Some 0)
      proto.Put("getMonth", getMonth, DontEnum)

      let getUTCMonth = Function(getUTCMonth) $ Utils.createFunction env (Some 0)
      proto.Put("getUTCMonth", getUTCMonth, DontEnum)

      let getDate = Function(getDate) $ Utils.createFunction env (Some 0)
      proto.Put("getDate", getDate, DontEnum)

      let getUTCDate = Function(getUTCDate) $ Utils.createFunction env (Some 0)
      proto.Put("getUTCDate", getUTCDate, DontEnum)

      let getDay = Function(getDay) $ Utils.createFunction env (Some 0)
      proto.Put("getDay", getDay, DontEnum)

      let getUTCDay = Function(getUTCDay) $ Utils.createFunction env (Some 0)
      proto.Put("getUTCDay", getUTCDay, DontEnum)

      let getHours = Function(getHours) $ Utils.createFunction env (Some 0)
      proto.Put("getHours", getHours, DontEnum)

      let getUTCHours = Function(getUTCHours) $ Utils.createFunction env (Some 0)
      proto.Put("getUTCHours", getUTCHours, DontEnum)

      let getMinutes = Function(getMinutes) $ Utils.createFunction env (Some 0)
      proto.Put("getMinutes", getMinutes, DontEnum)

      let getUTCMinutes = Function(getUTCMinutes) $ Utils.createFunction env (Some 0)
      proto.Put("getUTCMinutes", getUTCMinutes, DontEnum)

      let getSeconds = Function(getSeconds) $ Utils.createFunction env (Some 0)
      proto.Put("getSeconds", getSeconds, DontEnum)

      let getUTCSeconds = Function(getUTCSeconds) $ Utils.createFunction env (Some 0)
      proto.Put("getUTCSeconds", getUTCSeconds, DontEnum)

      let getMilliseconds = Function(getMilliseconds) $ Utils.createFunction env (Some 0)
      proto.Put("getMilliseconds", getMilliseconds, DontEnum)

      let getUTCMilliseconds = Function(getUTCMilliseconds) $ Utils.createFunction env (Some 0)
      proto.Put("getUTCMilliseconds", getUTCMilliseconds, DontEnum)

      let getTimezoneOffset = Function(getTimezoneOffset) $ Utils.createFunction env (Some 0)
      proto.Put("getTimezoneOffset", getTimezoneOffset, DontEnum)

      let setTime = FunctionReturn<double, BV>(setTime) $ Utils.createFunction env (Some 1)
      proto.Put("setTime", setTime, DontEnum)

      let setMilliseconds = SetFunc(setMilliseconds) $ Utils.createFunction env (Some 1)
      proto.Put("setMilliseconds", setMilliseconds, DontEnum)
 
      let setUTCMilliseconds = SetFunc(setUTCMilliseconds) $ Utils.createFunction env (Some 1)
      proto.Put("setUTCMilliseconds", setUTCMilliseconds, DontEnum)

      let setSeconds = SetFunc(setSeconds) $ Utils.createFunction env (Some 2)
      proto.Put("setSeconds", setSeconds, DontEnum)

      let setUTCSeconds = SetFunc(setUTCSeconds) $ Utils.createFunction env (Some 2)
      proto.Put("setUTCSeconds", setUTCSeconds, DontEnum)

      let setMinutes = SetFunc(setMinutes) $ Utils.createFunction env (Some 3)
      proto.Put("setMinutes", setMinutes, DontEnum)

      let setUTCMinutes = SetFunc(setUTCMinutes) $ Utils.createFunction env (Some 3)
      proto.Put("setUTCMinutes", setUTCMinutes, DontEnum)

      let setHours = SetFunc(setHours) $ Utils.createFunction env (Some 4)
      proto.Put("setHours", setHours, DontEnum)

      let setUTCHours = SetFunc(setUTCHours) $ Utils.createFunction env (Some 4)
      proto.Put("setUTCHours", setUTCHours, DontEnum)

      let setDate = SetFunc(setDate) $ Utils.createFunction env (Some 1)
      proto.Put("setDate", setDate, DontEnum)

      let setUTCDate = SetFunc(setUTCDate) $ Utils.createFunction env (Some 1)
      proto.Put("setUTCDate", setUTCDate, DontEnum)

      let setMonth = SetFunc(setMonth) $ Utils.createFunction env (Some 2)
      proto.Put("setMonth", setMonth, DontEnum)

      let setUTCMonth = SetFunc(setUTCMonth) $ Utils.createFunction env (Some 2)
      proto.Put("setUTCMonth", setUTCMonth, DontEnum)

      let setFullYear = SetFunc(setFullYear) $ Utils.createFunction env (Some 3)
      proto.Put("setFullYear", setFullYear, DontEnum)

      let setUTCFullYear = SetFunc(setUTCFullYear) $ Utils.createFunction env (Some 3)
      proto.Put("setUTCFullYear", setUTCFullYear, DontEnum)
