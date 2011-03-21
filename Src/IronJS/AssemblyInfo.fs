namespace IronJS


open System.Reflection
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

module Version =
  let [<Literal>] Major = 0
  let [<Literal>] Minor = 2
  let [<Literal>] Build = 0
  let [<Literal>] Revision = 0
  let [<Literal>] String = "0.2.0.0"
  let Tupled = Major, Minor, Build, Revision
  let FullName = sprintf "IronJS %s" String
 
[<assembly: AssemblyTitle("IronJS")>]
[<assembly: AssemblyDescription("IronJS - A JavaScript runtime for .NET")>]
[<assembly: AssemblyConfiguration("")>]
[<assembly: AssemblyCompany("IronJS")>]
[<assembly: AssemblyProduct("IronJS")>]
[<assembly: AssemblyCopyright("Copyright © Fredrik Holmström, 2010-2011")>]
[<assembly: AssemblyTrademark("")>]
[<assembly: AssemblyCulture("")>]
 
[<assembly: ComVisible(false)>]
[<assembly: Guid("93e32fd4-5c93-4d0b-91d5-0b4e54f0ce2d")>]

[<assembly: AssemblyVersion(Version.String)>]
[<assembly: AssemblyFileVersion(Version.String)>]

()