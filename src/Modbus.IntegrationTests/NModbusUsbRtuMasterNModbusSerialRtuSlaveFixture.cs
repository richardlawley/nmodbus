using Modbus.Device;
using NUnit.Framework;
using System;

namespace Modbus.IntegrationTests
{
	[TestFixture]
	public class NModbusUsbRtuMasterNModbusSerialRtuSlaveFixture : ModbusSerialMasterFixture
	{
		[TestFixtureSetUp]
		public override void Init()
		{
			base.Init();

			SetupSlaveSerialPort();
			Slave = ModbusSerialSlave.CreateRtu(SlaveAddress, SlaveSerialPort);
			StartSlave();

			MasterUsbPort = CreateAndOpenUsbPort(DefaultMasterUsbPortId);
			Master = ModbusSerialMaster.CreateRtu(MasterUsbPort);
		}

		/// <summary>
		/// Not supported
		/// </summary>
		public override void ReadWriteMultipleRegisters()
		{
		}

		/// <summary>
		/// Not supported
		/// </summary>
		public override void ReadWriteMultipleRegisters_WriteOccursBeforeRead()
		{
		}

		[Test]
		public override void ExecuteCustomMessage_ReadHoldingRegisters()
		{
			Assert.Throws<ArgumentException>(() => base.ExecuteCustomMessage_ReadHoldingRegisters());
		}

		[Test]
		public override void ExecuteCustomMessage_WriteMultipleRegisters()
		{
			Assert.Throws<ArgumentException>(() => base.ExecuteCustomMessage_WriteMultipleRegisters());
		}

		[Test]
		public override void ReadCoils()
		{
			base.ReadCoils();
		}
	}
}
