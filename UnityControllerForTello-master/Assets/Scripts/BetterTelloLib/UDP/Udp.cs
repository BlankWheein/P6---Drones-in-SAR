using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public struct Received
{
    public IPEndPoint Sender;
    public string Message;
    public byte[] bytes;
}

public abstract class UdpBase
{
    public UdpClient Client;

    protected UdpBase()
    {
        Client = new UdpClient();
        Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Debug, 15000);

    }

    public async Task<Received> Receive()
    {
        var result = await Client.ReceiveAsync();
        return new Received()
        {
            bytes = result.Buffer.ToArray(),
            Message = Encoding.ASCII.GetString(result.Buffer, 0, result.Buffer.Length),
            Sender = result.RemoteEndPoint
        };
    }

}

//Server
public class UdpListener : UdpBase
{
    private IPEndPoint _listenOn;

    public UdpListener(int port) : this(new IPEndPoint(IPAddress.Any, port))
    {
    }

    public UdpListener(IPEndPoint endpoint)
    {
        _listenOn = endpoint;
        Client = new UdpClient(_listenOn);
    }

    public void Reply(string message, IPEndPoint endpoint)
    {
        var datagram = Encoding.ASCII.GetBytes(message);
        Client.Send(datagram, datagram.Length, endpoint);
    }

}

//Client
public class UdpUser : UdpBase
{
    public UdpUser() { }

    public static UdpUser ConnectTo(string hostname, int port)
    {
        var connection = new UdpUser();
        connection.Client.Connect(hostname, port);
        return connection;
    }

    public int Send(string message)
    {
        var datagram = Encoding.UTF8.GetBytes(message);
        Client.Send(datagram, datagram.Length);
        return 1;
    }
    public void Send(byte[] message)
    {
        var mstr = Encoding.ASCII.GetString(message);
        Client.Send(message, message.Length);
    }
    public byte[] RecieveNew(ref IPEndPoint ipep)
    {
        return Client.Receive(ref ipep);
    }
}