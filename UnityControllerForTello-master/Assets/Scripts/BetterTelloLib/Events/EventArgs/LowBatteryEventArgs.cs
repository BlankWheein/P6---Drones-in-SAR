namespace BetterTelloLib.Commander.Events.EventArgs
{
    public class LowBatteryEventArgs : System.EventArgs
    {
        public int Percent;
        public LowBatteryEventArgs(int percent)
        {
            Percent = percent;
        }
    }
}