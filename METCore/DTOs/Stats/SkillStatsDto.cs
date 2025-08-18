namespace METCore.DTOs.Stats
{
    #region PassStatsDto
    public class PassStatsDto(int Yds, int Att, int TD, int Plus20, int Plus40, int Reach1st, int Lng,
        int Cmp, int INT, double PR, int Sck, int SckYds)
        : SkillStatsDto(Yds, Att, TD, Plus20, Plus40, Reach1st, Lng)
    {
        public int Cmp { get; private set; } = Cmp;
        public int INT { get; private set; } = INT;
        public double PR { get; private set; } = PR;

        public int Sck { get; private set; } = Sck;
        public int SckYds { get; private set; } = SckYds;

        #region NotMapped
        public double CmpPct { get { return Att == 0 ? 0 : Cmp / Att; } }
        public double Y_S { get { return Sck == 0 ? 0 : SckYds / Sck; } }
        #endregion NotMapped
    }
    #endregion PassStatsDto


    #region RecStatsDto
    public class RecStatsDto(int Yds, int Att, int TD, int Plus20, int Plus40, int Reach1st, int Lng,
        int Rec, int Fmb, int YAC)
        : SkillStatsDto(Yds, Att, TD, Plus20, Plus40, Reach1st, Lng)
    {
        public int Rec { get; private set; } = Rec;
        public int Fmb { get; private set; } = Fmb;
        public int YAC { get; private set; } = YAC;

        #region NotMapped
        public double R_A { get { return Att == 0 ? 0 : Rec / Att; } }
        #endregion NotMapped
    }
    #endregion RecStatsDto


    #region RushStatsDto
    public class RushStatsDto(int Yds, int Att, int TD, int Plus20, int Plus40, int Reach1st, int Lng,
        int Fmb)
        : SkillStatsDto(Yds, Att, TD, Plus20, Plus40, Reach1st, Lng)
    {
        public int Fmb { get; private set; } = Fmb;
    }
    #endregion RushStatsDto


    #region SkillStatsDto
    public abstract class SkillStatsDto(int Yds, int Att, int TD, int Plus20, int Plus40, int Reach1st, int Lng)
    {
        public int Yds { get; protected set; } = Yds;
        public int Att { get; protected set; } = Att;
        public int TD { get; protected set; } = TD;
        public int Plus20 { get; protected set; } = Plus20;
        public int Plus40 { get; protected set; } = Plus40;
        public int Reach1st { get; protected set; } = Reach1st;
        public int Lng { get; protected set; } = Lng;

        #region NotMapped
        public double Y_A { get { return Att == 0 ? 0 : Yds / Att; } }
        public double Reach1stPct { get { return Att == 0 ? 0 : Reach1st / Att; } }
        #endregion NotMapped
    }
    #endregion SkillStatsDto
}