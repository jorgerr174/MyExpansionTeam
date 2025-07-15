namespace METCore.DTOs.Stats
{
    public class FGStatsDto(int ShMade, int ShAtt, int MidMade, int MidAtt, int LongMade, int LongAtt, int Lng, int Blk)
    {
        public int ShMade { get; private set; } = ShMade;
        public int ShAtt { get; private set; } = ShAtt;

        public int MidMade { get; private set; } = MidMade;
        public int MidAtt { get; private set; } = MidAtt;

        public int LongMade { get; private set; } = LongMade;
        public int LongAtt { get; private set; } = LongAtt;

        public int Lng { get; private set; } = Lng;
        public int Blk { get; private set; } = Blk;

        #region NotMapped
        public double Pct { get { return Att == 0 ? 0 : Made / Att; } }
        public int Made { get { return ShMade + MidMade + LongMade; } }
        public int Att { get { return ShAtt + MidAtt + LongAtt; } }
        #endregion NotMapped
    }
}
