namespace IronJS.Native

open System
open System.Globalization

open IronJS
open IronJS.Support.Aliases

module Date =

  module private Formats = 
    let full = "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'zzz"
    let utc = "ddd, dd MMM yyyy HH':'mm':'ss 'UTC'"
    let date = "ddd, dd MMM yyyy"
    let time = "HH':'mm':'ss 'GMT'zzz"

  let private utcZeroDate() = 
    new DateTime(0L, DateTimeKind.Utc)

  let private parseDate input (result:DateTime byref) =
    let none = DateTimeStyles.None
    let ic = invariantCulture

    if DateTime.TryParse(input, ic, none, &result) |> not then
      if DateTime.TryParseExact(input, Formats.full, ic, none, &result) |> not then
        let mutable temp = utcZeroDate()

        if DateTime.TryParseExact(input, Formats.date, ic, none, &temp) then
          result <- result.AddTicks(temp.Ticks)

        if DateTime.TryParseExact(input, Formats.time, ic, none, &temp) then
          result <- result.AddTicks(temp.Ticks)

        if temp.Ticks > 0L then 
          true

        else
          result <- utcZeroDate()
          false

      else
        true

    else
      true
  
  let private constructor' (f:FO) (o:CO) (args:BV array) =
    let date = 
      match args.Length with
      | 0 -> f.Env.NewDate(DateTime.Now.ToUniversalTime())
      | 1 -> 
        let value = args.[0] |> TypeConverter.ToPrimitive

        match value.Tag with
        | TypeTags.String ->
          let value = string value.String
          let mutable date = utcZeroDate()

          if parseDate value &date 
            then f.Env.NewDate(date)
            else f.Env.NewDate(utcZeroDate())

        | _ -> 
          let value = value |> TypeConverter.ToNumber
          f.Env.NewDate(value |> DateObject.TicksToDateTime)

      | _ ->
        ()
      
    date |> BV.Box
