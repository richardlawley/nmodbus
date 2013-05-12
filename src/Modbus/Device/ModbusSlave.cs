using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using log4net;
using Modbus.Data;
using Modbus.IO;
using Modbus.Message;
using Unme.Common;

namespace Modbus.Device
{
	/// <summary>
	/// Modbus slave device.
	/// </summary>
	public abstract class ModbusSlave : ModbusDevice
	{	
		private static readonly ILog _logger = LogManager.GetLogger(typeof(ModbusSlave));
		private readonly Dictionary<byte, CustomMessageInfo> _customMessages = new Dictionary<byte, CustomMessageInfo>();

		private Func<Type, MethodInfo> _createModbusMessageCache = FunctionalUtility.Memoize((Type type) =>
		{
			MethodInfo method = typeof(ModbusMessageFactory).GetMethod("CreateModbusMessage");
			return method.MakeGenericMethod(type);
		});

		internal ModbusSlave(byte unitId, ModbusTransport transport)
			: base(transport)
		{
			DataStore = DataStoreFactory.CreateDefaultDataStore();
			UnitId = unitId;
		}

		/// <summary>
		/// Occurs when a modbus slave receives a request.
		/// </summary>
		public event EventHandler<ModbusSlaveRequestEventArgs> ModbusSlaveRequestReceived;

		/// <summary>
		/// Gets or sets the data store.
		/// </summary>
		public DataStore DataStore { get; set; }

		/// <summary>
		/// Gets or sets the unit ID.
		/// </summary>
		public byte UnitId { get; set; }

		/// <summary>
		/// Start slave listening for requests.
		/// </summary>
		public abstract void Listen();

		/// <summary>
		/// Registers specified custom function.
		/// </summary>
		/// <typeparam name="TRequest">The type of the modbus request message.</typeparam>
		/// <param name="functionCode">The function code to provide the implementation for.</param>
		/// <param name="applyRequest">The delegate to apply the request to the DataStore and return the appropriate response.</param>
		/// <exception cref="ArgumentException">Custom function registration already exists.</exception>
		public void RegisterCustomFunction<TRequest>(byte functionCode, Func<TRequest, DataStore, IModbusMessage> applyRequest)
			where TRequest : IModbusMessage, new()
		{
			if (applyRequest == null)
				throw new ArgumentNullException("applyRequest");
			if (_customMessages.ContainsKey(functionCode))
				throw new ArgumentException("A custom function already exists with the specified function code. You must unregister it first.", "functionCode");

			// CONSIDER only allowing true user-defined function codes, Modbus defines 65-72 and 100-110 as user-defined function codes.

			// wrap in more generic delegate type
			Func<IModbusMessage, DataStore, IModbusMessage> wrapper = (message, dataStore) => applyRequest((TRequest) message, dataStore);

			_customMessages[functionCode] = new CustomMessageInfo(typeof(TRequest), wrapper);
		}

		/// <summary>
		/// Unregisters specified custom function.
		/// </summary>
		/// <param name="functionCode">The function code.</param>
		/// <exception cref="KeyNotFoundException">The specified function code is not registered.</exception>
		public void UnregisterCustomFunction(byte functionCode)
		{
			if (!_customMessages.Remove(functionCode))
				throw new KeyNotFoundException(String.Format(CultureInfo.InvariantCulture, "Specified function code {0} is not registered.", functionCode));
		}		

		internal static ReadCoilsInputsResponse ReadDiscretes(ReadCoilsInputsRequest request, DataStore dataStore, ModbusDataCollection<bool> dataSource)
		{
			DiscreteCollection data = DataStore.ReadData<DiscreteCollection, bool>(dataStore, dataSource, request.StartAddress, request.NumberOfPoints, dataStore.SyncRoot);
			ReadCoilsInputsResponse response = new ReadCoilsInputsResponse(request.FunctionCode, request.SlaveAddress, data.ByteCount, data);

			return response;
		}

		internal static ReadHoldingInputRegistersResponse ReadRegisters(ReadHoldingInputRegistersRequest request, DataStore dataStore, ModbusDataCollection<ushort> dataSource)
		{
			RegisterCollection data = DataStore.ReadData<RegisterCollection, ushort>(dataStore, dataSource, request.StartAddress, request.NumberOfPoints, dataStore.SyncRoot);
			ReadHoldingInputRegistersResponse response = new ReadHoldingInputRegistersResponse(request.FunctionCode, request.SlaveAddress, data);

			return response;
		}

		internal static WriteSingleCoilRequestResponse WriteSingleCoil(WriteSingleCoilRequestResponse request, DataStore dataStore, ModbusDataCollection<bool> dataSource)
		{
			DataStore.WriteData(dataStore, new DiscreteCollection(request.Data[0] == Modbus.CoilOn), dataSource, request.StartAddress, dataStore.SyncRoot);

			return request;
		}

