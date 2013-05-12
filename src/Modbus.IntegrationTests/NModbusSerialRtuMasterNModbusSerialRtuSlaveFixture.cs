using System;
using MbUnit.Framework;
using Modbus.Device;
using Unme.MbUnit.Framework.Extensions;

namespace Modbus.IntegrationTests
{
	[TestFixture]
	public class NModbusSerialRtuMasterNModbusSerialRtuSlaveFixture : ModbusSerialMasterFixture
	{
		[TestFixtureSetUp]
		public override void Init()
		{
			base.Init();

			SetupSlaveSerialPort();
			Slave = ModbusSerialSlave.CreateRtu(SlaveAddress, SlaveSerialPort);
			StartSlave();

			MasterSerialPort = CreateAndOpenSerialPort(DefaultMasterSerialPortName);
			Master = ModbusSerialMaster.CreateRtu(MasterSerialPort);
		}

		[Test]
		public override void ReadCoils()
		{
			base.ReadCoils();
		}

		[Test]
		public override void ReadHoldingRegisters()
		{
			base.ReadHoldingRegisters();
		}

		[Test]
		public override void ReadInputs()
		{
			base.ReadInputs();
		}

		[Test]
		public override void WriteSingleCoil()
		{
			base.WriteSingleCoil();
		}

		[Test]
		public override void WriteMultipleCoils()
		{
			base.WriteMultipleCoils();
		}

		[Test]
		public override void WriteSingleRegister()
		{
			base.WriteSingleRegister();
		}

		[Test]
		public override void WriteMultipleRegisters()
		{
			base.WriteMultipleRegisters();
		}


		[Test, Ignore("Need to fix RTU slave for this function code")]
		public override void ReadWriteMultipleRegisters()
		{
			base.ReadWriteMultipleRegisters();
		}

		[Test, Ignore("Need to fix RTU slave for this function code")]
		public override void ReadWriteMultipleRegisters_WriteOccursBeforeRead()
		{
		}

		[Test]
		public override void ReturnQueryData()
		{
			base.ReturnQueryData();
		}

		[Test]
		public override void ExecuteCustomMessage_ReadHoldingRegisters()
		{
			AssertUtility.Throws<ArgumentException>(() => base.ExecuteCustomMessage_ReadHoldingRegisters());
		}

		[Test]
		public override void ExecuteCustomMessage_WriteMultipleRegisters()
		{
			AssertUtility.Throws<ArgumentException>(() => base.ExecuteCustomMessage_WriteMultipleRegisters());
		}
	}
}
