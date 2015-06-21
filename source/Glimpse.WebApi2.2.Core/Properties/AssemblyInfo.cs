using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Glimpse.Core.Extensibility;


[assembly: ComVisible(false)]
[assembly: Guid("f06665e7-b8c8-4804-98ef-74f2c699de9e")]

[assembly: AssemblyTitle("Glimpse for ASP.NET Web Api 2.2 Assembly")]
[assembly: AssemblyDescription("Glimpse extensions and tabs for ASP.NET Web Api 2.2.")]
[assembly: AssemblyProduct("WebApi2.2.Core")]
[assembly: AssemblyCopyright("© 2013 Nik Molnar & Anthony van der Hoorn")]
[assembly: AssemblyTrademark("Glimpse™")]

// Version is in major.minor.build format to support http://semver.org/
// Keep these three attributes in sync
[assembly: AssemblyVersion("1.0.0")]
[assembly: AssemblyFileVersion("1.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0")] // Used to specify the NuGet version number at build time

//[assembly: InternalsVisibleTo("Glimpse.Test.WebApi2.2.Core")]
[assembly: NuGetPackage("Glimpse.WebApi2.2.Core")]