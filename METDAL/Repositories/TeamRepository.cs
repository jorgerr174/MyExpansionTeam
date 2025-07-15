using METCore.Interfaces;
using METCore.Models;
using METCore.Models.Teams;
using METDAL.Data;

namespace METDAL.Repositories
{
    public class TeamRepository(ApplicationDbContext context) : BaseRepository<Team>(context), ITeamRepository
    {
        #region Get
        /// <summary> Busca listas de Team. </summary>
        /// <param name="mine">Parámetro de búsqueda de equipos dl User logeado.</param>
        /// <param name="user">User logeado de búsqueda del T.</param>
        /// IEnumerable<Team> (Con los Teams encontrados).
        public Task<IEnumerable<Team>> ListTeams(bool mine, User user)
        {
            return Task.FromResult(_context.Teams.AsAsyncEnumerable().ToBlockingEnumerable().Where(t => !mine || t.User == user));
        }
        #endregion
    }
}