		internal static WriteMultipleCoilsResponse WriteMultipleCoils(WriteMultipleCoilsRequest request, DataStore dataStore, ModbusDataCollection<bool> dataSource)
		{
			DataStore.WriteData(dataStore, request.Data.Take(request.NumberOfPoints), dataSource, request.StartAddress, dataStore.SyncRoot);
			WriteMultipleCoilsResponse response = new WriteMultipleCoilsResponse(request.SlaveAddress, request.StartAddress, request.NumberOfPoints);

			return response;
		}

		internal static WriteSingleRegisterRequestResponse WriteSingleRegister(WriteSingleRegisterRequestResponse request, DataStore dataStore, ModbusDataCollection<ushort> dataSource)
		{
			DataStore.WriteData(dataStore, request.Data, dataSource, request.StartAddress, dataStore.SyncRoot);

			return request;
		}

		internal static WriteMultipleRegistersResponse WriteMultipleRegisters(WriteMultipleRegistersRequest request, DataStore dataStore, ModbusDataCollection<ushort> dataSource)
		{
			DataStore.WriteData(dataStore, request.Data, dataSource, request.StartAddress, dataStore.SyncRoot);
			WriteMultipleRegistersResponse response = new WriteMultipleRegistersResponse(request.SlaveAddress, request.StartAddress, request.NumberOfPoints);

			return response;
		}

		internal bool TryApplyCustomMessage(IModbusMessage request, DataStore dataStore, out IModbusMessage response)
		{
			if (request == null)
				throw new ArgumentNullException("request");
			if (dataStore == null)
				throw new ArgumentNullException("dataStore");


			bool requestApplied = false;
			response = null;

			CustomMessageInfo messageInfo;
			if (TryGetCustomMessageInfo(request.FunctionCode, out messageInfo))
			{
				response = messageInfo.ApplyRequest(request, dataStore);
				requestApplied = true;
			}

			return requestApplied;
		}
		
		internal bool TryCreateModbusMessageRequest(byte functionCode, byte[] frame, out IModbusMessage request)
		{
			if (frame == null)
				throw new ArgumentNullException("frame");

			bool messageCreated = false;
			request = null;

			CustomMessageInfo messageInfo;
			if (TryGetCustomMessageInfo(functionCode, out messageInfo))
			{
				request = (IModbusMessage) _createModbusMessageCache(messageInfo.Type).Invoke(null, new object[] { frame });

				messageCreated = true;
			}

			return messageCreated;
		}

		internal bool TryGetCustomMessageInfo(byte functionCode, out CustomMessageInfo messageInfo)
		{
			return _customMessages.TryGetValue(functionCode, out messageInfo);
		}

		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Cast is not unneccessary.")]
		internal IModbusMessage ApplyRequest(IModbusMessage request)
		{
			if (request == null)
				throw new ArgumentNullException("request");

			_logger.Info(request.ToString());
			ModbusSlaveRequestReceived.Raise(this, new ModbusSlaveRequestEventArgs(request));
			IModbusMessage response;

			// allow custom function override
			if (!TryApplyCustomMessage(request, DataStore, out response))
			{
				// default implementation
				switch (request.FunctionCode)
				{
					case Modbus.ReadCoils:
						response = ReadDiscretes((ReadCoilsInputsRequest) request, DataStore, DataStore.CoilDiscretes);
						break;
					case Modbus.ReadInputs:
						response = ReadDiscretes((ReadCoilsInputsRequest) request, DataStore, DataStore.InputDiscretes);
						break;
					case Modbus.ReadHoldingRegisters:
						response = ReadRegisters((ReadHoldingInputRegistersRequest) request, DataStore, DataStore.HoldingRegisters);
						break;
					case Modbus.ReadInputRegisters:
						response = ReadRegisters((ReadHoldingInputRegistersRequest) request, DataStore, DataStore.InputRegisters);
						break;
					case Modbus.Diagnostics:
						response = request;
						break;
					case Modbus.WriteSingleCoil:
						response = WriteSingleCoil((WriteSingleCoilRequestResponse) request, DataStore, DataStore.CoilDiscretes);
						break;
					case Modbus.WriteSingleRegister:
						response = WriteSingleRegister((WriteSingleRegisterRequestResponse) request, DataStore, DataStore.HoldingRegisters);
						break;
					case Modbus.WriteMultipleCoils:
						response = WriteMultipleCoils((WriteMultipleCoilsRequest) request, DataStore, DataStore.CoilDiscretes);
						break;
					case Modbus.WriteMultipleRegisters:
						response = WriteMultipleRegisters((WriteMultipleRegistersRequest) request, DataStore, DataStore.HoldingRegisters);
						break;
					case Modbus.ReadWriteMultipleRegisters:
						ReadWriteMultipleRegistersRequest readWriteRequest = (ReadWriteMultipleRegistersRequest) request;
						WriteMultipleRegisters(readWriteRequest.WriteRequest, DataStore, DataStore.HoldingRegisters);
						response = ReadRegisters(readWriteRequest.ReadRequest, DataStore, DataStore.HoldingRegisters);
						break;
					default:
						string errorMessage = String.Format(CultureInfo.InvariantCulture, "Unsupported function code {0}", request.FunctionCode);
						_logger.Error(errorMessage);
						throw new ArgumentException(errorMessage, "request");
				}
			}

			return response;
		}
	}
}
