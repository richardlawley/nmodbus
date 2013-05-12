using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Modbus.IO;

namespace FtdAdapter
{
	/// <summary>
	/// Wrapper class for the FTD2XX USB resource.
	/// </summary>
	public class FtdUsbPort : IStreamResource, IDisposable
	{
		private const byte PurgeRx = 1;
		private uint _deviceHandle;
		private uint _readTimeout;
		private uint _writeTimeout;
		private int _baudRate = 9600;
		private int _dataBits = 8;
		private byte _stopBits = 1;
		private byte _parity;

		/// <summary>
		/// Initializes a new instance of the <see cref="FtdUsbPort"/> class.
		/// </summary>
		public FtdUsbPort()
		{
		}

		/// <summary>
		/// Gets or sets the serial baud rate. 
		/// </summary>
		public int BaudRate
		{
			get
			{
				return _baudRate;
			}
			set
			{
				if (value <= 0)
					throw new ArgumentOutOfRangeException("BaudRate", "BaudRate must be greater than 0.");

				_baudRate = value;

				if (IsOpen)
					InvokeFtdMethod(() => NativeMethods.FT_SetBaudRate(_deviceHandle, (uint) BaudRate));
			}
		}

		/// <summary>
		/// Gets or sets the standard length of data bits per byte. 
		/// </summary>
		public int DataBits
		{
			get
			{
				return _dataBits;
			}
			set
			{
				if (value < 5 || value > 8)
					throw new ArgumentOutOfRangeException("DataBits", "Value must be greater than 4 and less than 9.");

				_dataBits = value;

				if (IsOpen)
					InvokeFtdMethod(() => NativeMethods.FT_SetDataCharacteristics(_deviceHandle, (byte) DataBits, (byte) StopBits, (byte) Parity));
			}
		}

		/// <summary>
		/// Indicates that no time-out should occur.
		/// </summary>
		public int InfiniteTimeout
		{
			get { return 0; }
		}

		/// <summary>
		/// Gets or sets the number of milliseconds before a time-out occurs when a read operation does not finish.
		/// </summary>
		public int ReadTimeout
		{
			get
			{
				return (int) _readTimeout;
			}
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("ReadTimeout", "Read timeout must be greater than 0.");

				_readTimeout = (uint) value;

				if (IsOpen)
					InvokeFtdMethod(() => NativeMethods.FT_SetTimeouts(_deviceHandle, (uint) ReadTimeout, (uint) WriteTimeout));
			}
		}

