using METCore.Models;
using METCore.Models.Players;

namespace METCore.Interfaces
{
    public interface IContractRepository : IBaseRepository<Contract>
    {
        #region Get
        bool? ContractExists(Player player, int FranchiseId, int YearSigned, int Length, long Total);
        #endregion
    }
}
