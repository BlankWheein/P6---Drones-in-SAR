using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using BetterTelloLib.Commander.Events;
using BetterTelloLib.Commander.Factories;
using BetterTelloLib.Udp;

namespace BetterTelloLib.Commander
{
    public class BetterTello
    {
        public TelloState State;
        public TelloSdk30ControlCommands Commands;
        public BetterTelloEvents Events = new();
        public BetterTelloFactories Factories;
        public const string DefaultTelloAddress = "192.168.10.1";
        public const int DefaultTelloPort = 8889;
        public const int DefaultTelloStatePort = 8890;
        public const int DefaultTelloVideoPort = 11111;



        private readonly string _address;
        private readonly int _port;
        internal readonly UdpClient _client = new();
        internal readonly UdpListener stateServer;
        internal readonly UdpListener videoServer;
        internal IPEndPoint sender = new(IPAddress.Any, 0);
        internal static CancellationTokenSource cancelTokens;

        public BetterTello()
        {
            _address = DefaultTelloAddress;
            _port = DefaultTelloPort;
            State = new(this);
            stateServer = new (DefaultTelloStatePort);
            videoServer = new(DefaultTelloVideoPort);
            Commands = new(_client, this);
            Factories = new(this);
        }
        public void Connect()
        {
            _client.Connect(IPAddress.Parse(_address), _port);
            Commands.Command();
            Commands.StreamOn();
            Factories.StartFactories();
        }

        public void Dispose()
        {
            //SendCommand("quit");
            _client.Close();
        }

        internal int SendCommand(string command) => _client.Send(command);
    }
}