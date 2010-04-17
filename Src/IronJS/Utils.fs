namespace IronJS

open System

module Utils =
  
  let time fn = 
    let before = DateTime.Now
    fn()
    let result = DateTime.Now - before
    sprintf "%is:%ims" result.Seconds result.Milliseconds
