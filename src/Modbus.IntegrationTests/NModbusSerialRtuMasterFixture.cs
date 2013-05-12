using System;
using System.IO.Ports;
using Modbus.Device;
using MbUnit.Framework;

namespace Modbus.IntegrationTests
{
	[TestFixture]
	public class NModbusSerialRtuMasterFixture
	{
		[Test, ExpectedException(typeof(TimeoutException))]
		public void NModbusRtuMaster_ReadTimeout()
		{
			SerialPort port = ModbusMasterFixture.CreateAndOpenSerialPort(ModbusMasterFixture.DefaultMasterSerialPortName);
			using (var master = ModbusSerialMaster.CreateRtu(port))
			{
				master.Transport.ReadTimeout = master.Transport.WriteTimeout = 1000;
				master.ReadCoils(100, 1, 1);
			}
		}
	}
}
