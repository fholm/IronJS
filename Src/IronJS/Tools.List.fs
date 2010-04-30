namespace IronJS.Tools


module List = 

  let splitOn fn lst = 
    let split, body, tail =
      List.fold (fun (if', body, tail) x -> 
        match if' with
        | None -> 
          if fn x
            then Some(x), body, tail
            else None, x::body, tail
        | Some(_) -> if', body, x::tail
      ) (None, [], []) lst

    split, List.rev body, List.rev tail
