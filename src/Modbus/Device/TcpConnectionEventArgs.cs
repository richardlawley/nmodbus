using System;
using Unme.Common;

namespace Modbus.Device
{
	internal class TcpConnectionEventArgs : EventArgs
	{
		public TcpConnectionEventArgs(string endPoint)
		{
			if (endPoint == null)
				throw new ArgumentNullException("endPoint");
			if (endPoint.IsNullOrEmpty())
				throw new ArgumentException(Resources.EmptyEndPoint);

			EndPoint = endPoint;
		}

		public string EndPoint { get; set; }
	}
}
