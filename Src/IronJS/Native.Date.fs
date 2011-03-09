namespace IronJS.Native

open System
open System.Globalization

open IronJS
open IronJS.Support.Aliases
open IronJS.ExtensionMethods
open IronJS.DescriptorAttrs

module Date =

  module private Formats = 
    let full = "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'zzz"
    //let utc = "ddd, dd MMM yyyy HH':'mm':'ss 'UTC'"
    let date = "ddd, dd MMM yyyy"
    let time = "HH':'mm':'ss 'GMT'zzz"

  type private DT = DateTime
    
  let private ic = invariantCulture :> IFormatProvider
  let private cc = currentCulture:> IFormatProvider

  let private utcZeroDate() = 
    new DateTime(0L, DateTimeKind.Utc)

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
      | 0 -> f.Env.NewDate(DT.Now.ToUniversalTime())
      | 1 -> 
        let value = args.[0].ToPrimitive()

        match value.Tag with
        | TypeTags.String ->
          let value = string value.String
          let mutable date = utcZeroDate()

          if parseDate value ic &date || parseDate value cc &date 
            then f.Env.NewDate(date)
            else f.Env.NewDate(utcZeroDate())

        | _ -> 
          let value = value.ToNumber()
          f.Env.NewDate(value |> DateObject.TicksToDateTime)

      | _ ->
        let mutable date = utcZeroDate()

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
    let ctor = new JsFunc<BV array>(constructor')
    let ctor = Utils.createHostFunction env ctor

    ctor.ConstructorMode <- ConstructorModes.Host
    ctor.Put("prototype", env.Prototypes.Date)

    let parse = new JsFunc<string>(parse)
    let parse = parse |> Utils.createHostFunction env
    ctor.Put("parse", parse, Immutable) 

    let parseLocal = new JsFunc<string>(parseLocal)
    let parseLocal = parseLocal |> Utils.createHostFunction env
    ctor.Put("parseLocal", parseLocal, Immutable) 

    let utc = new JsFunc<BV array>(utc)
    let utc = utc |> Utils.createHostFunction env
    ctor.Put("UTC", utc, Immutable) 

    env.Globals.Put("Date", ctor)
    env.Constructors <- {env.Constructors with Date=ctor}

  module Prototype =

    let private toString (f:FO) (o:CO) =
      let o = o.CastTo<DO>()
      o.Date.ToLocalTime().ToString(Formats.full, ic) |> BV.Box
    
    let create (env:Environment) objPrototype =
      let prototype = env.NewDate(invalidDate)
      prototype.Prototype <- objPrototype
      prototype

    let setup (env:Environment) =
      let proto = env.Prototypes.Date

      proto.Put("constructor", env.Constructors.Date)

      let toString = JsFunc(toString)
      let toString = toString |> Utils.createHostFunction env
      proto.Put("toString", toString)