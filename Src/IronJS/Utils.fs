namespace IronJS

open System

module Utils =

  let time fn = 
    let before = DateTime.Now
    fn()
    let result = DateTime.Now - before
    sprintf "%i:%i" result.Seconds result.Milliseconds