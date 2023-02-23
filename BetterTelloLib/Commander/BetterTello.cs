using System.Data;
using System.Data.Common;
using System.Net;
using System.Net.Sockets;
using System.Text;
using BetterTelloLib.Udp;

namespace BetterTelloLib.Commander
{
    public class BetterTello : IDisposable
    {
        public const string DefaultTelloAddress = "192.168.10.1";
        public const int DefaultTelloPort = 8889;

        private readonly string _address;
        private readonly int _port;
        private readonly TelloUdpClient _client = new ();
        private const string HeightUnits = "dm";
        public TelloState State { get; set; } = new();

        private static CancellationTokenSource cancelTokens = new ();

        private static CancellationTokenSource masterCancelTokens = new ();
        private const string ApiModeCommand = "command";
        public const string LandCommand = "land";
        public const string DownCommand = "down";
        public const string GetHeightCommand = "height?";
        public const string RunScriptCommand = "runscript";
        public const string HistoryCommand = "history";
        public const string WriteHistoryCommand = "writehistory";
        public const string WaitCommand = "wait";
        public const string ForceFailCommand = "forcefail";
        public const string ReceiveTimeoutCommand = "receivetimeout";
        public const string SendTimeoutCommand = "sendtimeout";
        private static DateTime lastMessageTime;//for connection timeouts.


        public void StartListeners()
        {
            cancelTokens = new CancellationTokenSource();
            CancellationToken token = cancelTokens.Token;
            // State factory
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        if (token.IsCancellationRequested)
                            break;
                        var received = stateServer.Receive(ref sender);
                        State.ParseState(Encoding.ASCII.GetString(received, 0, received.Length));
                    }
                    catch (Exception e) { Console.WriteLine(e); }
                }
            });

            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    try
                    {
                        if (token.IsCancellationRequested)
                            break;
                        _client.Send("EXT tof?");
                    }
                    catch (Exception e) { Console.WriteLine(e); }
                    await Task.Delay(1000);
                }
            });

            // Command response Factory
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        if (token.IsCancellationRequested)
                            break;
                        string response = _client.Read();

                        if (response.Contains("tof"))
                            State.ParseExtTof(response);

                        if (response.Contains("unknown"))
                        {

                        }
                    }
                    catch (Exception e) { Console.WriteLine(e); }
                }
            });
        }



        public BetterTello()
        {
            _address = DefaultTelloAddress;
            _port = DefaultTelloPort;
        }
        private IPEndPoint ipep;
        private UdpClient stateServer;
        private IPEndPoint sender;
        public void Connect()
        {
            _client.Connect(IPAddress.Parse(_address), _port);
            _client.Send(ApiModeCommand);
            ipep = new IPEndPoint(IPAddress.Any, 8890);
            stateServer = new UdpClient(ipep);
            sender = new IPEndPoint(IPAddress.Any, 0);
            StartListeners();
        }

        public void Dispose()
        {
            SendCommand("quit");
            _client.Close();
        }

        public string SendCommand(string command, bool print = true)
        {
            _client.Send(command);
            string response = _client.Read();
            if (print)
            Console.WriteLine(command + ": " + response);
            return response;
        }
        public int GetHeight()
        {
            string response = SendCommand(GetHeightCommand, true).Replace(HeightUnits, "");
            int.TryParse(response, out int height);
            height *= 10;
            //Console.WriteLine($"Height is: {height} ");
            return height;
        }
    }
}