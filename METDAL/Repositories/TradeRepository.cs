using METCore.Interfaces;
using METCore.Models.Teams;
using METDAL.Data;

namespace METDAL.Repositories
{
    public class TradeRepository(ApplicationDbContext context) : BaseRepository<Trade>(context), ITradeRepository
    {
    }
}
