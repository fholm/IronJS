namespace IronJS.Native

open System
open IronJS
open IronJS.Runtime
open IronJS.Support.CustomOperators

///
module internal Object =
  
  ///
  let private constructor' (func:FO) (this:CO) (value:BV) =
    match value.Tag with
    | TypeTags.Undefined -> func.Env.NewObject()
    | TypeTags.Clr -> func.Env.NewObject()
    | _ -> TC.ToObject(func.Env, value)

  ///
  let setup (env:Env) =
    let ctor = Func<FO, CO, BV, CO>(constructor')
    let ctor = ctor $ Utils.createConstructor env (Some 1)

    ctor.MetaData.Name <- "Object"
    ctor.Put("prototype", env.Prototypes.Object, DescriptorAttrs.Immutable)

    env.Globals.Put("Object", ctor, DescriptorAttrs.DontEnum)
    env.Constructors.Object <- ctor
      
  ///
  module Prototype = 

    ///
    let private toString (_:FO) (this:CO) = 
      sprintf "[object %s]" this.ClassName

    ///
    let private toLocaleString = toString

    ///
    let private valueOf (_:FO) (this:CO) = this

    ///
    let private hasOwnProperty (_:FO) (this:CO) (name:string) =
      let mutable index = 0

      if this.PropertySchema.IndexMap.TryGetValue(name, &index) then 
        this.Properties.[index].HasValue

      elif this :? AO && name.Length > 0 && FSharp.Char.isDigit name.[0] then
        let mutable ai = 0u
        let mutable ao = this :?> AO
        UInt32.TryParse(name, &ai) && ao.HasIndex(ai)

      else
        false

    ///
    let private isPrototypeOf (_:FO) (this:CO) (v:CO) = 

      let rec isPrototypeOf (o:CO) (v:CO) =
        if v $ FSharp.Utils.isNull
          then false
          elif o == v.Prototype
            then true
            else isPrototypeOf o v.Prototype

      isPrototypeOf this v

    ///
    let private propertyIsEnumerable (_:FO) (this:CO) (name:string) =
      let mutable index = 0

      if this.PropertySchema.IndexMap.TryGetValue(name, &index) then
        let descriptor = this.Properties.[index]
        descriptor.HasValue && descriptor.IsEnumerable

      else 
        false
  
    ///    
    let create (env:Env) =
      CO(env, env.Maps.Base, null)

    ///
    let setup (env:Env) =
      //
      let proto = env.Prototypes.Object
      proto.Put("constructor", env.Constructors.Object, DescriptorAttrs.DontEnum)

      //
      let toString = toString $ Utils.createFunc0 env (Some 0)
      proto.Put("toString", toString, DescriptorAttrs.DontEnum)

      //
      let toLocaleString = toLocaleString $ Utils.createFunc0 env (Some 0)
      proto.Put("toLocaleString", toLocaleString, DescriptorAttrs.DontEnum)

      //
      let valueOf = valueOf $ Utils.createFunc0 env (Some 0)
      proto.Put("valueOf", valueOf, DescriptorAttrs.DontEnum)

      //
      let hasOwnProperty = hasOwnProperty $ Utils.createFunc1 env (Some 1)
      proto.Put("hasOwnProperty", hasOwnProperty, DescriptorAttrs.DontEnum)
    
      //
      let isPrototypeOf = isPrototypeOf $ Utils.createFunc1 env (Some 1)
      proto.Put("isPrototypeOf", isPrototypeOf, DescriptorAttrs.DontEnum)

      //
      let isNumerable = propertyIsEnumerable $ Utils.createFunc1 env (Some 1)
      proto.Put("propertyIsEnumerable", isNumerable, DescriptorAttrs.DontEnum)
