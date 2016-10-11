using BXM.ScoketProtocol;
using BXM.TCPAsyncClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketAsyncClient
{
    public class SocketClient: ISocketAsyncClient
    {
        public event DataReceivedHandler DataReceived;
        public event ClosedHandler Closed;
        public event ConnectedHandler Connected;
        public event ErrorHandler Error;


        TCPAsyncClient _client;
        BaseProtocol protocol;

        public SocketClient(string ip,Int32 port,Int32 bufferSize)
        {
            protocol = new BaseProtocol();
            _client = new TCPAsyncClient(ip, port, bufferSize, protocol);
            _client.Connected += client_Connected;
            _client.Closed += client_Closed;
            _client.Error += client_Error;
            protocol.Receive += protocol_ReceiveData;
        }

        public SocketClient(string ip, Int32 port, Int32 bufferSize,string localIp)
        {
            protocol = new BaseProtocol();
            _client = new TCPAsyncClient(ip, port, bufferSize, protocol,localIp,0);
            _client.Connected += client_Connected;
            _client.Closed += client_Closed;
            _client.Error += client_Error;
            protocol.Receive += protocol_ReceiveData;
        }

        bool protocol_ReceiveData(byte[] data, int sessionId)
        {
            if (DataReceived != null)
            {
                DataReceived(data);
            }
            return true;
        }

        void client_Error(Exception error)
        {
            if (Error != null)
            {
                Error(error);
            }
        }

        void client_Closed()
        {
            if (Closed != null)
            {
                Closed();
            }
        }

        void client_Connected()
        {
            if (Connected != null)
            {
                Connected();
            }
        }

        public bool Connect()
        {
            bool r = _client.Connect();
            Thread.Sleep(100);
            return r;
        }

        public void Close()
        {
            _client.Close();
            Thread.Sleep(100);
        }

        public void Send(byte[] data)
        {
            _client.Send(data,0,data.Length);
        }

        public bool IsConnected
        {
            get
            {
                return _client.IsConnected;
            }
        }
    }
}
