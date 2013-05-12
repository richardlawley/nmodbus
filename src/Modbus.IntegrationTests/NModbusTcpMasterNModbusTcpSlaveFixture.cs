using System.Linq;
using System.Net.Sockets;
using MbUnit.Framework;
using Modbus.Device;
using Modbus.IntegrationTests.CustomMessages;
using Unme.Common;

namespace Modbus.IntegrationTests
{
	[TestFixture]
	public class NModbusTcpMasterNModbusTcpSlaveFixture : ModbusMasterFixture
	{
		[TestFixtureSetUp]
		public override void Init()
		{
			base.Init();

			SlaveTcp = new TcpListener(TcpHost, Port);
			SlaveTcp.Start();
			Slave = ModbusTcpSlave.CreateTcp(SlaveAddress, SlaveTcp);
			StartSlave();

			MasterTcp = new TcpClient(TcpHost.ToString(), Port);
			Master = ModbusIpMaster.CreateIp(MasterTcp);
			Master.Transport.ReadTimeout = Master.Transport.WriteTimeout = 1000;
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
		public override void ReadMaximumNumberOfHoldingRegisters()
		{
			base.ReadMaximumNumberOfHoldingRegisters();
		}

		[Test]
		public override void ReadWriteMultipleRegisters()
		{
			base.ReadWriteMultipleRegisters();
		}

        [Test]
        public override void ReadWriteMultipleRegisters_WriteOccursBeforeRead()
        {
            base.ReadWriteMultipleRegisters_WriteOccursBeforeRead();
        }

		[Test]
		public override void SimpleReadRegistersPerformanceTest()
		{
			base.SimpleReadRegistersPerformanceTest();
		}

		[Test]
		public override void ExecuteCustomMessage_ReadHoldingRegisters()
		{
			base.ExecuteCustomMessage_ReadHoldingRegisters();
		}

		[Test]
		public override void ExecuteCustomMessage_WriteMultipleRegisters()
		{
			base.ExecuteCustomMessage_WriteMultipleRegisters();
		}

		[Test]
		public override void ExecuteCustomMessage_Foobar()
		{
			base.ExecuteCustomMessage_Foobar();
		}
	}
}
