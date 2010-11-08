namespace IronJS.Native

open System.Globalization

//System.DateTime.Parse("2010") //FAIL
//System.DateTime.Parse("2010-03") //OK
//System.DateTime.Parse("2010-03-07") //OK
//System.DateTime.Parse("2010T12:34") //FAIL
//System.DateTime.Parse("2010-02T12:34:56") //FAIL
//System.DateTime.Parse("2010-02-07T12:34:56.012") //OK
//System.DateTime.Parse("2010T12:34Z") //FAIL
//System.DateTime.Parse("2010-02T12:34:56Z") //FAIL
//System.DateTime.Parse("2010-02-07T12:34:56.012Z") //OK
//System.DateTime.Parse("2010T12:34+09:00") //FAIL
//System.DateTime.Parse("2010-02-07T12:34:56.012-09:00") //OK
//System.DateTime.Parse("2010-02-05T12:34:56.012") //OK

module Date =

  ()