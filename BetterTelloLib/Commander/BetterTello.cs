using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using BetterTelloLib.Udp;
using Microsoft.Extensions.Logging;

namespace BetterTelloLib.Commander
{
    public class BetterTello : IDisposable
    {
        public TelloState State { get; set; } = new();
        public TelloSdk30ControlCommands Commands;
        public const string DefaultTelloAddress = "192.168.10.1";
        public const int DefaultTelloPort = 8889;

        internal ILogger log;

        private static CancellationTokenSource cancelTokens = new();
        private readonly string _address;
        private readonly int _port;
        private readonly TelloUdpClient _client = new();
        private readonly IPEndPoint ipep = new(IPAddress.Any, 8890);
        private readonly UdpClient stateServer;
        private IPEndPoint sender = new(IPAddress.Any, 0);
        public BetterTello()
        {
            _address = DefaultTelloAddress;
            _port = DefaultTelloPort;
            stateServer = new UdpClient(ipep);
            Commands = new(_client);
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug);
            });
            log = loggerFactory.CreateLogger<BetterTello>();
        }
        public void Connect()
        {
            _client.Connect(IPAddress.Parse(_address), _port);
            _client.Send("command");
            StartFactories();
        }
        public void StartFactories()
        {
            cancelTokens = new CancellationTokenSource();
            CancellationToken token = cancelTokens.Token;
            StateFactory(token);
            EXTTofFactory(token);
            CommandFactory(token);
        }

        private void StateFactory(CancellationToken token)
        {
            Task.Factory.StartNew(() => // State factory
            {
                while (true)
                {
                    try
                    {
                        if (token.IsCancellationRequested)
                            break;
                        var received = stateServer.Receive(ref sender);
                        var state = Encoding.ASCII.GetString(received, 0, received.Length);
                        log.LogDebug("Reciewed raw state: {}", state);
                        State.ParseState(state);
                    }
                    catch (Exception e) { Console.WriteLine(e); }
                }
            }, token);
        }
        private void EXTTofFactory(CancellationToken token)
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    try
                    {
                        if (token.IsCancellationRequested)
                            break;
                        _client.Send("EXT tof?");
                        log.LogDebug($"Sent command: EXT tof?");

                    }
                    catch (Exception e) { Console.WriteLine(e); }
                    await Task.Delay(100);
                }
            }, token);
        }
        private void CommandFactory(CancellationToken token)
        {
            Task.Factory.StartNew(() => // Command response Factory
            {
                while (true)
                {
                    try
                    {
                        if (token.IsCancellationRequested)
                            break;
                        string response = _client.Read();
                        log.LogInformation("Got response: {}", response);

                        if (response.Contains("tof"))
                            State.ParseExtTof(response);
                    }
                    catch (Exception e) { Console.WriteLine(e); }
                }
            }, token);
        }
        

        public void Dispose()
        {
            //SendCommand("quit");
            GC.SuppressFinalize(this);
            _client.Close();
        }

        public int SendCommand(string command) => _client.Send(command);
    }
}