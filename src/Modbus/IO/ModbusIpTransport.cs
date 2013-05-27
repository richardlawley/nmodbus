using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using log4net;
using Modbus.Device;
using Modbus.Message;
using Unme.Common;

namespace Modbus.IO
{
    /// <summary>
    /// Transport for Internet protocols.
    /// Refined Abstraction - http://en.wikipedia.org/wiki/Bridge_Pattern
    /// </summary>
    internal class ModbusIpTransport : ModbusTransport
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ModbusIpTransport));
        private static readonly object _transactionIdLock = new object();
        private const int REREADTHRESHOLD = 3;
        private ushort _transactionId;

        internal ModbusIpTransport(IStreamResource streamResource)
            : base(streamResource)
        {
            Debug.Assert(streamResource != null, "Argument streamResource cannot be null.");
        }

        internal static byte[] ReadRequestResponse(IStreamResource streamResource)
        {
            // read header
            byte[] mbapHeader = new byte[6];
            int numBytesRead = 0;
            while (numBytesRead != 6)
            {
                numBytesRead += streamResource.Read(mbapHeader, numBytesRead, 6 - numBytesRead);

                if (numBytesRead == 0)
                    throw new IOException("Read resulted in 0 bytes returned.");
            }
            _logger.DebugFormat("MBAP header: {0}", mbapHeader.Join(", "));

            ushort frameLength = (ushort)IPAddress.HostToNetworkOrder(BitConverter.ToInt16(mbapHeader, 4));
            _logger.DebugFormat("{0} bytes in PDU.", frameLength);

            // read message
            byte[] messageFrame = new byte[frameLength];
            numBytesRead = 0;
            while (numBytesRead != frameLength)
            {
                numBytesRead += streamResource.Read(messageFrame, numBytesRead, frameLength - numBytesRead);

                if (numBytesRead == 0)
                    throw new IOException("Read resulted in 0 bytes returned.");
            }
            _logger.DebugFormat("PDU: {0}", frameLength);

            byte[] frame = mbapHeader.Concat(messageFrame).ToArray();
            _logger.DebugFormat("RX: {0}", frame.Join(", "));

            return frame;
        }

        internal static byte[] GetMbapHeader(IModbusMessage message)
        {
            byte[] transactionId = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)message.TransactionId));
            byte[] protocol = { 0, 0 };
            byte[] length = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)(message.ProtocolDataUnit.Length + 1)));

            return transactionId.Concat(protocol, length, new byte[] { message.SlaveAddress }).ToArray();
        }

        /// <summary>
        /// Create a new transaction ID.
        /// </summary>
        internal virtual ushort GetNewTransactionId()
        {
            lock (_transactionIdLock)
                _transactionId = _transactionId == UInt16.MaxValue ? (ushort)1 : ++_transactionId;

            return _transactionId;
        }

        internal IModbusMessage CreateMessageAndInitializeTransactionId<T>(byte[] fullFrame) where T : IModbusMessage, new()
        {
            byte[] mbapHeader = fullFrame.Slice(0, 6).ToArray();
            byte[] messageFrame = fullFrame.Slice(6, fullFrame.Length - 6).ToArray();

            IModbusMessage response = CreateResponse<T>(messageFrame);
            response.TransactionId = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(mbapHeader, 0));

            return response;
        }

        internal override byte[] BuildMessageFrame(IModbusMessage message)
        {
            List<byte> messageBody = new List<byte>();
            messageBody.AddRange(GetMbapHeader(message));
            messageBody.AddRange(message.ProtocolDataUnit);

            return messageBody.ToArray();
        }

        internal override void Write(IModbusMessage message)
        {
            message.TransactionId = GetNewTransactionId();
            byte[] frame = BuildMessageFrame(message);
            _logger.DebugFormat("TX: {0}", frame.Join(", "));
            StreamResource.Write(frame, 0, frame.Length);
        }

        internal override byte[] ReadRequest(ModbusSlave slave)
        {
            return ReadRequestResponse(StreamResource);
        }

        internal override IModbusMessage ReadResponse<T>()
        {
            return CreateMessageAndInitializeTransactionId<T>(ReadRequestResponse(StreamResource));
        }

        internal override bool OnValidateResponse(IModbusMessage request, IModbusMessage response)
        {
            if (request.TransactionId == response.TransactionId)
            {
                return true;
            }
            else if (request.TransactionId > response.TransactionId && (request.TransactionId - response.TransactionId) <= REREADTHRESHOLD)
            {
                _logger.InfoFormat("Response ({0}) was behind request ({1}) - requesting re-read", response.TransactionId, request.TransactionId);
                return false;
            }

            throw new IOException(String.Format(CultureInfo.InvariantCulture, "Response was not of expected transaction ID. Expected {0}, received {1}.", request.TransactionId, response.TransactionId));
        }
    }
}
