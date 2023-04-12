



using System.Net;
using System.Net.Sockets;

namespace BetterTelloLib.Udp
{

    public class TelloUdpClient : TelloUdpConnection
    {
        public void Connect(IPAddress remoteAddress, int port)
        {
            _sendEndpoint = new IPEndPoint(remoteAddress, port);
            _receiveEndpoint = new IPEndPoint(IPAddress.Any, 0);
            _client = new UdpClient();
            _client.Client.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.Debug, true);
            _client.Connect(_sendEndpoint);
        }
    }
}