using FtdAdapter;
using MbUnit.Framework;

namespace Modbus.IntegrationTests
{
	[TestFixture]
	public class FtdUsbPortAdapterFixture
	{
		[Test]
		public void GetDeviceInfos()
		{
			var deviceInfos = FtdUsbPort.GetDeviceInfos();
			Assert.IsNotNull(deviceInfos);
			Assert.AreEqual(1, deviceInfos.Length);			
		}

		[Test]
		public void GetDeviceInfo()
		{
			var deviceInfo = FtdUsbPort.GetDeviceInfo(0);
			Assert.IsNotNull(deviceInfo);

			Assert.AreEqual("ftCARN75", deviceInfo.SerialNumber);
			Assert.AreEqual("  USB Serial Converter", deviceInfo.Description);
		}

		[Test]
		public void OpenByIndex()
		{
			using (var port = new FtdUsbPort())
			{
				port.OpenByIndex(0);
				Assert.IsTrue(port.IsOpen);
			}
		}

		[Test]
		public void OpenByLocationId()
		{
			var deviceInfo = FtdUsbPort.GetDeviceInfo(0);
			Assert.IsNotNull(deviceInfo);

			using (var port = new FtdUsbPort())
			{
				port.OpenByLocationId(deviceInfo.LocationId);
				Assert.IsTrue(port.IsOpen);
			}
		}

		[Test]
		public void OpenByDescription()
		{
			var deviceInfo = FtdUsbPort.GetDeviceInfo(0);
			Assert.IsNotNull(deviceInfo);

			using (var port = new FtdUsbPort())
			{
				port.OpenByDescription(deviceInfo.Description);
				Assert.IsTrue(port.IsOpen);
			}
		}

		[Test]
		public void OpenBySerialNumber()
		{
			var deviceInfo = FtdUsbPort.GetDeviceInfo(0);
			Assert.IsNotNull(deviceInfo);

			using (var port = new FtdUsbPort())
			{
				port.OpenBySerialNumber(deviceInfo.SerialNumber);
				Assert.IsTrue(port.IsOpen);
			}
		}
	}
}
