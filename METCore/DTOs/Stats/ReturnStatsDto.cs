namespace METCore.DTOs.Stats
{
    #region KRStatsDto
    public class KRStatsDto(int Ret, int Yds, int TD, int Plus20, int Plus40, int Lng, int FC, int Fmb)
        : ReturnStatsDto(Ret, Yds, TD, Plus20, Plus40, Lng, FC, Fmb)
    { }
    #endregion KRStatsDto


    #region PRStatsDto
    public class PRStatsDto(int Ret, int Yds, int TD, int Plus20, int Plus40, int Lng, int FC, int Fmb)
        : ReturnStatsDto(Ret, Yds, TD, Plus20, Plus40, Lng, FC, Fmb)
    { }
    #endregion PRStatsDto


    #region ReturnStatsDto
    public abstract class ReturnStatsDto(int Ret, int Yds, int TD, int Plus20, int Plus40, int Lng, int FC, int Fmb)
    {
        public int Ret { get; protected set; } = Ret;
        public int Yds { get; protected set; } = Yds;
        public int TD { get; protected set; } = TD;

        public int Plus20 { get; protected set; } = Plus20;
        public int Plus40 { get; protected set; } = Plus40;

        public int Lng { get; protected set; } = Lng;
        public int FC { get; protected set; } = FC;
        public int Fmb { get; protected set; } = Fmb;

        public double Avg { get { return Ret == 0 ? 0 : Yds / Ret; } }
    }
    #endregion ReturnStatsDto
}
