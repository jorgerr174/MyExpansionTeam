using METCore.Interfaces;
using METCore.Models;
using METDAL.Data;

namespace METDAL.Repositories
{
    public class SeasonStatsRepository(ApplicationDbContext context) : BaseRepository<SeasonStats>(context), ISeasonStatsRepository
    {
    }
}
