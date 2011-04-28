namespace IronJS.Caches

open System
open IronJS.Support.Aliases
open IronJS.SplayTree

///
type LimitCache<'k, 'v when 'k : equality>(size:int) =
  let storage = ref List.empty<'k * 'v>

  member x.Lookup(key:'k, value:Lazy<'v>) =
    match !storage |> List.tryFind (fun (k, _) -> k = key) with
    | None ->
      storage := (key, value.Value) :: !storage

      if (!storage).Length > size * 2 then
        storage := !storage |> Seq.take size |> Seq.toList

      value.Value

    | Some(_, cached) -> 
      cached

///
type SplayCache<'k, 'v when 'k :> IComparable<'k> and 'v : equality>() =
  let storage = new SplayTree<'k, 'v>()

  member x.Lookup(key:'k, value:Lazy<'v>) =
      let result = storage.TryGetValue(key)
      if result.IsNone then
        let result = value.Value
        storage.[key] <- result
        if storage.Count > 1024 then
          storage.Trim(10)
        result
      else
         result.Value

///
type WeakCache<'TKey, 'TValue when 'TKey : equality>() =
  let cache = new System.Collections.Generic.Dictionary<'TKey, WeakReference>()

  member x.Lookup (key:'TKey, create:Lazy<'TValue>) =
    let cacheHit, reference = cache.TryGetValue(key)
    if cacheHit then
      match reference.Target with
      | :? 'TValue as value -> value
      | _ ->
        let value = create.Value
        let reference = new WeakReference(value)
        cache.[key] <- reference
        value
    else
      let value = create.Value
      let reference = new WeakReference(value)
      cache.Add(key, reference)
      value
