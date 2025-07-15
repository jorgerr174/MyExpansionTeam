using METCore.Interfaces;
using METCore.Models;
using METCore.Models.Players;
using METDAL.Data;


namespace METDAL.Repositories
{
    public class ContractRepository(ApplicationDbContext context) : BaseRepository<Contract>(context), IContractRepository
    {
        #region Get
        public bool? ContractExists(Player player, int FranchiseId, int YearSigned, int Length, long Total)
        {
            int count = player.Contracts.Count(c => c.FranchiseId == FranchiseId && c.YearSigned == YearSigned && c.Length == Length && c.Total == Total);
            return count > 1 ? null : count == 1;
        }
        #endregion Get
    }
}