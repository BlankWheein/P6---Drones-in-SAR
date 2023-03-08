namespace BetterTelloLib.Commander.Events.EventArgs
{
    public class HighTempEventArgs : System.EventArgs
    {
        public float Temp;
        public HighTempEventArgs(float temp)
        {
            Temp = temp;
        }
    }
}