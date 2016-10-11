using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace SocketServer
{
    public delegate void DataReceivedHandler(int client, byte[] data);
    public delegate void ClosedHandler();
    public delegate void ConnectedHandler();
    public delegate void ErrorHandler(Exception error);

    public interface ISocketAsyncServer : IDisposable
    {
        void Send(int client, byte[] data);
        event DataReceivedHandler DataReceived;
        event ClosedHandler Closed;
        event ConnectedHandler Connected;
        event ErrorHandler Error;

        void Start();
    }
}
