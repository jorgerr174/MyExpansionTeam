namespace METCore.DTOs.Stats
{
    #region KOStatsDto
    public class KOStatsDto(int Kick, int Yds, int TB, int OOB, int Ret, int RetYds, int TD,
        int OnSide, int OnSideRec)
        : KickStatsDto(Kick, Yds, TB, OOB, Ret, RetYds, TD)
    {
        public int OnSide { get; private set; } = OnSide;
        public int OnSideRec { get; private set; } = OnSideRec;
    }
    #endregion KOStatsDto


    #region PuntStatsDto
    public class PuntStatsDto(int Kick, int Yds, int TB, int OOB, int Ret, int RetYds, int TD,
        int Inside20, int Down, int FC, int Blk, int Lng)
        : KickStatsDto(Kick, Yds, TB, OOB, Ret, RetYds, TD)
    {
        public int Inside20 { get; private set; } = Inside20;
        public int Down { get; private set; } = Down;
        public int FC { get; private set; } = FC;
        public int Blk { get; private set; } = Blk;
        public int Lng { get; private set; } = Lng;

        #region NotMapped
        public double Avg { get { return Kick == 0 ? 0 : Yds / Kick; } }
        public double NetAvg { get { return Avg - RetAvg; } }

        #endregion NotMapped
    }
    #endregion PuntStatsDto


    #region KickStatsDto
    public abstract class KickStatsDto(int Kick, int Yds, int TB, int OOB, int Ret, int RetYds, int TD)
    {
        public int Kick { get; protected set; } = Kick;
        public int Yds { get; protected set; } = Yds;

        public int TB { get; protected set; } = TB;
        public int OOB { get; protected set; } = OOB;

        public int Ret { get; protected set; } = Ret;
        public int RetYds { get; protected set; } = RetYds;
        public int TD { get; protected set; } = TD;

        #region NotMapped
        public double TBPct { get { return Kick == 0 ? 0 : TB / Kick; } }
        public double RetAvg { get { return Ret == 0 ? 0 : RetYds / Ret; } }
        #endregion NotMapped
    }
    #endregion KickStatsDto
}