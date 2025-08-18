using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Intrinsics.X86;
using METCore.Models.Stats;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static METCore.Enums.Types;

namespace METCore.Models.Players
{
    public class Player : BaseClass
    {
        #region Attributes
        [Required]
        [Key]
        public override int Id { get; set; }

        [MaxLength(70)]
        public string Name { get; set; }

        public int Height { get; set; }

        public int Weight { get; set; }

        public DateOnly? BirthDate { get; set; }

        public PositionEnum Position { get; set; }

        public PositionEnum? Position2 { get; set; }

        public PositionEnum? Position3 { get; set; }

        public virtual IList<Contract> Contracts { get; set; }

        public virtual IList<SeasonStats> Stats { get; set; }

        public virtual Prospect? Prospect { get; set; }

        public string College { get; set; }

        public int Madden { get; set; }

        public int Jersey { get; set; }

        public int DraftYear { get; set; }

        public bool Retired { get; set; }

        [NotMapped]
        public int FranchiseId => ActiveContract?.FranchiseId ?? 0;

        [NotMapped]
        public Contract? ActiveContract => Contracts.FirstOrDefault(c => c.Active);

        [NotMapped]
        public decimal APY => ActiveContract is null || ActiveContract.APY is 0 ? 0 : Math.Round((decimal)(ActiveContract.APY / 1000000), 2);

        [NotMapped]
        public int? Age => BirthDate is null ? null : DateTime.Now.Year - BirthDate.Value.Year - (DateTime.Now.DayOfYear < BirthDate.Value.DayOfYear ? 1 : 0);

        [NotMapped]
        public bool? IsRookie => Prospect is not null && Prospect.Year == DateTime.Now.Year;

        [NotMapped]
        public string[] ImportProspectAttrs =>
            Prospect is null ? [] : [Name, Position.ToString(), College, Height.ToString(), Weight.ToString(), .. Prospect.ImportAttributes];

        public string StatsHtml()
        {
            SeasonStats? last = this.Stats.OrderByDescending(s => s.Season).FirstOrDefault();
            if (last is null) return "<br><small class='text-muted'>No relevant stats</small>";

            return "<small class='text-info'>" +
                this.Position switch
                {
                    PositionEnum.QB => this.PassStatsHtml(last.PassStats),
                    PositionEnum.ATH or PositionEnum.RB or PositionEnum.FB => this.RushStatsHtml(last.RushStats, last.RecStats),
                    PositionEnum.WR or PositionEnum.TE => this.RecStatsHtml(last.RecStats),
                    PositionEnum.NT or PositionEnum.DT or PositionEnum.ED => this.TackleStatsHtml(last.TackleStats),
                    PositionEnum.OLB or PositionEnum.MLB or PositionEnum.DB or PositionEnum.CB or
                    PositionEnum.S or PositionEnum.SS or PositionEnum.FS => this.DStatsHtml(last.IntStats, last.TackleStats),
                    PositionEnum.P => this.PuntStatsHtml(last.PuntStats),
                    PositionEnum.K => this.KOStatsHtml(last.KOStats),
                    PositionEnum.PR => this.ReturnStatsHtml(last.PRStats),
                    PositionEnum.KR => this.ReturnStatsHtml(last.KRStats),
                    PositionEnum.OT or PositionEnum.G or PositionEnum.C or PositionEnum.LS => string.Empty,
                    _ => string.Empty
                } + "</small>";
        }

        private string PassStatsHtml(PassStats? stats)
        {
            if (stats is null) return string.Empty;

            string display = string.Empty;

            if (stats.Cmp != 0 && stats.Att != 0)
                display += string.Format("Cmp/Att: {0}/{1}, ", stats.Cmp, stats.Att);

            if(stats.Yds != 0)
                display += string.Format("{0} yds, ", stats.Yds);

            if (stats.TD != 0)
                display += string.Format("{0} TD, ", stats.TD);

            if (stats.INT != 0)
                display += string.Format("{0} INT, ", stats.INT);

            if (stats.PR != 0)
                display += string.Format("{0} PR, ", Math.Round(stats.PR, 1));

            return display;
        }

