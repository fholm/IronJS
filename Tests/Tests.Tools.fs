namespace IronJS.Tests

module Tools =
  
  let test name test =
    try 
      test (IronJS.Hosting.Context.Create())
      printfn "%s: PASSED" name

    with
    | ex -> printfn "%s: FAILED (%s)" name ex.Message