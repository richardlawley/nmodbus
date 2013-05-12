using MbUnit.Framework;
using Modbus.Device;

namespace Modbus.IntegrationTests
{
	public abstract class ModbusSerialMasterFixture : ModbusMasterFixture
	{
		[Test]
		public virtual void ReturnQueryData()
		{
			Assert.IsTrue(((ModbusSerialMaster) Master).ReturnQueryData(SlaveAddress, 18));
			Assert.IsTrue(((ModbusSerialMaster) Master).ReturnQueryData(SlaveAddress, 5));
		}
	}
}
