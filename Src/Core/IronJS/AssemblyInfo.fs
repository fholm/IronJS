namespace IronJS


open System.Reflection
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

module Version =
  let [<Literal>] Major = 0
  let [<Literal>] Minor = 1
  let [<Literal>] Build = 92
  let [<Literal>] Revision = 1
  let [<Literal>] Tag = "preview"
  let [<Literal>] String = "0.1.92.1"
  let Tupled = Major, Minor, Build, Revision, Tag
  let Tagged = sprintf "%s-%s" String Tag
  let FullName = sprintf "IronJS %s" Tagged
 
[<assembly: AssemblyTitle("IronJS")>]
[<assembly: AssemblyDescription("IronJS - A JavaScript runtime for .NET")>]
[<assembly: AssemblyConfiguration("")>]
[<assembly: AssemblyCompany("IronJS")>]
[<assembly: AssemblyProduct("IronJS")>]
[<assembly: AssemblyCopyright("Copyright © Fredrik Holmström, 2010")>]
[<assembly: AssemblyTrademark("")>]
[<assembly: AssemblyCulture("")>]
 
[<assembly: ComVisible(false)>]
[<assembly: Guid("93e32fd4-5c93-4d0b-91d5-0b4e54f0ce2d")>]

[<assembly: AssemblyVersion(Version.String)>]
[<assembly: AssemblyFileVersion(Version.String)>]

()