using MbUnit.Framework;
using Modbus.Extensions.Enron;

namespace Modbus.IntegrationTests
{
	[TestFixture]
	public class EnronFixture : NModbusSerialRtuMasterDl06SlaveFixture
	{
		[Test]
		public virtual void ReadHoldingRegisters32()
		{
			uint[] registers = Master.ReadHoldingRegisters32(SlaveAddress, 104, 2);
			Assert.AreEqual(new uint[] { 0, 0 }, registers);
		}

		[Test]
		public virtual void ReadInputRegisters32()
		{
			uint[] registers = Master.ReadInputRegisters32(SlaveAddress, 104, 2);
			Assert.AreEqual(new uint[] { 0, 0 }, registers);
		}
		
		[Test]
		public virtual void WriteSingleRegister32()
		{
			ushort testAddress = 200;
			uint testValue = 350;

			uint originalValue = Master.ReadHoldingRegisters32(SlaveAddress, testAddress, 1)[0];
			Master.WriteSingleRegister32(SlaveAddress, testAddress, testValue);
			Assert.AreEqual(testValue, Master.ReadHoldingRegisters32(SlaveAddress, testAddress, 1)[0]);
			Master.WriteSingleRegister32(SlaveAddress, testAddress, originalValue);
			Assert.AreEqual(originalValue, Master.ReadHoldingRegisters(SlaveAddress, testAddress, 1)[0]);
		}

		[Test]
		public virtual void WriteMultipleRegisters32()
		{
			ushort testAddress = 120;
			uint[] testValues = new uint[] { 10, 20, 30, 40, 50 };

			uint[] originalValues = Master.ReadHoldingRegisters32(SlaveAddress, testAddress, (ushort) testValues.Length);
			Master.WriteMultipleRegisters32(SlaveAddress, testAddress, testValues);
			uint[] newValues = Master.ReadHoldingRegisters32(SlaveAddress, testAddress, (ushort) testValues.Length);
			Assert.AreEqual(testValues, newValues);
			Master.WriteMultipleRegisters32(SlaveAddress, testAddress, originalValues);
		}
	}
}
