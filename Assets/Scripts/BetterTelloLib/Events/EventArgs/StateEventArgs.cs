namespace BetterTelloLib.Commander.Events.EventArgs
{
    public class StateEventArgs : System.EventArgs
    {
        public TelloState State;
        public StateEventArgs(TelloState state)
        {
            State = state;
        }
    }
}