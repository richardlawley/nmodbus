using System;
using System.Globalization;
using Modbus.Data;
using Unme.Common;

namespace Modbus.Message
{
	internal class CustomMessageInfo
	{
		private readonly Type _type;
		private readonly Func<IModbusMessage, DataStore, IModbusMessage> _applyRequest;
		private readonly Func<IModbusMessageRtu> _instanceGetter;

		public CustomMessageInfo(Type type, Func<IModbusMessage, DataStore, IModbusMessage> applyRequest)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (applyRequest == null)
				throw new ArgumentNullException("applyRequest");

			_type = type;
			_applyRequest = applyRequest;

			// lazily initialize the instance, this will only be needed for the RTU protocol so 
			// we don't actually require the type argument to implement IModbusMessageRtu
			_instanceGetter = FunctionalUtility.Memoize(() =>
			{
				if (!typeof(IModbusMessageRtu).IsAssignableFrom(type))
				{
					throw new ArgumentException(
						String.Format(CultureInfo.InvariantCulture,
						"Custom type {0} needs to implement the {1} interface.",
						type.Name,
						typeof(IModbusMessageRtu).Name));
				}

				return (IModbusMessageRtu) Activator.CreateInstance(Type);
			});
		}

		public Type Type
		{
			get
			{
				return _type;
			}
		}

		public Func<IModbusMessage, DataStore, IModbusMessage> ApplyRequest
		{
			get
			{
				return _applyRequest;
			}
		}

		public IModbusMessageRtu Instance
		{
			get
			{
				return _instanceGetter.Invoke();
			}
		}
	}
}
