using System.Diagnostics.CodeAnalysis;
namespace FtdAdapter
{
	/// <summary>
	/// Specifies the number of stop bits used on the UsbPort object.
	/// </summary>
	[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
	[SuppressMessage("Microsoft.Naming", "CA1717:OnlyFlagsEnumsShouldHavePluralNames")]
	public enum FtdStopBits
	{
		/// <summary>
		/// One stop bit is used.
		/// </summary>
		One = 1,
		/// <summary>
		/// 1.5 stop bits are used.
		/// </summary>
		OnePointFive,
		/// <summary>
		/// Two stop bits are used.
		/// </summary>
		Two
	}

	/// <summary>
	/// Specifies the parity used on the UsbPort object.
	/// </summary>
	public enum FtdParity
	{
		/// <summary>
		/// No parity check occurs.
		/// </summary>
		None = 0,
		/// <summary>
		/// Sets the parity bit so that the count of bits set is an odd number.
		/// </summary>
		Odd,
		/// <summary>
		/// Sets the parity bit so that the count of bits set is an even number.
		/// </summary>
		Even,
		/// <summary>
		/// Leaves the parity bit set to 1.
		/// </summary>
		Mark,
		/// <summary>
		/// Leaves the parity bit set to 0.
		/// </summary>
		Space
	}

	///<summary>
	/// Specifies the flow control used on the UsbPort object.
	///</summary>
	[SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
	public enum FtdFlowControl
	{
		///<summary>
		/// No flow control is used
		///</summary>
		None = 0x0000,
		///<summary>
		/// RTS CTS are used for flow control
		///</summary>
		RtsCts = 0x0100,
		/// <summary>
		/// DTR DSR are used for flow control
		/// </summary>
		DtrDsr = 0x0200,
		///<summary>
		/// XON XOFF are used for flow control
		///</summary>
		XonXoff = 0x0400
	}

	/// <summary>
	/// Specifies the result of a UsbPort operation.
	/// </summary>
	internal enum FtdStatus
	{
		OK = 0,
		InvalidHandle,
		DeviceNotFound,
		DeviceNotOpened,
		IOError,
		InsufficientResources,
		InvalidParameter,
		InvalidBaudRate,
		DeviceNotOpenedForErase,
		DeviceNotOpenedForWrite,
		FailedToWriteDevice,
		EEPromReadFailed,
		EEPromWriteFailed,
		EEPromEraseFailed,
		EEPromNotPresent,
		EEPromNotProgrammed,
		InvalidArgs,
		OtherError
	}

	/// <summary>
	/// Specifies how to open the device
	/// </summary>
	internal enum OpenExFlags
	{
		BySerialNumber = 1,
		ByDescription = 2,
		ByLocation = 4
	}
}
