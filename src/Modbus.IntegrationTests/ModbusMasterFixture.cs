using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using FtdAdapter;
using log4net;
using MbUnit.Framework;
using Modbus.Data;
using Modbus.Device;
using Modbus.IntegrationTests.CustomMessages;
using Unme.Common;
using Unme.MbUnit.Framework.Extensions;

namespace Modbus.IntegrationTests
{
	public abstract class ModbusMasterFixture
	{
		public ModbusMaster Master;
		public SerialPort MasterSerialPort;
		public const string DefaultMasterSerialPortName = "COM6";
		public FtdUsbPort MasterUsbPort;
		public const uint DefaultMasterUsbPortId = 0;
		public TcpClient MasterTcp;
		public UdpClient MasterUdp;

		public ModbusSlave Slave;
		public Thread SlaveThread;
		public SerialPort SlaveSerialPort;
		public TcpListener SlaveTcp;
		public UdpClient SlaveUdp;
		public const string DefaultSlaveSerialPortName = "COM5";
		public const byte SlaveAddress = 1;

		public static readonly IPAddress TcpHost = new IPAddress(new byte[] { 127, 0, 0, 1 });
		public static readonly IPEndPoint DefaultModbusIPEndPoint = new IPEndPoint(TcpHost, Port);
		public const int Port = 502;
		public Process Jamod;

		protected static readonly ILog log = LogManager.GetLogger(typeof(ModbusMasterFixture));

		public virtual double AverageReadTime
		{
			get { return 150; }
		}

		public virtual void Init()
		{
			log4net.Config.XmlConfigurator.Configure();
		}

		public void SetupSlaveSerialPort()
		{
			log.DebugFormat("Configure and open slave serial port {0}.", DefaultSlaveSerialPortName);
			SlaveSerialPort = new SerialPort(DefaultSlaveSerialPortName);
			SlaveSerialPort.Parity = Parity.None;
			SlaveSerialPort.Open();
		}

		public static SerialPort CreateAndOpenSerialPort(string portName)
		{
			SerialPort port = new SerialPort(portName);
			port.Parity = Parity.None;
			port.Open();

			return port;
		}

		public static FtdUsbPort CreateAndOpenUsbPort(uint index)
		{
			FtdUsbPort port = new FtdUsbPort();
			port.OpenByIndex(index);

			return port;
		}

		public void StartSlave()
		{
			log.Debug("Start NModbus slave.");
			SlaveThread = new Thread(Slave.Listen);
			SlaveThread.IsBackground = true;
			SlaveThread.Start();
		}

		public void StartJamodSlave(string program)
		{
			string pathToJamod = Path.Combine(
				Path.GetDirectoryName(Assembly.GetAssembly(typeof(ModbusMasterFixture)).Location)
				, "../../../../tools/jamod");
			string classpath = String.Format(@"-classpath ""{0};{1};{2}""", Path.Combine(pathToJamod, "jamod.jar"), Path.Combine(pathToJamod, "comm.jar"), Path.Combine(pathToJamod, "."));
			ProcessStartInfo startInfo = new ProcessStartInfo("java", String.Format(CultureInfo.InvariantCulture, "{0} {1}", classpath, program));
			Jamod = Process.Start(startInfo);

			Thread.Sleep(4000);
			Assert.IsFalse(Jamod.HasExited, "Jamod Serial Ascii Slave did not start correctly.");
		}

		[TestFixtureTearDown]
		public void CleanUp()
		{
			log.Debug("Clean up after tests.");

			if (Master != null)
				Master.Dispose();

			if (Slave != null)
				Slave.Dispose();

			if (Jamod != null)
			{
				Jamod.Kill();
				Thread.Sleep(4000);
			}
		}

		[Test]
		public virtual void ReadCoils()
		{
			bool[] coils = Master.ReadCoils(SlaveAddress, 2048, 8);
			Assert.AreEqual(new bool[] { false, false, false, false, false, false, false, false }, coils);
		}

		[Test]
		public virtual void ReadInputs()
		{
			bool[] inputs = Master.ReadInputs(SlaveAddress, 150, 3);
			Assert.AreEqual(new bool[] { false, false, false }, inputs);
		}

		[Test]
		public virtual void ReadHoldingRegisters()
		{
			ushort[] registers = Master.ReadHoldingRegisters(SlaveAddress, 104, 2);
			Assert.AreEqual(new ushort[] { 0, 0 }, registers);
		}

