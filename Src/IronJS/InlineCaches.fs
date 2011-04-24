namespace IronJS.Runtime.Optimizations

open System
open IronJS

///
type InlinePropertyGetCache(env:Env, throwOnMissing:bool) =
  
  [<DefaultValue>] val mutable CachedId : uint64
  [<DefaultValue>] val mutable CachedIndex : int32
  
  ///
  member x.Get(co:CO, name:string) =
    let mutable index = 0

    if co.PropertySchema.IndexMap.TryGetValue(name, &index) && co.Properties.[index].HasValue then
      if co.PropertySchema.Id > 1UL then
        x.CachedId <- co.PropertySchema.Id
        x.CachedIndex <- index

      co.Properties.[index].Value

    else
      let descriptor = co.Find(name)

      if descriptor.HasValue then 
        descriptor.Value

      elif throwOnMissing then 
        env.RaiseReferenceError(sprintf "%s is not defined" name)

      else 
        Undefined.Boxed

  ///
  member x.Get(bv:BV, name:string) : BV =
    match bv.Tag with
    | TypeTags.Function
    | TypeTags.Object -> x.Get(bv.Object, name)
    | _ -> TC.ToObject(env, bv).Get(name)

///
type InlinePropertyPutCache() =
  
  [<DefaultValue>] val mutable CachedId : uint64
  [<DefaultValue>] val mutable CachedIndex : int32

  member private x.UpdateCache(co:CO, index:int32) =
    if not(co.PropertySchema :? DynamicSchema) then
      x.CachedId <- co.PropertySchema.Id
      x.CachedIndex <- index

  member x.Put(co:CO, name:string, value:BV) : unit =
    let mutable index = 0
    if co.CanPut(name, &index) then
      co.Properties.[index].Value <- value
      co.Properties.[index].HasValue <- true
      x.UpdateCache(co, index)

  member x.Put(co:CO, name:string, value:obj, tag:uint32) : unit =
    let mutable index = 0
    if co.CanPut(name, &index) then
      co.Properties.[index].Value.Clr <- value
      co.Properties.[index].Value.Tag <- tag
      co.Properties.[index].HasValue <- true
      x.UpdateCache(co, index)

  member x.Put(co:CO, name:string, value:double) : unit =
    let mutable index = 0
    if co.CanPut(name, &index) then
      co.Properties.[index].Value.Number <- value
      co.Properties.[index].HasValue <- true
      x.UpdateCache(co, index)
    
///
type InlineInvokeCache(env:Env) =
  
  [<DefaultValue>] val mutable CachedId : uint64
  [<DefaultValue>] val mutable CachedDelegate : Func<FO, CO, BV>

  ///
  member x.Invoke(func:FO, this:CO) =
    x.CachedId <- func.MetaData.Id
    x.CachedDelegate <- func.MetaData.GetDelegate<Func<FO, CO, BV>>(func) 
    x.CachedDelegate.Invoke(func, this)

///
type InlineInvokeCache<'a>(env:Env) =
  
  [<DefaultValue>] val mutable CachedId : uint64
  [<DefaultValue>] val mutable CachedDelegate : Func<FO, CO, 'a, BV>

  ///
  member x.Invoke(func:FO, this:CO, a) =
    x.CachedId <- func.MetaData.Id
    x.CachedDelegate <- func.MetaData.GetDelegate<Func<FO, CO, 'a, BV>>(func) 
    x.CachedDelegate.Invoke(func, this, a)

///
type InlineInvokeCache<'a, 'b>(env:Env) =
  
  [<DefaultValue>] val mutable CachedId : uint64
  [<DefaultValue>] val mutable CachedDelegate : Func<FO, CO, 'a, 'b, BV>

  ///
  member x.Invoke(func:FO, this:CO, a, b) =
    x.CachedId <- func.MetaData.Id
    x.CachedDelegate <- func.MetaData.GetDelegate<Func<FO, CO, 'a, 'b, BV>>(func) 
    x.CachedDelegate.Invoke(func, this, a, b)

///
type InlineInvokeCache<'a, 'b, 'c>(env:Env) =
  
  [<DefaultValue>] val mutable CachedId : uint64
  [<DefaultValue>] val mutable CachedDelegate : Func<FO, CO, 'a, 'b, 'c, BV>

  ///
  member x.Invoke(func:FO, this:CO, a, b, c) =
    x.CachedId <- func.MetaData.Id
    x.CachedDelegate <- func.MetaData.GetDelegate<Func<FO, CO, 'a, 'b, 'c, BV>>(func) 
    x.CachedDelegate.Invoke(func, this, a, b, c)

///
type InlineInvokeCache<'a, 'b, 'c, 'd>(env:Env) =
  
  [<DefaultValue>] val mutable CachedId : uint64
  [<DefaultValue>] val mutable CachedDelegate : Func<FO, CO, 'a, 'b, 'c, 'd, BV>

  ///
  member x.Invoke(func:FO, this:CO, a, b, c, d) =
    x.CachedId <- func.MetaData.Id
    x.CachedDelegate <- func.MetaData.GetDelegate<Func<FO, CO, 'a, 'b, 'c, 'd, BV>>(func) 
    x.CachedDelegate.Invoke(func, this, a, b, c, d)

///
type InlineVariadicInvokeCache(env:Env) =
  
  [<DefaultValue>] val mutable CachedId : uint64
  [<DefaultValue>] val mutable CachedDelegate : VariadicFunction

  ///
  member x.Invoke(func:FO, this:CO, args) =
    x.CachedId <- func.MetaData.Id
    x.CachedDelegate <- func.MetaData.GetDelegate<VariadicFunction>(func) 
    x.CachedDelegate.Invoke(func, this, args)

///
module Utils =
  
  ///
  let getInvokeInlineCache (types:Type array) =
    match types.Length with
    | 0 -> typeof<InlineInvokeCache>
    | 1 -> typedefof<InlineInvokeCache<_>>.MakeGenericType(types)
    | 2 -> typedefof<InlineInvokeCache<_, _>>.MakeGenericType(types)
    | 3 -> typedefof<InlineInvokeCache<_, _, _>>.MakeGenericType(types)
    | 4 -> typedefof<InlineInvokeCache<_, _, _, _>>.MakeGenericType(types)
    | _ -> typeof<InlineVariadicInvokeCache>