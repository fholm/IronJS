namespace IronJS.Native

open System
open IronJS
open IronJS.Api.Extensions

//------------------------------------------------------------------------------
// 15.4
module Array =

  //----------------------------------------------------------------------------
  // 15.4.2
  let constructor' (f:IjsFunc) (_:IjsObj) (args:IjsBox array) : IjsObj =
    if args.Length = 1 then
      let number = Api.TypeConverter.toNumber args.[0]
      let size = Api.TypeConverter.toUInt32 number
      Api.Environment.createArray f.Env size

    else
      let size = args.Length |> uint32
      let array = Api.Environment.createArray f.Env size
      
      Array.iteri (fun i value -> 
        array.Methods.PutBoxIndex.Invoke(array, uint32 i, value)) args

      array
      
  //----------------------------------------------------------------------------
  let setupConstructor (env:IjsEnv) =
    let ctor = 
      (Api.HostFunction.create 
        env (new Func<IjsFunc, IjsObj, IjsBox array, IjsObj>(constructor')))

    ctor.ConstructorMode <- ConstructorModes.Host
    ctor.put("prototype", env.Array_prototype)
    env.Globals.put("Array", ctor)
    
  //----------------------------------------------------------------------------
  let createPrototype (env:IjsEnv) =
    let prototype = Api.Environment.createArray env 0u
    Api.Object.Property.setMap prototype env.Array_Class
    prototype.Prototype <- env.Object_prototype
    prototype.Class <- Classes.Array
    prototype
    
  //----------------------------------------------------------------------------
  let setupPrototype (env:IjsEnv) =
    env.Array_prototype.put("constructor", 
      env.Globals.get("Array"))