		/// <summary>
		/// Gets or sets the number of milliseconds before a time-out occurs when a write operation does not finish.
		/// </summary>
		public int WriteTimeout
		{
			get
			{
				return (int) _writeTimeout;
			}
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("WriteTimeout", "Write timeout must be greater than 0.");

				_writeTimeout = (uint) value;

				if (IsOpen)
					InvokeFtdMethod(() => NativeMethods.FT_SetTimeouts(_deviceHandle, (uint) ReadTimeout, (uint) WriteTimeout));
			}
		}

		/// <summary>
		/// Gets or sets the standard number of stopbits per byte.
		/// </summary>
		public FtdStopBits StopBits
		{
			get
			{
				return (FtdStopBits) Enum.Parse(typeof(FtdStopBits), _stopBits.ToString(CultureInfo.InvariantCulture));
			}
			set
			{
				_stopBits = (byte) value;

				if (IsOpen)
					InvokeFtdMethod(() => NativeMethods.FT_SetDataCharacteristics(_deviceHandle, (byte) DataBits, (byte) StopBits, (byte) Parity));
			}
		}

		/// <summary>
		/// Gets or sets the parity-checking protocol.
		/// </summary>
		public FtdParity Parity
		{
			get
			{
				return (FtdParity) Enum.Parse(typeof(FtdParity), _parity.ToString(CultureInfo.InvariantCulture));
			}
			set
			{
				_parity = (byte) value;

				if (IsOpen)
					InvokeFtdMethod(() => NativeMethods.FT_SetDataCharacteristics(_deviceHandle, (byte) DataBits, (byte) StopBits, (byte) Parity));
			}
		}

		/// <summary>
		/// Gets a value indicating the open or closed status of the UsbPort object.
		/// </summary>
		public bool IsOpen
		{
			get
			{
				return _deviceHandle != 0;
			}
		}

		/// <summary>
		/// Returns the number of D2XX devices connected to the system.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Operation is expensive.")]
		public static int GetDeviceCount()
		{
			return CreateDeviceInfoList();
		}

		/// <summary>
		/// Gets an array of the currently connected device's device info.
		/// </summary>
		/// <returns>An array of FtdDeviceInfo objects.</returns>
		/// <exception cref="IOException"></exception>
		public static FtdDeviceInfo[] GetDeviceInfos()
		{
			return Enumerable.Range(0, CreateDeviceInfoList()).Select(n => GetDeviceInfo((uint) n)).ToArray();
		}

		/// <summary>
		/// Gets the device info at the specified device index.
		/// </summary>
		/// <param name="index">Index of the device.</param>
		/// <returns>An FtdDeviceInfo instance.</returns>
		public static FtdDeviceInfo GetDeviceInfo(uint index)
		{
			uint flags = 0, type = 0, id = 0, locId = 0, handle = 0;
			var serial = new byte[16];
			var description = new byte[64];

			CreateDeviceInfoList();
			InvokeFtdMethod(() => NativeMethods.FT_GetDeviceInfoDetail(index, ref flags, ref type, ref id, ref locId, serial, description, ref handle));

			return new FtdDeviceInfo(flags, type, id, locId, Encoding.ASCII.GetString(serial).Split('\0')[0], Encoding.ASCII.GetString(description).Split('\0')[0]);
		}

		/// <summary>
		/// Opens the device by index.
		/// </summary>
		/// <param name="index">Must be 0 if only one device is attached. For multiple devices 1, 2 etc.</param>
		public void OpenByIndex(uint index)
		{
			if (IsOpen)
				throw new InvalidOperationException("Port is already open.");

			InvokeFtdMethod(() => NativeMethods.FT_Open(index, ref _deviceHandle));
			InitializeUsbPort();
		}

		/// <summary>
		/// Opens the device by location id.
		/// </summary>
		/// <param name="locationId">The location id.</param>
		public void OpenByLocationId(uint locationId)
		{
			if (IsOpen)
				throw new InvalidOperationException("Port is already open.");

			InvokeFtdMethod(() => NativeMethods.FT_OpenEx(locationId, (uint) OpenExFlags.ByLocation, ref _deviceHandle));
			InitializeUsbPort();
		}

		/// <summary>
		/// Opens the device by serial number.
		/// </summary>
		/// <param name="serialNumber">The serial number.</param>
		public void OpenBySerialNumber(string serialNumber)
		{
			if (serialNumber == null)
				throw new ArgumentNullException("serialNumber");
			if (IsOpen)
				throw new InvalidOperationException("Port is already open.");

			InvokeFtdMethod(() => NativeMethods.FT_OpenEx(Encoding.ASCII.GetBytes(serialNumber), (uint) OpenExFlags.BySerialNumber, ref _deviceHandle));
			InitializeUsbPort();
		}

		/// <summary>
		/// Opens the device by description.
		/// </summary>
		/// <param name="description">The description.</param>
		public void OpenByDescription(string description)
		{
			if (description == null)
				throw new ArgumentNullException("description");
			if (IsOpen)
				throw new InvalidOperationException("Port is already open.");

			InvokeFtdMethod(() => NativeMethods.FT_OpenEx(Encoding.ASCII.GetBytes(description), (uint) OpenExFlags.ByDescription, ref _deviceHandle));
			InitializeUsbPort();
		}

		/// <summary>
		/// Closes the port connection.
		/// </summary>
		public void Close()
		{
			try
			{
				InvokeFtdMethod(() => NativeMethods.FT_Close(_deviceHandle));
			}
			finally
			{
				_deviceHandle = 0;
			}
		}

		/// <summary>
		/// Writes a specified number of bytes to the port from an output buffer, starting at the specified offset.
		/// </summary>
		/// <param name="buffer">The byte array that contains the data to write to the port.</param>
		/// <param name="offset">The offset in the buffer array to begin writing.</param>
		/// <param name="count">The number of bytes to write.</param>
		public void Write(byte[] buffer, int offset, int count)
		{
			if (!IsOpen)
				throw new InvalidOperationException("Port not open.");
			if (buffer == null)
				throw new ArgumentNullException("buffer", "Argument buffer cannot be null.");
			if (offset < 0)
				throw new ArgumentOutOfRangeException("offset", "Argument offset must be greater than 0.");
			if (count < 0)
				throw new ArgumentOutOfRangeException("count", "Argument count must be greater than 0.");
			if ((buffer.Length - offset) < count)
				throw new ArgumentException("Invalid buffer size.");

			uint bytesWritten = 0;

			var buf = new byte[count];
			Array.Copy(buffer, offset, buf, 0, count);
			InvokeFtdMethod(() => NativeMethods.FT_Write(_deviceHandle, buf, (uint) count, ref bytesWritten));

			if (count != 0 && bytesWritten == 0)
				throw new TimeoutException("The operation has timed out.");
			if (bytesWritten != count)
				throw new IOException("Not all bytes written to stream.");
		}

		/// <summary>
		/// Reads a number of bytes from the UsbPort input buffer and writes those bytes into a byte array at the specified offset.
		/// </summary>
		/// <param name="buffer">The byte array to write the input to.</param>
		/// <param name="offset">The offset in the buffer array to begin writing.</param>
		/// <param name="count">The number of bytes to read.</param>
		/// <returns>The number of bytes read.</returns>
		public int Read(byte[] buffer, int offset, int count)
		{
			if (!IsOpen)
				throw new InvalidOperationException("Port not open.");
			if (buffer == null)
				throw new ArgumentNullException("buffer", "Argument buffer cannot be null.");
			if (offset < 0)
				throw new ArgumentOutOfRangeException("offset", "Argument offset cannot be less than 0.");
			if (count < 0)
				throw new ArgumentOutOfRangeException("count", "Argument count cannot be less than 0.");
			if ((buffer.Length - offset) < count)
				throw new ArgumentException("Invalid buffer size.");

			uint numBytesReturned = 0;
			var buf = new byte[count];
			InvokeFtdMethod(() => NativeMethods.FT_Read(_deviceHandle, buf, (uint) count, ref numBytesReturned));
			Array.Copy(buf, 0, buffer, offset, numBytesReturned);

			if (count != 0 && numBytesReturned == 0)
				throw new TimeoutException("The operation has timed out.");

			return (int) numBytesReturned;
		}

		/// <summary>
		/// Purges the receive buffer.
		/// </summary>
		public void DiscardInBuffer()
		{
			if (!IsOpen)
				throw new InvalidOperationException("Port is not open.");

			InvokeFtdMethod(() => NativeMethods.FT_Purge(_deviceHandle, PurgeRx));
		}

		///<summary>
		/// Set flow control.
		///</summary>
		///<param name="flowControl">Type of flow control</param>
		///<param name="xOn">XON symbol</param>
		///<param name="xOff">XOFF symbol</param>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "1#x")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "2#x")]
		public void SetFlowControl(FtdFlowControl flowControl, byte xOn, byte xOff)
		{
			InvokeFtdMethod(() => NativeMethods.FT_SetFlowControl(_deviceHandle, (ushort) flowControl, xOn, xOff));
		}

		/// <summary>
		/// Invokes FT method and checks the FTStatus result, throw IOException if result is something other than FTStatus.OK
		/// </summary>
		internal static void InvokeFtdMethod(Func<FtdStatus> func)
		{
			FtdStatus status = func();

			if (status != FtdStatus.OK)
				throw new IOException(Enum.GetName(typeof(FtdStatus), status));
		}

		internal static int CreateDeviceInfoList()
		{
			uint deviceCount = 0;
			InvokeFtdMethod(() => NativeMethods.FT_CreateDeviceInfoList(ref deviceCount));

			return (int) deviceCount;
		}

		internal void InitializeUsbPort()
		{
			BaudRate = BaudRate;
			SetFlowControl(FtdFlowControl.None, 0, 0);
			InvokeFtdMethod(() => NativeMethods.FT_SetDataCharacteristics(_deviceHandle, (byte) DataBits, (byte) StopBits, (byte) Parity));
			InvokeFtdMethod(() => NativeMethods.FT_SetTimeouts(_deviceHandle, (uint) ReadTimeout, (uint) WriteTimeout));
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing && IsOpen)
				Close();
		}
	}
}
