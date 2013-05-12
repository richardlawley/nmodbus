using System;
using System.IO.Ports;
using Modbus.Device;
using MbUnit.Framework;

namespace Modbus.IntegrationTests
{
	[TestFixture]
	public class NModbusSerialAsciiMasterFixture
	{
		[Test, ExpectedException(typeof(TimeoutException))]
		public void NModbusAsciiMaster_ReadTimeout()
		{
			SerialPort port = ModbusMasterFixture.CreateAndOpenSerialPort(ModbusMasterFixture.DefaultMasterSerialPortName);
			using (IModbusSerialMaster master = ModbusSerialMaster.CreateAscii(port))
			{
				master.Transport.ReadTimeout = master.Transport.WriteTimeout = 1000;
				master.ReadCoils(100, 1, 1);
			}
		}
	}
}
