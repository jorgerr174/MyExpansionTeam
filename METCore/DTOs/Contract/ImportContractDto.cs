using METCore.Interfaces.Importing;
using static METCore.Enums.Types;

namespace METCore.DTOs.Contract
{
    public class ImportContractDto(string Player, PositionEnum Position, FranchiseEnum Franchise, int YearSigned, int Length, long Total, long Guaranteed)
        : ContractDto(Id: null, Player, Franchise, YearSigned, Length, Total, Guaranteed, Active: true), IImportableDto
    {
        public PositionEnum Position { get; set; } = Position;
    }
}
