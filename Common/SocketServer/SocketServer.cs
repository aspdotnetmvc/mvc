using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using BXM.TCPAsyncServer;
using BXM.ScoketProtocol;

namespace SocketServer
{
    public sealed class SocketServer:ISocketAsyncServer
    {
        public event DataReceivedHandler DataReceived;
        public event ClosedHandler Closed;
        public event ConnectedHandler Connected;
        public event ErrorHandler Error;

        TCPAsyncServer _socket;
        IProtocol _protocol;
        /// <summary>
        /// 构造函数，建立一个未初始化的服务器实例
        /// </summary>
        /// <param name="numConnections">服务器的最大连接数据</param>
        /// <param name="bufferSize"></param>
        public SocketServer(Int32 numConnections, Int32 bufferSize,Int32 port,string ip)
        {
            _protocol = new BaseProtocol();
            _protocol.Receive += _protocol_Receive;
            _socket = new TCPAsyncServer(port, numConnections, bufferSize, _protocol, 0);
        }

        private bool _protocol_Receive(byte[] data, int sessionId)
        {
            if(DataReceived!=null)
            {
                DataReceived(sessionId, data);
            }
            return true;
        }

        void OnError(Exception ex)
        {
            if (Error != null)
            {
                Error(ex);
            }
        }

        public void Send(int client, byte[] data)
        {
            _socket.Send(data, 0, data.Length, 0);
        }

        public void Start()
        {
            _socket.Start();
        }

        public void Dispose()
        {
            _socket = null;
            _protocol = null;
        }
    }
}
