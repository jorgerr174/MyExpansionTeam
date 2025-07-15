using METCore.Interfaces.Importing;

namespace METCore.DTOs.Stats
{
    #region IImportStatsDto
    public interface IImportStatsDto : IImportableDto
    {
    }
    #endregion IImportStatsDto


    #region ImportFGStatsDto
    public class ImportFGStatsDto(string Player,
        int ShMade, int ShAtt, int MidMade, int MidAtt, int LongMade, int LongAtt, int Lng, int Blk)
        : FGStatsDto(ShMade, ShAtt, MidMade, MidAtt, LongMade, LongAtt, Blk, Lng), IImportStatsDto
    {
        public string Player { get; set; } = Player;
    }
    #endregion ImportFGStatsDto


    #region ImportIntStatsDto
    public class ImportIntStatsDto(string Player,
        int INT, int TD, int Yds, int Lng)
        : IntStatsDto(INT, TD, Yds, Lng), IImportStatsDto
    {
        public string Player { get; set; } = Player;
    }
    #endregion ImportIntStatsDto


    #region ImportKOStatsDto
    public class ImportKOStatsDto(string Player,
        int Kick, int Yds, int TB, int OOB, int Ret, int RetYds, int TD,
        int OnSide, int OnSideRec)
        : KOStatsDto(Kick, Yds, TB, OOB, Ret, RetYds, TD,
            OnSide, OnSideRec), IImportStatsDto
    {
        public string Player { get; set; } = Player;
    }
    #endregion ImportKOStatsDto


    #region ImportPuntStatsDto
    public class ImportPuntStatsDto(string Player,
        int Kick, int Yds, int TB, int OOB, int Ret, int RetYds, int TD,
        int Inside20, int Down, int FC, int Blk, int Lng)
        : PuntStatsDto(Kick, Yds, TB, OOB, Ret, RetYds, TD,
            Inside20, Down, FC, Blk, Lng), IImportStatsDto
    {
        public string Player { get; set; } = Player;
    }
    #endregion ImportPuntStatsDto


    #region ImportKRStatsDto
    public class ImportKRStatsDto(string Player,
        int Ret, int Yds, int TD, int Plus20, int Plus40, int Lng, int FC, int Fmb)
        : KRStatsDto(Ret, Yds, TD, Plus20, Plus40, Lng, FC, Fmb), IImportStatsDto
    {
        public string Player { get; set; } = Player;
    }
    #endregion ImportKRStatsDto


    #region ImportPRStatsDto
    public class ImportPRStatsDto(string Player,
        int Ret, int Yds, int TD, int Plus20, int Plus40, int Lng, int FC, int Fmb)
        : PRStatsDto(Ret, Yds, TD, Plus20, Plus40, Lng, FC, Fmb), IImportStatsDto
    {
        public string Player { get; set; } = Player;
    }
    #endregion ImportPRStatsDto


    #region ImportPassStatsDto
    public class ImportPassStatsDto(string Player,
        int Yds, int Att, int TD, int Plus20, int Plus40, int Reach1st, int Lng,
        int Cmp, int INT, double PR, int Sck, int SckYds)
        : PassStatsDto(Yds, Att, TD, Plus20, Plus40, Reach1st, Lng,
            Cmp, INT, PR, Sck, SckYds), IImportStatsDto
    {
        public string Player { get; set; } = Player;
    }
    #endregion ImportPassStatsDto


    #region ImportRecStatsDto
    public class ImportRecStatsDto(string Player,
        int Yds, int Att, int TD, int Plus20, int Plus40, int Reach1st, int Lng,
        int Rec, int Fmb, int YAC)
        : RecStatsDto(Yds, Att, TD, Plus20, Plus40, Reach1st, Lng,
            Rec, Fmb, YAC), IImportStatsDto
    {
        public string Player { get; set; } = Player;
    }
    #endregion ImportRecStatsDto


    #region ImportRushStatsDto
    public class ImportRushStatsDto(string Player,
        int Yds, int Att, int TD, int Plus20, int Plus40, int Reach1st, int Lng,
        int Fmb)
        : RushStatsDto(Yds, Att, TD, Plus20, Plus40, Reach1st, Lng,
            Fmb), IImportStatsDto
    {
        public string Player { get; set; } = Player;
    }
    #endregion ImportRushStatsDto


    #region ImportTackleStatsDto
    public class ImportTackleStatsDto(string Player,
        int Comb, int Solo, int Asst, double Sck)
        : TackleStatsDto(Comb, Solo, Asst, Sck), IImportStatsDto
    {
        public string Player { get; set; } = Player;
    }
    #endregion ImportTackleStatsDto
}
