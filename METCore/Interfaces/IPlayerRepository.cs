using METCore.Models.Players;
using static METCore.Enums.Types;

namespace METCore.Interfaces
{
    public interface IPlayerRepository : IBaseRepository<Player>
    {
        #region Create
        Task<Player?> CreateBasic(string name, PositionEnum defaultPosition = PositionEnum.ATH);
        #endregion Create


        #region Get
        /// <summary>Busca un Player por su nombre.</summary>
        /// <param name="name">Nombre como parámetro de búsqueda.</param>
        /// <returns>User con Nombre igual a name, o Null en su defecto.</returns>
        Task<IList<Player>> GetByName(string name);


        Task<IList<Player>> GetByNamePosition(string name, PositionEnum position);


        Task<Player?> GetProspect(Player Player);
        Task<bool?> PlayerExists(Player Player);
        Task<bool?> PlayerExists(string Name, int Height, int Weight, DateOnly BirthDate, PositionEnum Position, string College);


        Task<int> CountByName(string name);

        Task<IList<Player>> GetDraftProspects(int year);
        #endregion Get
    }
}
