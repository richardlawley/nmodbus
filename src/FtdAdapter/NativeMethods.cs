using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace FtdAdapter
{
	internal static class NativeMethods
	{
		[DllImport(FtdAssemblyName)]
		internal static extern FtdStatus FT_Close(uint deviceHandle);
		[DllImport(FtdAssemblyName)]
		internal static extern FtdStatus FT_Open(uint deviceId, ref uint deviceHandle);
		[DllImport(FtdAssemblyName)]
		internal static extern FtdStatus FT_SetBaudRate(uint deviceHandle, uint baudRate);
		[DllImport(FtdAssemblyName)]
		internal static extern FtdStatus FT_SetFlowControl(uint handle, ushort usFlowControl, byte uXon, byte uXoff);
		[DllImport(FtdAssemblyName)]
		internal static extern FtdStatus FT_SetDataCharacteristics(uint deviceHandle, byte wordLength, byte stopBits, byte parity);
		[DllImport(FtdAssemblyName)]
		internal static extern FtdStatus FT_Read(uint deviceHandle, byte[] buffer, uint bytesToRead, ref uint bytesReturned);
		[DllImport(FtdAssemblyName)]
		internal static extern FtdStatus FT_Write(uint deviceHandle, byte[] buffer, uint bytesToWrite, ref uint bytesWritten);
		[DllImport(FtdAssemblyName)]
		internal static extern FtdStatus FT_SetTimeouts(uint deviceHandle, uint readTimeout, uint writeTimeout);
		[DllImport(FtdAssemblyName)]
		internal static extern FtdStatus FT_Purge(uint deviceHandle, uint mask);
		[DllImport(FtdAssemblyName)]
		internal static extern FtdStatus FT_CreateDeviceInfoList(ref uint deviceCount);
		[DllImport(FtdAssemblyName)]
		internal static extern FtdStatus FT_GetDeviceInfoDetail(uint index, ref uint flags, ref uint type, ref uint id,
			ref uint locid, [In] [Out] byte[] serial, [In] [Out] byte[] description, ref uint deviceHandle);
		[DllImport(FtdAssemblyName)]
		internal static extern FtdStatus FT_OpenEx(byte[] key, uint flags, ref uint deviceHandle);
		[DllImport(FtdAssemblyName)]
		internal static extern FtdStatus FT_OpenEx(uint locationId, uint flags, ref uint deviceHandle);

		private const string FtdAssemblyName = "FTD2XX.dll";
	}
}
