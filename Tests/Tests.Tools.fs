namespace TestingTools

module Testing =

  let createTesters init =
    let failed = ref 0
    let passed = ref 0

    let clean () =
      failed := 0
      passed := 0

    let report () =
      printfn "PASSED: %i / FAILED: %i" !passed !failed

    let state () =
      !passed, !failed

    let test name f =
      try 
        f (init())
        incr passed
        printfn "%s: PASSED" name

      with
      | ex -> 
        incr failed

        let matches = 
          let pattern = @":line (\d+)"
          System.Text.RegularExpressions.Regex.Matches(ex.StackTrace, pattern)

        let line = 
          match matches.Count with
          | 0 -> "<unknown>" 
          | 1 -> matches.[0].Groups.[1].Value
          | _ -> matches.[1].Groups.[1].Value

        printfn "%s: FAILED (%s) LINE: %s " name ex.Message line

    test, clean, state, report

module Assert =

  //Make sure to reference
  //Microsoft.VisualStudio.QualityTools.UnitTestFramework
  open Microsoft.VisualStudio.TestTools.UnitTesting

  let equal a b = Assert.AreEqual(a, b)
  let equalM a b (m:string) = Assert.AreEqual(a, b, m)

  let notEqual a b = Assert.AreNotEqual(a, b)
  let notEqualM a b (m:string) = Assert.AreNotEqual(a, b, m)

  let same a b = Assert.AreSame(a, b)
  let sameM a b (m:string) = Assert.AreSame(a, b, m)

  let notSame a b = Assert.AreNotSame(a, b)
  let notSameM a b (m:string) = Assert.AreNotSame(a, b, m)

  let is a b = Assert.IsInstanceOfType(a, b)
  let isM a b (m:string) = Assert.IsInstanceOfType(a, b, m)

  let isNot a b = Assert.IsNotInstanceOfType(a, b)
  let isNotM a b (m:string) = Assert.IsNotInstanceOfType(a, b, m)

  let isT<'a> v = is v typeof<'a>
  let isTM<'a> v m = isM v typeof<'a> m

  let isNotT<'a> v = isNot v typeof<'a>
  let isNotTM<'a> v m = isNotM v typeof<'a> m

  let isTrue v = Assert.IsTrue v
  let isTrueM v (m:string) = Assert.IsTrue(v, m)

  let isFalse v = Assert.IsFalse v
  let isFalseM v (m:string) = Assert.IsFalse(v, m)

  let isNull v = Assert.IsNull v
  let isNullM v m = Assert.IsNull(v, m)

  let isNotNull v = Assert.IsNotNull v
  let isNotNullM v m = Assert.IsNotNull(v, m)

  let fail m = Assert.Fail m
  let inconclusive m = Assert.Inconclusive m