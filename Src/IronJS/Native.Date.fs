namespace IronJS.Native

open System
open System.Globalization

open IronJS
open IronJS.Support.Aliases
open IronJS.ExtensionMethods

module Date =

  module private Formats = 
    let full = "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'zzz"
    //let utc = "ddd, dd MMM yyyy HH':'mm':'ss 'UTC'"
    let date = "ddd, dd MMM yyyy"
    let time = "HH':'mm':'ss 'GMT'zzz"

  let private utcZeroDate() = 
    new DateTime(0L, DateTimeKind.Utc)

  let private invalidDate = 
    DateTime.MinValue

  let private parseDate input culture (result:DateTime byref) : bool =   
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
      | 0 -> f.Env.NewDate(DateTime.Now.ToUniversalTime())
      | 1 -> 
        let value = args.[0].ToPrimitive()

        match value.Tag with
        | TypeTags.String ->
          let value = string value.String
          let mutable date = utcZeroDate()
          
          let ic = invariantCulture :> IFormatProvider
          let cc = currentCulture:> IFormatProvider

          if parseDate value ic &date || parseDate value cc &date 
            then f.Env.NewDate(date)
            else f.Env.NewDate(utcZeroDate())

        | _ -> 
          let value = value |> TypeConverter.ToNumber
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
    | null -> date.CallMember("toString")
    | _ -> date |> BV.Box
