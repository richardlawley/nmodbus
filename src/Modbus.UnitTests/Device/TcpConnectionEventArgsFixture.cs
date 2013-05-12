using System;
using MbUnit.Framework;
using Modbus.Device;

namespace Modbus.UnitTests.Device
{
	[TestFixture]
	public class TcpConnectionEventArgsFixture
	{
		[Test, ExpectedArgumentNullException]
		public void TcpConnectionEventArgs_NullEndPoint()
		{
			new TcpConnectionEventArgs(null);
		}

		[Test, ExpectedArgumentException]
		public void TcpConnectionEventArgs_EmptyEndPoint()
		{
			new TcpConnectionEventArgs(String.Empty);
		}

		[Test]
		public void TcpConnectionEventArgs()
		{
			var args = new TcpConnectionEventArgs("foo");

			Assert.AreEqual("foo", args.EndPoint);
		}
	}
}
