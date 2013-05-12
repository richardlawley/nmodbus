using System.Net.Sockets;
using System.Threading;
using MbUnit.Framework;
using Modbus.Device;

namespace Modbus.IntegrationTests
{
	[TestFixture]
	public class ModbusIpMasterFixture
	{
		[Test]
		public void OverrideTimeoutOnTcpClient()
		{
			var listener = new TcpListener(ModbusMasterFixture.TcpHost, ModbusMasterFixture.Port);
			using (var slave = ModbusTcpSlave.CreateTcp(ModbusMasterFixture.SlaveAddress, listener))
			{
				var slaveThread = new Thread(slave.Listen);
				slaveThread.Start();

				var client = new TcpClient(ModbusMasterFixture.TcpHost.ToString(), ModbusMasterFixture.Port);
				client.ReceiveTimeout = 1500;
				client.SendTimeout = 3000;
				using (var master = ModbusIpMaster.CreateIp(client))
				{
					Assert.AreEqual(1500, client.GetStream().ReadTimeout);
					Assert.AreEqual(3000, client.GetStream().WriteTimeout);
				}
			}
		}

		[Test]
		public void OverrideTimeoutOnNetworkStream()
		{
			var listener = new TcpListener(ModbusMasterFixture.TcpHost, ModbusMasterFixture.Port);
			using (var slave = ModbusTcpSlave.CreateTcp(ModbusMasterFixture.SlaveAddress, listener))
			{
				var slaveThread = new Thread(slave.Listen);
				slaveThread.Start();

				var client = new TcpClient(ModbusMasterFixture.TcpHost.ToString(), ModbusMasterFixture.Port);
				client.GetStream().ReadTimeout = 1500;
				client.GetStream().WriteTimeout = 3000;
				using (var master = ModbusIpMaster.CreateIp(client))
				{
					Assert.AreEqual(1500, client.GetStream().ReadTimeout);
					Assert.AreEqual(3000, client.GetStream().WriteTimeout);
				}
			}
		}
	}
}
