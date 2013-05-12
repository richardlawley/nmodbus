using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Modbus.Data;
using Modbus.Message;
using Unme.Common;
using System.Globalization;

namespace Modbus.IntegrationTests.CustomMessages
{
	public class CustomWriteMultipleRegistersRequest : IModbusMessage
	{
		private const byte WriteMultipleRegistersFunctionCode = 16;

		public CustomWriteMultipleRegistersRequest(byte slaveAddress, ushort startAddress, RegisterCollection data)
		{
			SlaveAddress = slaveAddress;
			StartAddress = startAddress;
			NumberOfPoints = (ushort) data.Count;
			ByteCount = data.ByteCount;
			Data = data;
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
				pdu.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short) StartAddress)));
				pdu.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short) NumberOfPoints)));
				pdu.Add(ByteCount);
				pdu.AddRange(Data.NetworkBytes);

				return pdu.ToArray();
			}
		}

		public ushort TransactionId { get; set; }

		public byte FunctionCode
		{
			get
			{
				return WriteMultipleRegistersFunctionCode;
			}
			set
			{
				if (value != WriteMultipleRegistersFunctionCode)
				{
					throw new ArgumentException(
						String.Format(CultureInfo.InvariantCulture, "Invalid function code in message frame, expected: {0}; actual: {1}",
						WriteMultipleRegistersFunctionCode,
						value));
				}
			}
		}

		public byte SlaveAddress { get; set; }

		public ushort StartAddress { get; set; }

		public ushort NumberOfPoints { get; set; }

		public byte ByteCount { get; set; }

		public RegisterCollection Data { get; set; }

		public void Initialize(byte[] frame)
		{
			if (frame == null)
				throw new ArgumentNullException("frame");

			if (frame.Length < 7 || frame.Length < 7 + frame[6])
				throw new FormatException("Message frame does not contain enough bytes.");

			SlaveAddress = frame[0];
			FunctionCode = frame[1];
			StartAddress = (ushort) IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 2));
			NumberOfPoints = (ushort) IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 4));
			ByteCount = frame[6];
			Data = new RegisterCollection(frame.Slice(7, ByteCount).ToArray());
		}
	}
}
