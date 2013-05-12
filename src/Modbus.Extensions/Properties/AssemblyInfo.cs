using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Modbus.Extensions")]
[assembly: AssemblyProduct("NModbus")]
[assembly: AssemblyCopyright("Licensed under MIT License.")]
[assembly: AssemblyDescription("Provides utiltiy extensions for the NModbus library.")]

// required for VBA applications
[assembly: ComVisible(true)]
[assembly: CLSCompliant(false)]
[assembly: Guid("A0218720-E559-434d-AB0D-CC03E6D69F46")]
[assembly: AssemblyVersion("1.10.0.0")]
#if !WindowsCE
[assembly: AssemblyFileVersion("1.10.0.0")]
#endif