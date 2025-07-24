using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        //        [NotMapped]
        //        public string ProtectableHtml => $"<option class='player' value='{Id}'>{Name} | {Position} | {(Age != null ? Age + 'y' : "")} | ${ActiveContract?.APY}</option>";
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
