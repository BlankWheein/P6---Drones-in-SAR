namespace BetterTelloLib.Commander.Events.EventArgs
{
    public class MissionPadDetectedEventArgs : System.EventArgs
    {
        public int mid;
        public MissionPadDetectedEventArgs(int mid)
        {
            this.mid = mid;
        }
    }
    
}