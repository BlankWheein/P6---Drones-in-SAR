namespace BetterTelloLib.Commander.Events.EventArgs
{
    public class ExtTofEventArgs : System.EventArgs
    {
        public int tof;
        public ExtTofEventArgs(int tof)
        {
            this.tof = tof;
        }
    }
}