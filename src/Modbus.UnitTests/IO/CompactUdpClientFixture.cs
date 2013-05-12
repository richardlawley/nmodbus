using System;
using System.Net.Sockets;
using System.Threading;
using MbUnit.Framework;
using Modbus.IO;
using Unme.MbUnit.Framework.Extensions;

namespace Modbus.UnitTests.IO
{
	[TestFixture]
	public class CompactUdpClientFixture
	{
		[Test]
		public void ReadTimeout()
		{
			var client = new CompactUdpClient(new UdpClient());
			Assert.AreEqual(Timeout.Infinite, client.ReadTimeout);
			AssertUtility.Throws<InvalidOperationException>(() => client.ReadTimeout = 1000);
		}

		[Test]
		public void WriteTimeout()
		{
			var client = new CompactUdpClient(new UdpClient());
			Assert.AreEqual(Timeout.Infinite, client.WriteTimeout);
			AssertUtility.Throws<InvalidOperationException>(() => client.WriteTimeout = 1000);
		}
	}
}
