using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using System.IO.Ports;
using Modbus.Device;
using Modbus.Data;
using System.Threading;

namespace Modbus.IntegrationTests
{
	[TestFixture]
	public class NModbusSerialRtuSlaveFixture
	{
		[TestFixtureSetUp]
		public void SetUp()
		{
			log4net.Config.XmlConfigurator.Configure();
		}

		[Test]
		public void NModbusSerialRtuSlave_BonusCharacter_VerifyTimeout()
		{
			SerialPort masterPort = ModbusMasterFixture.CreateAndOpenSerialPort(ModbusMasterFixture.DefaultMasterSerialPortName);
			SerialPort slavePort = ModbusMasterFixture.CreateAndOpenSerialPort(ModbusMasterFixture.DefaultSlaveSerialPortName);

			using (var master = ModbusSerialMaster.CreateRtu(masterPort))
			using (var slave = ModbusSerialSlave.CreateRtu(1, slavePort))
			{
				master.Transport.ReadTimeout = master.Transport.WriteTimeout = 1000;				
				slave.DataStore = DataStoreFactory.CreateTestDataStore();

				Thread slaveThread = new Thread(slave.Listen);
				slaveThread.IsBackground = true;
				slaveThread.Start();

				// assert successful communication
				Assert.AreEqual(new bool[] { false, true }, master.ReadCoils(1, 1, 2));

				// write "bonus" character
				masterPort.Write("*");

				// assert successful communication
				Assert.AreEqual(new bool[] { false, true }, master.ReadCoils(1, 1, 2));
			}
		}
	}
}
