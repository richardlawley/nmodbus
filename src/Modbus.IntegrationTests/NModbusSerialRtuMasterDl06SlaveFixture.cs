using Modbus.Device;
using MbUnit.Framework;
using System;

namespace Modbus.IntegrationTests
{
	[TestFixture]
	public class NModbusSerialRtuMasterDl06SlaveFixture : ModbusSerialMasterFixture
	{
		[TestFixtureSetUp]
		public override void Init()
		{
			base.Init();

			MasterSerialPort = CreateAndOpenSerialPort("COM4");
			Master = ModbusSerialMaster.CreateRtu(MasterSerialPort);
		}

		/// <summary>
		/// Not supported
		/// </summary>
		public override void ReadWriteMultipleRegisters()
		{
		}

		/// <summary>
		/// Not supported
		/// </summary>
		public override void ReadWriteMultipleRegisters_WriteOccursBeforeRead()
		{		
		}

		/// <summary>
		/// Not supported by the DL06
		/// </summary>
		public override void ReturnQueryData()
		{
		}		

		[Test]
		public override void ReadCoils()
		{
			base.ReadCoils();
		}

		/// <summary>
		/// Not supported by the DL06
		/// </summary>
		public override void ExecuteCustomMessage_Foobar()
		{
		}
		
		/// <summary>
		/// Does not implement IModbusMessageRtu
		/// </summary>
		public override void ExecuteCustomMessage_ReadHoldingRegisters()
		{
		}

		/// <summary>
		/// Does not implement IModbusMessageRtu
		/// </summary>
		public override void ExecuteCustomMessage_WriteMultipleRegisters()
		{
		}	}
}
