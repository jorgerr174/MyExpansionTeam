using System.ComponentModel.DataAnnotations;

namespace METCore.Models.Teams
{
    public class Trade : BaseClass
    {
        #region Attributes
        [Required]
        [Key]
        public override int Id { get; set; }
        public int FranchiseId { get; set; }
        public IList<int> PlayersTaken { get; set; }
        public IList<int> PlayersSent { get; set; }
        public int TotalCapTaken { get; set; }
        public int TotalCapSent { get; set; }
        public IList<int> PicksTaken { get; set; }
        public IList<int> PicksSent { get; set; }
        #region Not Mapped
        #endregion Not Mapped

        #endregion Attributes


        #region Constructors
        public Trade()
        {
            this.FranchiseId = 0;
            this.PlayersTaken = [];
            this.PlayersSent = [];
            this.TotalCapTaken = 0;
            this.TotalCapSent = 0;
            this.PicksTaken = [];
            this.PicksSent = [];
        }

        public Trade(int FranchiseId, IList<int> PlayersTaken, IList<int> PlayersSent, int TotalCapTaken, int TotalCapSent, IList<int> PicksTaken, IList<int> PicksSent)
        {
            this.FranchiseId = FranchiseId;
            this.PlayersTaken = PlayersTaken;
            this.PlayersSent = PlayersSent;
            this.TotalCapTaken = TotalCapTaken;
            this.TotalCapSent = TotalCapSent;
            this.PicksTaken = PicksTaken;
            this.PicksSent = PicksSent;
        }
        #endregion Constructors
    }
}
