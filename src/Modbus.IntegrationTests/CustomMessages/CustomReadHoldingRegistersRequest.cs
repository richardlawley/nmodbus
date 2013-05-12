using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using Modbus.Message;

namespace Modbus.IntegrationTests.CustomMessages
{
	public class CustomReadHoldingRegistersRequest : IModbusMessage
	{
		private const byte ReadHoldingRegistersFunctionCode = 3;

		public CustomReadHoldingRegistersRequest(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
		{
			FunctionCode = ReadHoldingRegistersFunctionCode;
			SlaveAddress = slaveAddress;
			StartAddress = startAddress;
			NumberOfPoints = numberOfPoints;
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

				return pdu.ToArray();
			}
		}

		public ushort TransactionId { get; set; }

		public byte FunctionCode
		{
			get
			{
				return ReadHoldingRegistersFunctionCode;
			}
			set
			{
				if (value != ReadHoldingRegistersFunctionCode)
				{
					throw new ArgumentException(
						String.Format(CultureInfo.InvariantCulture, "Invalid function code in message frame, expected: {0}; actual: {1}",
						ReadHoldingRegistersFunctionCode,
						value));
				}
			}
		}

		public byte SlaveAddress { get; set; }

		public ushort StartAddress { get; set; }

		public ushort NumberOfPoints { get; set; }

		public void Initialize(byte[] frame)
		{
			if (frame == null)
				throw new ArgumentNullException("frame");
			if (frame.Length < 6)
				throw new ArgumentException("Invalid frame.", "frame");

			SlaveAddress = frame[0];
			FunctionCode = frame[1];
			StartAddress = (ushort) IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 2));
			NumberOfPoints = (ushort) IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 4));
		}
	}
}
