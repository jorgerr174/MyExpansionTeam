using static METCore.Enums.Types;

namespace METCore.DTOs.Contract
{
    public class ContractDto(int? Id, string PlayerName, FranchiseEnum Franchise, int YearSigned, int Length, long Total, long Guaranteed, bool? Active)
    {
        public int Id { get; set; } = Id ?? 0;

        public string Player { get; set; } = PlayerName;

        public FranchiseEnum Franchise { get; set; } = Franchise;

        public int YearSigned { get; set; } = YearSigned;

        public int Length { get; set; } = Length;

        public long Total { get; set; } = Total;

        public long Guaranteed { get; set; } = Guaranteed;

        public bool Active { get; set; } = Active ?? true;
    }
}
