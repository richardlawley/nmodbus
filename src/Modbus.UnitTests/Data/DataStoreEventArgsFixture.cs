using System.Linq;
using MbUnit.Framework;
using Modbus.Data;

namespace Modbus.UnitTests.Data
{
	[TestFixture]
	public class DataStoreEventArgsFixture
	{
		[Test]
		public void CreateDataStoreEventArgs()
		{
			var eventArgs = DataStoreEventArgs.CreateDataStoreEventArgs(5, ModbusDataType.HoldingRegister, new ushort[] { 1, 2, 3 });
			Assert.AreEqual(ModbusDataType.HoldingRegister, eventArgs.ModbusDataType);
			Assert.AreEqual(5, eventArgs.StartAddress);
			Assert.AreEqual(new ushort[] { 1, 2, 3 }, eventArgs.Data.B.ToArray());
		}

		[Test, ExpectedArgumentException]
		public void CreateDataStoreEventArgs_InvalidType()
		{
			var eventArgs = DataStoreEventArgs.CreateDataStoreEventArgs(5, ModbusDataType.HoldingRegister, new int[] { 1, 2, 3 });
		}

		[Test, ExpectedArgumentNullException]
		public void CreateDataStoreEventArgs_DataNull()
		{
			var eventArgs = DataStoreEventArgs.CreateDataStoreEventArgs(5, ModbusDataType.HoldingRegister, default(ushort[]));
		}
	}
}
