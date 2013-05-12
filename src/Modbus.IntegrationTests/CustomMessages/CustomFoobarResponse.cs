using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Modbus.Data;
using Modbus.Message;
using Unme.Common;

namespace Modbus.IntegrationTests.CustomMessages
{
	public class CustomFoobarResponse : IModbusMessageRtu
	{
		internal const byte FoobarFunctionCode = 100;
		private RegisterCollection _data;

		public ushort[] Data
		{
			get
			{
				return _data.ToArray();
			}
			set
			{
				_data = new RegisterCollection(value);
			}
		}

		public byte[] MessageFrame
		{
			get
			{
				List<byte> frame = new List<byte>();
				frame.Add(SlaveAddress);
				frame.AddRange(ProtocolDataUnit);

				return frame.ToArray();
			}
		}

		public byte[] ProtocolDataUnit
		{
			get
			{
				List<byte> pdu = new List<byte>();

				pdu.Add(FunctionCode);
				pdu.Add(ByteCount);
				pdu.AddRange(_data.NetworkBytes);

				return pdu.ToArray();
			}
		}

		public ushort TransactionId { get; set; }

		public byte FunctionCode
		{
			get
			{
				return FoobarFunctionCode;
			}
			set
			{
				if (value != FoobarFunctionCode)
				{
					throw new ArgumentException(
						String.Format(CultureInfo.InvariantCulture, "Invalid function code in message frame, expected: {0}; actual: {1}",
						FoobarFunctionCode,
						value));
				}
			}
		}

		public byte SlaveAddress { get; set; }

		public byte ByteCount { get; set; }

		public Func<byte[], int> RtuBytesRemaining
		{
			get
			{
				return frameStart => frameStart[2] + 1;
			}
		}

		public void Initialize(byte[] frame)
		{
			if (frame == null)
				throw new ArgumentNullException("frame");
			if (frame.Length < 3 || frame.Length < 3 + frame[2])
				throw new ArgumentException("Message frame does not contain enough bytes.", "frame");

			SlaveAddress = frame[0];
			FunctionCode = frame[1];
			ByteCount = frame[2];
			_data = new RegisterCollection(frame.Slice(3, ByteCount).ToArray());
		}
	}
}
