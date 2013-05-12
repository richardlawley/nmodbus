using System.Net.Sockets;
using Modbus.Device;
using MbUnit.Framework;

namespace Modbus.IntegrationTests
{
	[TestFixture]
	public class NModbusUdpMasterNModbusUdpSlaveFixture : ModbusMasterFixture
	{
		[TestFixtureSetUp]
		public override void Init()
		{
			base.Init();

			SlaveUdp = new UdpClient(Port);
			Slave = ModbusUdpSlave.CreateUdp(SlaveUdp);
			StartSlave();

			MasterUdp = new UdpClient();
			MasterUdp.Connect(DefaultModbusIPEndPoint);
			Master = ModbusIpMaster.CreateIp(MasterUdp);
		}

		[Test]
		public override void ReadCoils()
		{
			base.ReadCoils();
		}

		[Test]
		public override void SimpleReadRegistersPerformanceTest()
		{
			base.SimpleReadRegistersPerformanceTest();
		}
	}
}
