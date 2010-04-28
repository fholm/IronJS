namespace IronJS.Tools

module InterOp = 

  let toList<'a> (ilst:System.Collections.IList) =
    match ilst with
    | null -> []
    | _    -> let rec convert (lst:System.Collections.IList) n =
                if n = lst.Count 
                  then []
                  else (lst.[n] :?> 'a) :: convert lst (n+1)

              convert ilst 0


