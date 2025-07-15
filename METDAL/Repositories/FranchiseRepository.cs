using METCore.Interfaces;
using METCore.Models;
using METCore.Models.Players;
using METDAL.Data;

namespace METDAL.Repositories
{
    public class FranchiseRepository(ApplicationDbContext context) : BaseRepository<Franchise>(context), IFranchiseRepository
    {
        #region Get
        public IList<Player> GetDefaultProtected(int numPerFranchise)
        {
            IList<Player> list = [];
            try
            {
                if (numPerFranchise > 0)
                {
                    list = [.. _context.Franchises.Select(f => f.Protected1)];
                    if (numPerFranchise > 1)
                    {
                        list = [.. list.Concat(_context.Franchises.Select(f => f.Protected2))];
                        if (numPerFranchise > 2) list = [.. list.Concat(_context.Franchises.Select(f => f.Protected3))];
                    }
                }

            }
            catch { list = []; };

            return list;
        }
        #endregion Get
    }
}
