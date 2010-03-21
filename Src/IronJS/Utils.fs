module IronJS.Utils

let toList<'a> (ilst:System.Collections.IList) =
  let mutable lst = []
  let cnt = ilst.Count - 1
  for n in 0 .. cnt do 
    lst <- ilst.[cnt - n] :: lst
  lst