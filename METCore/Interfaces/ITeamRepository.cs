using METCore.Models;
using METCore.Models.Teams;

namespace METCore.Interfaces
{
    public interface ITeamRepository : IBaseRepository<Team>
    {
        #region Get
        /// <summary> Busca listas de Team. </summary>
        /// <param name="mine">Parámetro de búsqueda de equipos dl User logeado.</param>
        /// <param name="user">User logeado de búsqueda del T.</param>
        /// IEnumerable<Team> (Con los Teams encontrados).
        Task<IEnumerable<Team>> ListTeams(bool mine, User user);
        #endregion
    }
}
