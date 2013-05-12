using System;
using System.Net.Sockets;
using MbUnit.Framework;
using Modbus.IO;
using Unme.MbUnit.Framework.Extensions;

namespace Modbus.UnitTests.IO
{
	[TestFixture]
	public class UdpClientAdapterFixture
	{
		[Test]
		public void Read_ArgumentValidation()
		{
			var adapter = new UdpClientAdapter(new UdpClient());

			// buffer
			AssertUtility.Throws<ArgumentNullException>(() => adapter.Read(null, 1, 1));
			
			// offset
			AssertUtility.Throws<ArgumentOutOfRangeException>(() => adapter.Read(new byte[2], -1, 2));
			AssertUtility.Throws<ArgumentOutOfRangeException>(() => adapter.Read(new byte[2], 3, 3));

			AssertUtility.Throws<ArgumentOutOfRangeException>(() => adapter.Read(new byte[2], 0, -1));
			AssertUtility.Throws<ArgumentOutOfRangeException>(() => adapter.Read(new byte[2], 1, 2));
		}

		[Test]
		public void Write_ArgumentValidation()
		{
			var adapter = new UdpClientAdapter(new UdpClient());

			// buffer 
			AssertUtility.Throws<ArgumentNullException>(() => adapter.Write(null, 1, 1));

			// offset
			AssertUtility.Throws<ArgumentOutOfRangeException>(() => adapter.Write(new byte[2], -1, 2));
			AssertUtility.Throws<ArgumentOutOfRangeException>(() => adapter.Write(new byte[2], 3, 3));
		}
	}
}
