using System.Globalization;

namespace BetterTelloLib.Commander
{
    public enum FlyingState
    {
        Grounded,
        Flying,
    }
    public class TelloState
    {
        internal BetterTello bt;

        public TelloState(BetterTello bt)
        {
            this.bt = bt;
        }
        public static int ProximityLimit = 1000;
        public FlyingState FlyingState = FlyingState.Grounded;
        public string RawState = "";
        public int MId = 0;
        public int X = 0;
        public int Y = 0;
        public int Z = 0;
        public string MPRY = "";
        public int Pitch = 0;
        public int Roll = 0;
        public int Yaw = 0;
        public int Vgx = 0;
        public int Vgy = 0;
        public int Vgz = 0;
        public int Templ = 0;
        public int Temph = 0;
        public int Tof = 0;
        public int H = 0;
        public int Bat = 0;
        public float Baro = 0;
        public int Time = 0;
        public float Agx = 0;
        public float Agy = 0;
        public float Agz = 0;
        public int ExtTof = -1;
        public bool ObstacleTooCloseInFront;

        private float _prevAgx = 0f;
        private float _prevAgy = 0f;
        private float _prevAgz = 0f;

        public void ParseExtTof(string state)
        {
            if (state.Contains("tof "))
            {
                var x = state.Split("tof ")?.LastOrDefault();

                if (x != null && int.TryParse(x, out int _tof))
                {
                    ExtTof = (int)_tof;
                    bt.Events.ExtTofRecieved(new Events.EventArgs.ExtTofEventArgs(ExtTof));
                    if (ExtTof < TelloState.ProximityLimit)
                    {
                        if (!ObstacleTooCloseInFront)
                            bt.Commands.Stop();
                        ObstacleTooCloseInFront = true;
                    }
                    else
                        ObstacleTooCloseInFront = false;
                }
            }
                
                
        }

        public void ParseState(string state)
        {
            RawState = state;
            ParseState("mid", ref MId);
            ParseState("x", ref X);
            ParseState("y", ref Y);
            ParseState("z", ref Z);
            ParseState("mpry", ref MPRY);
            ParseState("pitch", ref Pitch);
            ParseState("roll", ref Roll);
            ParseState("yaw", ref Yaw);
            ParseState("vgx", ref Vgx);
            ParseState("vgy", ref Vgy);
            ParseState("vgz", ref Vgz);
            ParseState("templ", ref Templ);
            ParseState("temph", ref Temph);
            ParseState("tof", ref Tof);
            ParseState("h", ref H);
            ParseState("bat", ref Bat);
            ParseState("baro", ref Baro);
            ParseState("time", ref Time);
            ParseState("agx", ref Agx);
            ParseState("agy", ref Agy);
            ParseState("agz", ref Agz);
            FlyingState = CalculateFlyingState();
        }
        private FlyingState CalculateFlyingState()
        {
            if (Agx + Agy + Agz > 0.05f)
                return FlyingState.Flying;
            return FlyingState.Grounded;
        }
        private void ParseState(string id, ref int Prop)
        {
            Prop = int.Parse(GetStateStringValue(id));
        }
        private void ParseState(string id, ref float Prop)
        {
            var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";
            if (id == "agx")
                _prevAgx = Agx;
            else if (id == "agy")
                _prevAgy = Agy;
            else if (id == "agz")
                _prevAgz = Agz;
            Prop = float.Parse(GetStateStringValue(id), culture);
        }
        private void ParseState(string id, ref string Prop)
        {
            Prop = GetStateStringValue(id);
        }
        private string GetStateStringValue(string id)
        {
            return RawState.Split(id + ":")[1].Split(";")[0];
        }

    }
}