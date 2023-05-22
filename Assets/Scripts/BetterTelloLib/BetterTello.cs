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
        public static UdpUser Client;
        internal static UdpListener stateServer;
        internal static UdpListenerVideo videoServer;
        internal IPEndPoint sender = new(IPAddress.Any, 0);
        internal static CancellationTokenSource cancelTokens;

        public BetterTello()
        {
            _address = DefaultTelloAddress;
            _port = DefaultTelloPort;
        }
        public void Connect()
        {
            Client = UdpUser.ConnectTo(_address, _port);
            Commands = new(Client, this);
            Factories = new(this);
            Commands.Command();
            Commands.StreamOn();
            State = new(this);
            stateServer = new (8890);
            videoServer = new (11111);
            Factories.StartFactories();
        }

        public void Dispose()
        {
        }

        internal int SendCommand(string command) => Client.Send(command);
    }
}