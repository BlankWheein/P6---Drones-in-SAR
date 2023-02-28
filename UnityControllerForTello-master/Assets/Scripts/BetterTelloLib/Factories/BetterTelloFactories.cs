using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BetterTelloLib.Commander.Events.EventArgs;

namespace BetterTelloLib.Commander.Factories
{
    public class BetterTelloFactories
    {
        private BetterTello tello;

        public BetterTelloFactories(BetterTello tello)
        {
            this.tello = tello;
        }

        internal void StartFactories()
        {
            BetterTello.cancelTokens = new CancellationTokenSource();
            CancellationToken token = BetterTello.cancelTokens.Token;
            StateFactory(token);
            EXTTofFactory(token);
            CommandFactory(token);
            VideoFeedFactory(token);
        }
        private void VideoFeedFactory(CancellationToken token)
        {
            tello.SendCommand("streamon");
            Task.Factory.StartNew(() => // Video Feed Factory
            {
                while (true)
                {
                    try
                    {
                        if (token.IsCancellationRequested)
                            break;
                        var received = tello.videoServer.Receive(ref tello.sender);
                        VideoDataRecievedEventArgs eventArgs = new(received);
                        tello.Events.VideoDataRecieved(eventArgs);
                    }
                    catch (Exception e) { Console.WriteLine(e); }
                }
            }, token);
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
                        var received = tello.stateServer.Receive(ref tello.sender);
                        var state = Encoding.ASCII.GetString(received, 0, received.Length);
                        //tello.log.LogDebug("Reciewed raw state: {}", state);
                        tello.State.ParseState(state);
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
                        tello.SendCommand("EXT tof?");
                        //tello.log.LogDebug($"Sent command: EXT tof?");

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
                        string response = tello._client.Read();
                        //tello.log.LogInformation("Got response: {}", response);

                        if (response.Contains("tof"))
                            tello.State.ParseExtTof(response);
                    }
                    catch (Exception e) { Console.WriteLine(e); }
                }
            }, token);
        }
    }
}