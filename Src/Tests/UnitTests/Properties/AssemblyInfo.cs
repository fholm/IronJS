using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NUnit.Framework;

[assembly: AssemblyTitle("IronJS.Tests.UnitTests")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Fredrik Holmström")]
[assembly: AssemblyProduct("IronJS.Tests.UnitTests")]
[assembly: AssemblyCopyright("Copyright © Fredrik Holmström, 2010-2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: Guid("5d960940-23f3-487b-bbe7-01d291a39bf5")]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

#if !DEBUG
[assembly: Timeout(180000)]  // Tests that run over two minutes are cancelled.
#endif