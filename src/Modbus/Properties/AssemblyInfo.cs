using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Modbus")]
[assembly: AssemblyProduct("NModbus")]
[assembly: AssemblyCopyright("Licensed under MIT License.")]
[assembly: AssemblyDescription("Provides connectivity to Modbus slave compatible devices and applications.")]

// required for VBA applications
[assembly: ComVisible(true)]
[assembly: CLSCompliant(false)]
[assembly: Guid("a2ec5378-e1b7-4bb0-b696-1c657407eeb9")]
[assembly: AssemblyVersion("1.9.0.0")]
#if !WindowsCE
[assembly: AssemblyFileVersion("1.9.0.0")]
#endif

#if SIGNED
[assembly: InternalsVisibleTo(@"FtdAdapter, PublicKey=00240000048000009400000006020000002400005253413100040000010001008164fa1d8adb85038b7d1ad4a4dcb9d680e7b23398d318dcf6673432bca1256c191e8d82f90d4221d81d1a9065830c74d4d0062046707397e3dcc179c98c443cab8501dba986e78424fd395675281a3574cf7554da4aee1ce8a97e1826bc4a45856b1f5c4479c9e5051bf78999f9eb035a9987efae198d5d599c03e743c207e9")]
#else
[assembly: InternalsVisibleTo(@"FtdAdapter")]
[assembly: InternalsVisibleTo(@"Modbus.UnitTests")]
[assembly: InternalsVisibleTo(@"DynamicProxyGenAssembly2")]
#endif
