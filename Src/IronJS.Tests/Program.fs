//Unit Tests
IronJS.Tests.Runner.run "function.js" true
IronJS.Tests.Runner.run "with.js" true
IronJS.Tests.Runner.run "exception.js" true
IronJS.Tests.Runner.run "break.js" true
IronJS.Tests.Runner.run "eval.js" true

//Wait
System.Console.WriteLine("Press [Enter]");
System.Console.ReadLine() |> ignore