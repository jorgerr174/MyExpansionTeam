using METCore.Models;
using METCore.Models.Players;

namespace METCore.Interfaces
{
    public interface IFranchiseRepository : IBaseRepository<Franchise>
    {
        #region Get
        IList<Player> GetDefaultProtected(int numPerFranchise);
        #endregion Get
    }
}
