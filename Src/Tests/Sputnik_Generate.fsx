open System

let path = @"E:\Projects\IronJS\Src\Tests\sputnik\Conformance\09_Type_Conversion\9.9_ToObject"

let testPath =
  let parts = path.Split('\\')
  let path = parts.[parts.Length-3] + "\\" + parts.[parts.Length-2] + "\\" + parts.[parts.Length-1]

  if Char.IsNumber(path.[0]) 
    then parts.[parts.Length-4] + "\\" + path
    else path 
  
let sanitize (txt:string) =
  txt.Replace(".", "_").Replace("\\", "_")

let testHeader = sanitize testPath

for file in IO.Directory.GetFiles(path) do
  let parts = file.Split('\\');
  let fileName = parts.[parts.Length-1]

  let test = 
    "[TestMethod]\n" +
    "public void "+ testHeader + "_" + (sanitize fileName)+"() {\n" + 
    "Test(@\""+testPath+"\", () => {" +
    "RunFile(\""+fileName+"\");" +
    "});\n" +
    "}\n"

  printfn "%s" test
