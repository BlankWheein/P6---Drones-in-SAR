using BetterTelloLib.Udp;

namespace BetterTelloLib.Commander
{
    public class TelloSdk30ControlCommands
    {
        private readonly TelloUdpClient _client;
        public TelloSdk30ControlCommands(TelloUdpClient client)
        {
            _client = client;
        }
    }
}