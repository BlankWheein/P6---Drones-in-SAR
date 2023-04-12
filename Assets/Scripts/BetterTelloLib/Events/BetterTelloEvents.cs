using BetterTelloLib.Commander.Events.EventArgs;
using BetterTelloLib.Commander.Factories;
using System;

namespace BetterTelloLib.Commander.Events
{
    public class BetterTelloEvents
    {
        public event EventHandler<VideoDataRecievedEventArgs>? OnVideoDataRecieved;
        internal virtual void VideoDataRecieved(VideoDataRecievedEventArgs e)
        {
            OnVideoDataRecieved?.Invoke(this, e);
        }

        public event EventHandler<TaskRecievedEventArgs>? OnOkRecieved;
        internal virtual void OkRecieved(TaskRecievedEventArgs e)
        {
            OnOkRecieved?.Invoke(this, e);
        }

        public event EventHandler<System.EventArgs>? OnTakeOff;
        internal virtual void TakeOff()
        {
            OnTakeOff?.Invoke(this, System.EventArgs.Empty);
        }

        public event EventHandler<System.EventArgs>? OnLand;
        internal virtual void Land()
        {
            OnLand?.Invoke(this, System.EventArgs.Empty);
        }

        public event EventHandler<System.EventArgs>? OnConnect;
        internal virtual void Connect()
        {
            OnConnect?.Invoke(this, System.EventArgs.Empty);
        }

        public event EventHandler<HighTempEventArgs>? OnHighTemp;
        internal virtual void HighTemp(HighTempEventArgs e)
        {
            OnHighTemp?.Invoke(this, e);
        }

        public event EventHandler<LowBatteryEventArgs>? OnLowBattery;
        internal virtual void LowBattery(LowBatteryEventArgs e)
        {
            OnLowBattery?.Invoke(this, e);
        }

        public event EventHandler<MissionPadDetectedEventArgs>? OnMissionPadDetected;
        internal virtual void MissionPadDetected(MissionPadDetectedEventArgs e)
        {
            OnMissionPadDetected?.Invoke(this, e);
        }

        public event EventHandler<System.EventArgs>? OnDisconnect;
        internal virtual void Disconnect()
        {
            OnDisconnect?.Invoke(this, System.EventArgs.Empty);
        }

        public event EventHandler<StateEventArgs>? OnStateRecieved;
        internal virtual void StateRecieved(StateEventArgs e)
        {
            OnStateRecieved?.Invoke(this, e);
        }

        public event EventHandler<ExtTofEventArgs>? OnExtTof;
        internal virtual void ExtTofRecieved(ExtTofEventArgs e) {
            OnExtTof?.Invoke(this, e);
        }
    }
}