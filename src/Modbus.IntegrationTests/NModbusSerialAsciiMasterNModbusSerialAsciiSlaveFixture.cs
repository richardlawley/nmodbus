using Modbus.Device;
using MbUnit.Framework;

namespace Modbus.IntegrationTests
{
	[TestFixture]
	public class NModbusSerialAsciiMasterNModbusSerialAsciiSlaveFixture : ModbusSerialMasterFixture
	{
		[TestFixtureSetUp]
		public override void Init()
		{
			base.Init();

			MasterSerialPort = CreateAndOpenSerialPort(DefaultMasterSerialPortName);			
			Master = ModbusSerialMaster.CreateAscii(MasterSerialPort);
			SetupSlaveSerialPort();
			Slave = ModbusSerialSlave.CreateAscii(SlaveAddress, SlaveSerialPort);

			StartSlave();
		}

		[Test]
		public override void ReadCoils()
		{ 
			base.ReadCoils();
		}

		[Test]
		public override void ReadInputs()
		{
			base.ReadInputs();
		}

		[Test]
		public override void ReadHoldingRegisters()
		{
			base.ReadHoldingRegisters();
		}

		[Test]
		public override void ReadInputRegisters()
		{
			base.ReadInputRegisters();
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

		[Test]
		public override void ReadWriteMultipleRegisters()
		{
			base.ReadWriteMultipleRegisters();
		}

		[Test]
		public override void ReturnQueryData()
		{
			base.ReturnQueryData();
		}

		[Test]
		public override void ExecuteCustomMessage_Foobar()
		{
			base.ExecuteCustomMessage_Foobar();
		} 
	}
}
