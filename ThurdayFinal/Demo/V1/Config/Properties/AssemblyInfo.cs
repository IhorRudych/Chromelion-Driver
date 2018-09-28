using System.Reflection;

[assembly: AssemblyTitle("MyCompany.Demo Driver Configuration")]
[assembly: AssemblyProduct("Chromeleon MyCompany.Demo Driver Example")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyCompany("Thermo Fisher Scientific Inc.")]
[assembly: AssemblyCopyright("Copyright © 2018 Thermo Fisher Scientific Inc.")]
[assembly: AssemblyTrademark("Chromeleon")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
//                      Major.Minor.Buid.Revision
[assembly: AssemblyVersion    ("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
