open System

let startPath = @"C:\Users\fredrikhm\Personal\IronJS\Src\Tests\Sputnik\js"
let outputPath = IO.Directory.GetParent(startPath).FullName + "\\Tests"

let sanitize (txt:string) =
  txt.Replace(".", "_").Replace("\\", "_")

let getClassAndPathName (path:string) =
  let parts = path.Split('\\') |> Array.toList |> List.rev

  let rec buildName (className, pathName) parts = 
    match parts with
    | []
    | "js"::_ -> (className, pathName)
    | directory::parts ->
      let className = (sanitize directory) + "_" + className
      let pathName = directory + "\\" + pathName
      buildName (className, pathName) parts

  let className, pathName = buildName ("", "") parts
  className.Trim('_'), pathName.Trim('\\')

let makeClass = 
  sprintf @"using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class %s : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(%s); }
%s
  }
}"

let makeTest = 
  sprintf "    [TestMethod] public void %s() { RunFile(@\"%s\"); }"

let fileName (filePath:string) = 
  let parts = filePath.Split('\\')
  parts.[parts.Length-1]

let rec buildTests path =
  
  let files = IO.Directory.GetFiles(path)
  let files = files |> Array.filter (fun s -> s.EndsWith(".js"))
  let files = files |> Array.map fileName
  if files.Length > 0 then
    let className, pathName = path |> getClassAndPathName
    let tests = [for file in files -> makeTest (sanitize file) file]
    let tests = tests |> String.concat "\n"
    let tests = makeClass className ("@\"" + pathName + "\"") tests
    IO.File.WriteAllText(outputPath + "\\" + className + ".cs", tests)

  for dir in IO.Directory.GetDirectories(path) do
    buildTests dir

startPath |> buildTests