		[Test]
		public virtual void ReadInputRegisters()
		{
			ushort[] registers = Master.ReadInputRegisters(SlaveAddress, 104, 2);
			Assert.AreEqual(new ushort[] { 0, 0 }, registers);
		}

		[Test]
		public virtual void WriteSingleCoil()
		{
			bool coilValue = Master.ReadCoils(SlaveAddress, 10, 1)[0];
			Master.WriteSingleCoil(SlaveAddress, 10, !coilValue);
			Assert.AreEqual(!coilValue, Master.ReadCoils(SlaveAddress, 10, 1)[0]);
			Master.WriteSingleCoil(SlaveAddress, 10, coilValue);
			Assert.AreEqual(coilValue, Master.ReadCoils(SlaveAddress, 10, 1)[0]);
		}

		[Test]
		public virtual void WriteSingleRegister()
		{
			ushort testAddress = 200;
			ushort testValue = 350;

			ushort originalValue = Master.ReadHoldingRegisters(SlaveAddress, testAddress, 1)[0];
			Master.WriteSingleRegister(SlaveAddress, testAddress, testValue);
			Assert.AreEqual(testValue, Master.ReadHoldingRegisters(SlaveAddress, testAddress, 1)[0]);
			Master.WriteSingleRegister(SlaveAddress, testAddress, originalValue);
			Assert.AreEqual(originalValue, Master.ReadHoldingRegisters(SlaveAddress, testAddress, 1)[0]);
		}

		[Test]
		public virtual void WriteMultipleRegisters()
		{
			ushort testAddress = 120;
			ushort[] testValues = new ushort[] { 10, 20, 30, 40, 50 };

			ushort[] originalValues = Master.ReadHoldingRegisters(SlaveAddress, testAddress, (ushort) testValues.Length);
			Master.WriteMultipleRegisters(SlaveAddress, testAddress, testValues);
			ushort[] newValues = Master.ReadHoldingRegisters(SlaveAddress, testAddress, (ushort) testValues.Length);
			Assert.AreEqual(testValues, newValues);
			Master.WriteMultipleRegisters(SlaveAddress, testAddress, originalValues);
		}

		[Test]
		public virtual void WriteMultipleCoils()
		{
			ushort testAddress = 200;
			bool[] testValues = new bool[] { true, false, true, false, false, false, true, false, true, false };

			bool[] originalValues = Master.ReadCoils(SlaveAddress, testAddress, (ushort) testValues.Length);
			Master.WriteMultipleCoils(SlaveAddress, testAddress, testValues);
			bool[] newValues = Master.ReadCoils(SlaveAddress, testAddress, (ushort) testValues.Length);
			Assert.AreEqual(testValues, newValues);
			Master.WriteMultipleCoils(SlaveAddress, testAddress, originalValues);
		}

		[Test]
		public virtual void ReadMaximumNumberOfHoldingRegisters()
		{
			ushort[] registers = Master.ReadHoldingRegisters(SlaveAddress, 104, 125);
			Assert.AreEqual(125, registers.Length);
		}

		[Test]
		public virtual void ReadWriteMultipleRegisters()
		{
			ushort startReadAddress = 120;
			ushort numberOfPointsToRead = 5;
			ushort startWriteAddress = 50;
			ushort[] valuesToWrite = new ushort[] { 10, 20, 30, 40, 50 };

			ushort[] valuesToRead = Master.ReadHoldingRegisters(SlaveAddress, startReadAddress, numberOfPointsToRead);
			ushort[] readValues = Master.ReadWriteMultipleRegisters(SlaveAddress, startReadAddress, numberOfPointsToRead, startWriteAddress, valuesToWrite);
			Assert.AreEqual(valuesToRead, readValues);

			ushort[] writtenValues = Master.ReadHoldingRegisters(SlaveAddress, startWriteAddress, (ushort) valuesToWrite.Length);
			Assert.AreEqual(valuesToWrite, writtenValues);
		}

		[Test]
		public virtual void ReadWriteMultipleRegisters_WriteOccursBeforeRead()
		{
			ushort readWriteAddress = 50;
			ushort numberOfPointsToRead = 5;
			ushort[] valuesToWrite = new ushort[] { 10, 20, 30, 40, 50 };

			ushort[] readValues = Master.ReadWriteMultipleRegisters(SlaveAddress, readWriteAddress, numberOfPointsToRead, readWriteAddress, valuesToWrite);
			Assert.AreEqual(valuesToWrite, readValues);
		}