        private string RecStatsHtml(RecStats? stats)
        {
            if (stats is null) return string.Empty;

            string display = string.Empty;

            if (stats.Rec != 0 && stats.Att != 0)
                display += string.Format("Rec/Att: {0}/{1}, ", stats.Rec, stats.Att);

            if (stats.Yds != 0)
                display += string.Format("{0} yds, ", stats.Yds);

            if (stats.TD != 0)
                display += string.Format("{0} TD, ", stats.TD);

            if (stats.Fmb != 0)
                display += string.Format("{0} Fmb, ", stats.Fmb);

            return display;
        }

        private string RushStatsHtml(RushStats? stats, RecStats? rec)
        {
            if (stats is null) return string.Empty;

            string display = string.Empty;

            if (stats.Yds != 0 && stats.Att != 0)
                display += string.Format("Yds/Att: {0}, ", Math.Round((decimal)stats.Yds/stats.Att, 2));

            if (stats.Yds != 0)
                display += string.Format("{0} yds, ", stats.Yds);

            if (stats.TD != 0)
                display += string.Format("{0} TD, ", stats.TD);

            if (stats.Fmb != 0)
                display += string.Format("{0} Fmb, ", stats.Fmb);

            return display + RecStatsHtml(rec);
        }

        private string TackleStatsHtml(TackleStats? stats)
        {
            if (stats is null) return string.Empty;

            string display = string.Empty;

            if (stats.Solo != 0 && stats.Comb != 0)
                display += string.Format("Solo/Comb: {0}/{1}, ", stats.Solo, stats.Total);

            if (stats.Sck != 0)
                display += string.Format("{0} sacks, ", stats.Sck);

            return display;
        }

        private string DStatsHtml(IntStats? stats, TackleStats? tackle)
        {
            if (stats is null) return string.Empty;

            string display = string.Empty;

            if (stats.INT != 0)
                display += string.Format("{0} INT, ", stats.INT);

            if (stats.TD != 0)
                display += string.Format("{0} TD, ", stats.TD);

            if (stats.Yds != 0)
                display += string.Format("{0} Yds, ", stats.Yds);

            return display + TackleStatsHtml(tackle);
        }

        private string PuntStatsHtml(PuntStats? stats)
        {
            if (stats is null) return string.Empty;

            string display = string.Empty;

            if (stats.Yds != 0 && stats.Kick != 0)
                display += string.Format("Net Yds/Kick: {0}, ", Math.Round((decimal)(stats.Yds-stats.RetYds) / stats.Kick, 2));

            if (stats.Inside20 != 0)
                display += string.Format("{0} In20, ", stats.Inside20);

            if (stats.FC != 0)
                display += string.Format("{0} FC, ", stats.FC);

            return display;
        }

        private string KOStatsHtml(KOStats? stats)
        {
            if (stats is null) return string.Empty;

            string display = string.Empty;

            if (stats.Yds != 0 && stats.Kick != 0)
                display += string.Format("Net Yds/Kick: {0}, ", Math.Round((decimal)(stats.Yds-stats.RetYds) / stats.Kick, 2));

            if (stats.OOB != 0)
                display += string.Format("{0} OoB, ", stats.OOB);

            if (stats.OnSideRec != 0 && stats.OnSide != 0)
                display += string.Format("Onside%: {0}, ", Math.Round((decimal)stats.OnSideRec / stats.OnSide, 2));

            return display;
        }

        private string ReturnStatsHtml(ReturnStats? stats)
        {
            if (stats is null) return string.Empty;

            string display = string.Empty;

            if (stats.Yds != 0 && stats.Ret != 0)
                display += string.Format("Yds/Ret: {0}, ", Math.Round((decimal)stats.Yds / stats.Ret, 2));

            if (stats.Lng != 0)
                display += string.Format("Lng: {0} yds, ", stats.Ret);

            if (stats.Fmb != 0)
                display += string.Format("Fmb: {0}, ", stats.Fmb);

            if (stats.TD != 0)
                display += string.Format("TD: {0}, ", stats.TD);

            return display;
        }
        #endregion Attributes


        #region Constructors
        public Player()
        {
            Name = "";
            Contracts = [];
            Stats = [];
            College = "";
        }
        #endregion
    }
}
