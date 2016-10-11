using System;
namespace SocketAsyncClient
{
    public delegate void DataReceivedHandler(byte[] data);
    public delegate void ClosedHandler();
    public delegate void ConnectedHandler();
    public delegate void ErrorHandler(Exception error);

    public interface ISocketAsyncClient
    {
        event DataReceivedHandler DataReceived;
        event ClosedHandler Closed;
        event ConnectedHandler Connected;
        event ErrorHandler Error;

        bool IsConnected { get; }

        bool Connect();
        void Close();
        void Send(byte[] data);
    }
}