		[Test]
		public virtual void SimpleReadRegistersPerformanceTest()
		{
			int retries = Master.Transport.Retries;
			Master.Transport.Retries = 5;
			double actualAverageReadTime = CalculateAverage(Master);
			Master.Transport.Retries = retries;
			log.InfoFormat("Average read time for {0} - {1}ms", GetType().FullName, actualAverageReadTime);
			Assert.IsTrue(actualAverageReadTime < AverageReadTime, String.Format(CultureInfo.InvariantCulture, "Test failed, actual average read time {0} is greater than expected {1}", actualAverageReadTime, AverageReadTime));
		}

		[Test]
		public virtual void ExecuteCustomMessage_ReadHoldingRegisters()
		{
			CustomReadHoldingRegistersRequest request = new CustomReadHoldingRegistersRequest(SlaveAddress, 104, 2);
			CustomReadHoldingRegistersResponse response = Master.ExecuteCustomMessage<CustomReadHoldingRegistersResponse>(request);
			Assert.AreEqual(new ushort[] { 0, 0 }, response.Data);
		}

		[Test]
		public virtual void ExecuteCustomMessage_Foobar()
		{
			try
			{
				bool delegateExectuted = false;

				Slave.RegisterCustomFunction<CustomFoobarRequest>(CustomFoobarRequest.FoobarFunctionCode,
					(request, dataStore) =>
					{
						delegateExectuted = true;
						
						// get data from data store, make sure to access within a lock to respect multi threaded access
						ushort[] data;
						lock (dataStore.SyncRoot)
							data = dataStore.HoldingRegisters.Slice(request.StartAddress, request.NumberOfPoints).ToArray();

						return new CustomFoobarResponse 
						{ 
							SlaveAddress = request.SlaveAddress, 
							ByteCount = 4, 
							Data = data
						};
					});				

				var foobarRequest = new CustomFoobarRequest(SlaveAddress, 104, 2);
				var foobarResponse = Master.ExecuteCustomMessage<CustomFoobarResponse>(foobarRequest);

				Assert.IsTrue(delegateExectuted);
				Assert.AreEqual(new ushort[] { 0, 0 }, foobarResponse.Data);
			}
			finally
			{
				Slave.UnregisterCustomFunction(CustomFoobarRequest.FoobarFunctionCode);
			}
		}

		[Test]
		public virtual void ExecuteCustomMessage_WriteMultipleRegisters()
		{
			ushort testAddress = 120;
			ushort[] testValues = new ushort[] { 10, 20, 30, 40, 50 };
			CustomReadHoldingRegistersRequest readRequest = new CustomReadHoldingRegistersRequest(SlaveAddress, testAddress, (ushort) testValues.Length);
			CustomWriteMultipleRegistersRequest writeRequest = new CustomWriteMultipleRegistersRequest(SlaveAddress, testAddress, new RegisterCollection(testValues));

			var response = Master.ExecuteCustomMessage<CustomReadHoldingRegistersResponse>(readRequest);
			ushort[] originalValues = response.Data;
			Master.ExecuteCustomMessage<CustomWriteMultipleRegistersResponse>(writeRequest);
			response = Master.ExecuteCustomMessage<CustomReadHoldingRegistersResponse>(readRequest);
			ushort[] newValues = response.Data;
			Assert.AreEqual(testValues, newValues);
			writeRequest = new CustomWriteMultipleRegistersRequest(SlaveAddress, testAddress, new RegisterCollection(originalValues));
			Master.ExecuteCustomMessage<CustomWriteMultipleRegistersResponse>(writeRequest);
		}

		/// <summary>
		/// Perform read registers command 
		/// </summary>
		/// <param name="master"></param>
		/// <returns></returns>
		internal static double CalculateAverage(IModbusMaster master)
		{
			ushort startAddress = 5;
			ushort numRegisters = 5;

			// JIT compile the IL
			master.ReadHoldingRegisters(SlaveAddress, startAddress, numRegisters);

			Stopwatch stopwatch = new Stopwatch();
			long sum = 0;
			double numberOfReads = 50;

			for (int i = 0; i < numberOfReads; i++)
			{
				stopwatch.Reset();
				stopwatch.Start();
				master.ReadHoldingRegisters(SlaveAddress, startAddress, numRegisters);
				stopwatch.Stop();
				log.DebugFormat("CalculateAverage read {0}", i + 1);

				checked
				{
					sum += stopwatch.ElapsedMilliseconds;
				}
			}

			return sum / numberOfReads;
		}
	}
}
