namespace IronJS.Native

open System
open System.Globalization

open IronJS
open IronJS.Support.Aliases
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
  
  let private constructor' (f:FO) (o:CO) (args:BV array) =
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

  let private utc (f:FO) (o:CO) (args:BV array) =
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
    let create a = Utils.createHostFunction env a
    let ctor = new JsFunc<BV array>(constructor') |> create

    ctor.ConstructorMode <- ConstructorModes.Host
    ctor?prototype <- env.Prototypes.Date
    ctor?parse <- (JsFunc<string>(parse) |> create)
    ctor?parseLocal <- (JsFunc<string>(parseLocal) |> create)
    ctor?UTC <- (JsFunc<BV array>(utc) |> create)

    env.Globals?Date <- ctor
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
    let private valueOf (f:FO) (o:CO) =
      o.CastTo<DO>().Date
      |> DateObject.DateTimeToTicks 
      |> float 
      |> BV.Box

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
    type private SetFunc = Func<FO, CO, Args, CO>

    let private setMilliseconds = setTimeGeneric toLocalTime DT.setMilliseconds None
    let private setUTCMilliseconds = setTimeGeneric toUTCTime DT.setMilliseconds None
    let private setSeconds = setTimeGeneric toLocalTime DT.setSeconds (Some setMilliseconds)
    let private setUTCSeconds = setTimeGeneric toUTCTime DT.setSeconds (Some setUTCMilliseconds)
    let private setMinutes = setTimeGeneric toLocalTime DT.setMinutes (Some setSeconds)
    let private setUTCMinutes = setTimeGeneric toUTCTime DT.setMinutes (Some setUTCSeconds)
    let private setHours = setTimeGeneric toLocalTime DT.setHours (Some setMinutes)
    let private setUTCHours = setTimeGeneric toUTCTime DT.setHours (Some setUTCMinutes)
    let private setDate = setTimeGeneric toLocalTime DT.setDate (Some setHours)
    let private setUTCDate = setTimeGeneric toUTCTime DT.setDate (Some setUTCHours)
    let private setMonth = setTimeGeneric toLocalTime DT.setMonth (Some setDate)
    let private setUTCMonth = setTimeGeneric toUTCTime DT.setMonth (Some setUTCDate)
    let private setFullYear:Set = setTimeGeneric toLocalTime DT.setYear (Some setMonth)
    let private setUTCFullYear:Set = setTimeGeneric toUTCTime DT.setYear (Some setUTCMonth)

    let create (env:Environment) objPrototype =
      let prototype = env.NewDate(invalidDate)
      prototype.Prototype <- objPrototype
      prototype

    let setup (env:Environment) =
      let proto = env.Prototypes.Date
      let create func = func |> Utils.createHostFunction env

      proto?constructor <- env.Constructors.Date
      proto?valueOf <- (JsFunc(valueOf) |> create)
      proto?toString <- (JsFunc(toString) |> create)
      proto?toDateString <- (JsFunc(toDateString) |> create)
      proto?toTimeString <- (JsFunc(toTimeString) |> create)
      proto?toLocaleString <- (JsFunc(toLocaleString) |> create)
      proto?toLocaleDateString <- (JsFunc(toLocaleDateString) |> create)
      proto?toLocaleTimeString <- (JsFunc(toLocaleTimeString) |> create)
      proto?getTime <- (JsFunc(getTime) |> create)
      proto?getFullYear <- (JsFunc(getFullYear) |> create)
      proto?getUTCFullYear <- (JsFunc(getUTCFullYear) |> create)
      proto?getMonth <- (JsFunc(getMonth) |> create)
      proto?getUTCMonth <- (JsFunc(getUTCMonth) |> create)
      proto?getDate <- (JsFunc(getDate) |> create)
      proto?getUTCDate <- (JsFunc(getUTCDate) |> create)
      proto?getDay <- (JsFunc(getDay) |> create)
      proto?getUTCDay <- (JsFunc(getUTCDay) |> create)
      proto?getHours <- (JsFunc(getHours) |> create)
      proto?getUTCHours <- (JsFunc(getUTCHours) |> create)
      proto?getMinutes <- (JsFunc(getMinutes) |> create)
      proto?getUTCMinutes <- (JsFunc(getUTCMinutes) |> create)
      proto?getSeconds <- (JsFunc(getSeconds) |> create)
      proto?getUTCSeconds <- (JsFunc(getUTCSeconds) |> create)
      proto?getMilliseconds <- (JsFunc(getMilliseconds) |> create)
      proto?getUTCMilliseconds <- (JsFunc(getUTCMilliseconds) |> create)
      proto?getTimezoneOffset <- (JsFunc(getTimezoneOffset) |> create)
      proto?setTime <- (JsFunc<double>(setTime) |> create)

      proto?setMilliseconds <- (SetFunc(setMilliseconds) |> create) 
      proto?setUTCMilliseconds <- (SetFunc(setUTCMilliseconds) |> create)
      proto?setSeconds <- (SetFunc(setSeconds) |> create)
      proto?setUTCSeconds <- (SetFunc(setUTCSeconds) |> create)
      proto?setMinutes <- (SetFunc(setMinutes) |> create)
      proto?setUTCMinutes <- (SetFunc(setUTCMinutes) |> create)
      proto?setHours <- (SetFunc(setHours) |> create)
      proto?setUTCHours <- (SetFunc(setUTCHours) |> create)
      proto?setDate <- (SetFunc(setDate) |> create)
      proto?setUTCDate <- (SetFunc(setUTCDate) |> create)
      proto?setMonth <- (SetFunc(setMonth) |> create)
      proto?setUTCMonth <- (SetFunc(setUTCMonth) |> create)
      proto?setFullYear <- (SetFunc(setFullYear) |> create)
      proto?setUTCFullYear <- (SetFunc(setUTCFullYear) |> create)