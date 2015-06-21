using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Glimpse.Core.Extensibility;


[assembly: ComVisible(false)]
[assembly: Guid("86966d77-164d-475c-ab15-f9c89ccd81c2")]

[assembly: AssemblyTitle("Glimpse for ASP.NET Web Api 2.1 Assembly")]
[assembly: AssemblyDescription("Glimpse extensions and tabs for ASP.NET Web Api 2.1.")]
[assembly: AssemblyProduct("WebApi2.1.WebHost")]
[assembly: AssemblyCopyright("© 2013 Nik Molnar & Anthony van der Hoorn")]
[assembly: AssemblyTrademark("Glimpse™")]

// Version is in major.minor.build format to support http://semver.org/
// Keep these three attributes in sync
[assembly: AssemblyVersion("1.0.0")]
[assembly: AssemblyFileVersion("1.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0")] // Used to specify the NuGet version number at build time

//[assembly: InternalsVisibleTo("Glimpse.Test.WebApi2.1.WebHost")]
[assembly: NuGetPackage("Glimpse.WebApi2.1.WebHost")]