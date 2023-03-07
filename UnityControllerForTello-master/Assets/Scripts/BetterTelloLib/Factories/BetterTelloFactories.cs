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
        public int ExtTofDelay = 100;

        public BetterTelloFactories(BetterTello tello)
        {
            this.tello = tello;
        }

        public event EventHandler<TaskRecievedEventArgs>? OnTaskRecieved;
        internal virtual void TaskRecieved(TaskRecievedEventArgs e)
        {
            OnTaskRecieved?.Invoke(this, e);
            if (e.Received == "ok")
                OkRecieved(e);
        }

        public event EventHandler<TaskRecievedEventArgs>? OnOkRecieved;
        internal virtual void OkRecieved(TaskRecievedEventArgs e)
        {
            OnOkRecieved?.Invoke(this, e);
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
            Task.Factory.StartNew(async () => // Video Feed Factory
            {
                while (true)
                {
                    try
                    {
                        if (token.IsCancellationRequested)
                            break;
                        var received = await BetterTello.videoServer.Receive();
                        VideoDataRecievedEventArgs eventArgs = new(received.bytes);
                        tello.Events.VideoDataRecieved(eventArgs);
                    }
                    catch (Exception e) { Console.WriteLine(e); }
                }
            }, token);
        }

        private void StateFactory(CancellationToken token)
        {
            Task.Factory.StartNew(async () => // State factory
            {
                while (true)
                {
                    try
                    {
                        if (token.IsCancellationRequested)
                            break;
                        var received = await BetterTello.stateServer.Receive();
                        tello.State.ParseState(received.Message);
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
                    }
                    catch (Exception e) { Console.WriteLine(e); }
                    await Task.Delay(100);
                }
            }, token);
        }
        private void CommandFactory(CancellationToken token)
        {
            Task.Factory.StartNew(async () => // Command response Factory
            {
                while (true)
                {
                    try
                    {
                        if (token.IsCancellationRequested)
                            break;
                        string response = (await BetterTello.Client.Receive()).Message;
                        TaskRecieved(new TaskRecievedEventArgs()
                        {
                            Received = response
                        });
                        if (response.Contains("tof"))
                            tello.State.ParseExtTof(response);
                    }
                    catch (Exception e) { Console.WriteLine(e); }
                }
            }, token);
        }
    }

    public class TaskRecievedEventArgs : EventArgs
    {
        public string Received { get; set; }
    }
}