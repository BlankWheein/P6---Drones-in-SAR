using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using BetterTelloLib.Commander.Events;
using BetterTelloLib.Commander.Factories;
using BetterTelloLib.Udp;
using Microsoft.Extensions.Logging;

namespace BetterTelloLib.Commander
{
    public class BetterTello : IDisposable
    {
        public TelloState State = new();
        public TelloSdk30ControlCommands Commands;
        public BetterTelloEvents Events = new();
        public BetterTelloFactories Factories;
        public const string DefaultTelloAddress = "192.168.10.1";
        public const int DefaultTelloPort = 8889;
        public const int DefaultTelloStatePort = 8890;
        public const int DefaultTelloVideoPort = 11111;


        internal ILogger log;

        internal static CancellationTokenSource cancelTokens = new();
        private readonly string _address;
        private readonly int _port;
        internal readonly TelloUdpClient _client = new();
        internal readonly IPEndPoint stateIpEp = new(IPAddress.Any, DefaultTelloStatePort);
        internal readonly IPEndPoint videoIpEp = new(IPAddress.Any, DefaultTelloVideoPort);
        internal readonly UdpClient stateServer;
        internal readonly UdpClient videoServer;
        internal IPEndPoint sender = new(IPAddress.Any, 0);
        public BetterTello()
        {
            _address = DefaultTelloAddress;
            _port = DefaultTelloPort;
            stateServer = new UdpClient(stateIpEp);
            videoServer = new UdpClient(videoIpEp);
            Commands = new(_client);
            Factories = new(this);
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug);
            });
            log = loggerFactory.CreateLogger<Program>();
        }
        public void Connect()
        {
            log.LogInformation("Trying to connect to Tello");
            _client.Connect(IPAddress.Parse(_address), _port);
            _client.Send("command");
            _client.Send("streamoff");
            Factories.StartFactories();
        }

        public void Dispose()
        {
            //SendCommand("quit");
            GC.SuppressFinalize(this);
            _client.Close();
        }

        internal int SendCommand(string command) => _client.Send(command);
    }
}