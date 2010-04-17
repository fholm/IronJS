namespace IronJS

open System

module Utils =
  
  let time fn = 
    let before = DateTime.Now
    fn()
    DateTime.Now - before
