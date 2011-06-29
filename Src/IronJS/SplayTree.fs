module IronJS.SplayTree

open System
open System.Collections
open System.Collections.Generic
open System.Diagnostics

[<AllowNullLiteral>]
type SplayTreeNode<'TKey, 'TValue> =
    val Key:'TKey
    val mutable Value:'TValue

    [<DefaultValue>]
    val mutable Left:SplayTreeNode<'TKey,'TValue>

    [<DefaultValue>]
    val mutable Right:SplayTreeNode<'TKey,'TValue>

    new(key:'TKey, value:'TValue) = { Key = key; Value = value }

type TiedList<'T, 'TKey, 'TValue when 'TKey :> IComparable<'TKey> and 'TValue : equality>(tree:SplayTree<'TKey, 'TValue>, version:int, backingList:IList<'T>) =
    let tree = tree
    let version = version
    let backingList:IList<'T> = backingList

    interface IEnumerable with
        member this.GetEnumerator() = this.GetEnumerator() :> IEnumerator

    interface ICollection<'T> with
        member this.Add(item:'T) = this.Add(item)
        member this.Clear() = this.Clear()
        member this.Contains(item) = this.Contains(item)
        member this.CopyTo(array, arrayIndex) = this.CopyTo(array, arrayIndex)
        member this.Remove(item:'T) = this.Remove(item)
        member this.GetEnumerator() = this.GetEnumerator()
        member this.Count
            with get() = this.Count
        member this.IsReadOnly
            with get() = this.IsReadOnly

    member private this.CheckVersion() =
        if version <> tree.version then
            raise (new InvalidOperationException("The collection has been modified."))

    member this.Count
        with get() = tree.count

    member this.IsReadOnly
        with get() = true

    member this.Item
        with get(index) =
            this.CheckVersion()
            backingList.[index]
        and set(index) value =
            raise (new NotSupportedException())

    member this.IndexOf(item) =
        this.CheckVersion()
        backingList.IndexOf(item)

    member this.Insert(index:int, item:'T) =
        raise (new NotSupportedException())

    member this.RemoveAt(index:int) =
        raise (new NotSupportedException())

    member this.Add(item:'T) =
        raise (new NotSupportedException())

    member this.Clear() =
        raise (new NotSupportedException())

    member this.Contains(item) =
        this.CheckVersion()
        backingList.Contains(item)

    member this.CopyTo(array, arrayIndex) =
        this.CheckVersion()
        backingList.CopyTo(array, arrayIndex)

    member this.Remove(item:'T) =
        raise (new NotSupportedException())

    member this.GetEnumerator() : IEnumerator<'T> =
        let sequence = seq {
            this.CheckVersion()
            for i in backingList do
                yield i
                this.CheckVersion()
        }
        sequence.GetEnumerator()

and public SplayTree<'TKey, 'TValue when 'TKey :> IComparable<'TKey> and 'TValue : equality>() =
    [<DefaultValue>]
    val mutable root:SplayTreeNode<'TKey,'TValue>

    [<DefaultValue>]
    val mutable count:int

    [<DefaultValue>]
    val mutable version:int

    interface IEnumerable with
        member this.GetEnumerator() = this.GetEnumerator() :> IEnumerator

    interface IDictionary<'TKey, 'TValue> with
        member this.Add(key:'TKey, value:'TValue) = this.Add(key, value)
        member this.Add(item:KeyValuePair<'TKey, 'TValue>) = this.Add(item)
        member this.Clear() = this.Clear()
        member this.ContainsKey(key:'TKey) = this.ContainsKey(key)
        member this.Contains(item:KeyValuePair<'TKey, 'TValue>) = this.Contains(item)
        member this.Remove(key:'TKey) = this.Remove(key)
        member this.Remove(item:KeyValuePair<'TKey, 'TValue>) = this.Remove(item)
        member this.CopyTo(array:KeyValuePair<'TKey, 'TValue>[], arrayIndex:int) = this.CopyTo(array, arrayIndex)
        member this.GetEnumerator() = this.GetEnumerator()
        member this.TryGetValue(key:'TKey, value:byref<'TValue>) =
            let result = this.TryGetValue(key)
            if result.IsSome then
                value <- result.Value
                true
            else
                value <- Unchecked.defaultof<'TValue>
                false
        member this.Item
            with get(key:'TKey) = this.[key]
            and set(key:'TKey) value = this.[key] <- value
        member this.Keys
            with get() = this.Keys
        member this.Values
            with get() = this.Values
        member this.Count
            with get() = this.Count
        member this.IsReadOnly
            with get() = this.IsReadOnly

    member this.Add(key:'TKey, value:'TValue) =
        this.Set(key, value, true)

    member this.Add(item:KeyValuePair<'TKey, 'TValue>) =
        this.Set(item.Key, item.Value, true)

    member private this.Set(key:'TKey, value:'TValue, throwOnExisting:bool) =
        if this.count = 0 then
            this.version <- this.version + 1
            this.root <- new SplayTreeNode<'TKey,'TValue>(key, value)
            this.count <- 1
        else
            this.Splay(key)
            let c = key.CompareTo(this.root.Key)
            if c = 0 then
                if throwOnExisting then
                    raise (new ArgumentException("An item with the same key already exists in the tree."))
                this.version <- this.version + 1
                this.root.Value <- value
            else
                let n = new SplayTreeNode<'TKey,'TValue>(key, value)
                if c < 0 then
                    n.Left <- this.root.Left
                    n.Right <- this.root
                    this.root.Left <- null
                else
                    n.Right <- this.root.Right
                    n.Left <- this.root
                    this.root.Right <- null;
                this.version <- this.version + 1
                this.root <- n
                this.count <- this.count + 1

    member this.Clear() =
        if this.count <> 0 then
            this.root <- null
            this.count <- 0
            this.version <- this.version + 1

    member this.ContainsKey(key:'TKey) =
        if this.count = 0 then
            false
        else
            this.Splay(key)
            key.CompareTo(this.root.Key) = 0

    member this.Contains(item:KeyValuePair<'TKey, 'TValue>) =
        if this.count = 0 then
            false
        else
            this.Splay(item.Key)
            item.Key.CompareTo(this.root.Key) = 0 && (obj.ReferenceEquals(this.root.Value, item.Value) || (not (obj.ReferenceEquals(item.Value, null)) && item.Value.Equals(this.root.Value)))

    member private this.Splay(key:'TKey) =
        let mutable t = this.root
        let mutable header = new SplayTreeNode<'TKey,'TValue>(Unchecked.defaultof<'TKey>, Unchecked.defaultof<'TValue>)
        let mutable l = header
        let mutable r = header

        let mutable go = true
        while go do
            let c = key.CompareTo(t.Key)
            if c < 0 then
                if t.Left = null then
                    go <- false
                else
                    if key.CompareTo(t.Left.Key) < 0 then
                        let temp = t.Left
                        t.Left <- temp.Right
                        temp.Right <- t
                        t <- temp
                        if t.Left = null then
                            go <- false
                    if go then
                        r.Left <- t
                        r <- t
                        t <- t.Left
            elif c > 0 then
                if t.Right = null then
                    go <- false
                else
                    if key.CompareTo(t.Right.Key) > 0 then
                        let temp = t.Right
                        t.Right <- temp.Left
                        temp.Left <- t
                        t <- temp
                        if t.Right = null then
                            go <- false
                    if go then
                        l.Right <- t
                        l <- t
                        t <- t.Right
            else
                go <- false
        l.Right <- t.Left
        r.Left <- t.Right
        t.Left <- header.Right
        t.Right <- header.Left
        this.root <- t

    member this.Remove(key:'TKey) : bool =
        if this.count = 0 then
            false
        else
            this.Splay(key)
            if key.CompareTo(this.root.Key) <> 0 then
                false
            else
                if this.root.Left = null then
                    this.root <- this.root.Right
                else
                    let swap = this.root.Right
                    this.root <- this.root.Left
                    this.Splay(key)
                    this.root.Right <- swap
                this.version <- this.version + 1
                this.count <- this.count - 1
                true

    member this.Remove(item:KeyValuePair<'TKey, 'TValue>) : bool =
        if this.count = 0 then
            false
        else
            this.Splay(item.Key)
            if item.Key.CompareTo(this.root.Key) = 0 && (obj.ReferenceEquals(this.root.Value, item.Value) || (not (obj.ReferenceEquals(item.Value, null)) && item.Value.Equals(this.root.Value))) then
                false
            else
                if this.root.Left = null then
                    this.root <- this.root.Right
                else
                    let swap = this.root.Right
                    this.root <- this.root.Left
                    this.Splay(item.Key)
                    this.root.Right <- swap
                this.version <- this.version + 1
                this.count <- this.count - 1
                true

    member this.TryGetValue(key:'TKey) =
        if this.count = 0 then
            None
        else
            this.Splay(key)
            if key.CompareTo(this.root.Key) <> 0 then
                None
            else
                Some(this.root.Value)

    member this.Item
        with get(key:'TKey) =
            if this.count = 0 then
                raise (new KeyNotFoundException("The key was not found in the tree."))
            else
                this.Splay(key)
                if key.CompareTo(this.root.Key) <> 0 then
                    raise (new KeyNotFoundException("The key was not found in the tree."))
                else
                    this.root.Value
        and set(key:'TKey) value =
            this.Set(key, value, false)

    member this.Count
        with get() = this.count

    member this.IsReadOnly
        with get() = false

    member this.Trim(depth:int) =
        if depth < 0 then
            raise (new ArgumentOutOfRangeException("depth", "The trim depth must not be negative."))
        elif depth = 0 then
            this.Clear()
        elif this.count <> 0 then
            let prevCount = this.count
            this.count <- this.Trim(this.root, depth - 1)
            if prevCount <> this.count then
                this.version <- this.version + 1

    member private this.Trim(node:SplayTreeNode<'TKey, 'TValue>, depth:int) =
        if depth = 0 then
            node.Left <- null
            node.Right <- null
            1
        else
            let mutable count = 1
            if node.Left <> null then
                count <- count + this.Trim(node.Left, depth - 1)
            if node.Right <> null then
                count <- count + this.Trim(node.Right, depth - 1)
            count

    member this.CopyTo(array:KeyValuePair<'TKey, 'TValue>[], arrayIndex:int) =
        this.AsList(fun node -> new KeyValuePair<'TKey, 'TValue>(node.Key, node.Value)).CopyTo(array, arrayIndex)

    member private this.AsList<'TEnumerator>(selector:SplayTreeNode<'TKey, 'TValue>->'TEnumerator) : IList<'TEnumerator> =
        let result = new List<'TEnumerator>(this.count)
        if this.root <> null then
            this.PopulateList(this.root, result, selector)
        result :> IList<'TEnumerator>

    member private this.PopulateList(node:SplayTreeNode<'TKey, 'TValue>, list:List<'TEnumerator>, selector:SplayTreeNode<'TKey, 'TValue>->'TEnumerator) =
        if node.Left <> null then this.PopulateList(node.Left, list, selector)
        list.Add(selector(node))
        if node.Right <> null then this.PopulateList(node.Right, list, selector)

    member this.GetEnumerator() : IEnumerator<KeyValuePair<'TKey, 'TValue>> =
        let backingList = this.AsList(fun node -> new KeyValuePair<'TKey, 'TValue>(node.Key, node.Value));
        let tiedList = new TiedList<KeyValuePair<'TKey, 'TValue>, 'TKey, 'TValue>(this, this.version, backingList)
        tiedList.GetEnumerator()

    member this.Keys
        with get() : ICollection<'TKey> = new TiedList<'TKey, 'TKey, 'TValue>(this, this.version, this.AsList(fun node -> node.Key)) :> ICollection<'TKey>

    member this.Values
        with get() : ICollection<'TValue> = new TiedList<'TValue, 'TKey, 'TValue>(this, this.version, this.AsList(fun node -> node.Value)) :> ICollection<'TValue>
