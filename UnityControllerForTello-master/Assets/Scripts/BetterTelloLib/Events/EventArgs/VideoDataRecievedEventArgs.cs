namespace BetterTelloLib.Commander.Events.EventArgs
{
    public class VideoDataRecievedEventArgs : System.EventArgs
    {
        public byte[] Data;
        public VideoDataRecievedEventArgs(byte[] data)
        {
            Data = data;
        }
    }
    
}