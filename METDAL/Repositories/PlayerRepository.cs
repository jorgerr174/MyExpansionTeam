using METCore.DTOs.Player;
using METCore.Interfaces;
using METCore.Models.Players;
using METDAL.Data;
using Microsoft.EntityFrameworkCore;
using static METCore.Enums.Types;

namespace METDAL.Repositories
{
    public class PlayerRepository(ApplicationDbContext context) : BaseRepository<Player>(context), IPlayerRepository
    {
        #region Create
        public async Task<Player?> CreateBasic(string name, PositionEnum defaultPosition = PositionEnum.ATH)
        {
            Player newPlayer = new() { Name = name, Height = 72, Weight = 220, Position = defaultPosition, Retired = false };
            return await CreateT(newPlayer) < 1 ? null : newPlayer;
        }
        #endregion Create


        #region Get
        /// <summary> Busca un Player por su nombre. </summary>
        /// <param name="name">Nombre como parámetro de búsqueda.</param>
        /// <returns>User con Nombre igual a name, o Null en su defecto.</returns>
        public async Task<IList<Player>> GetByName(string name)
        {
            return await _context.Players.Where(p => p.Name == name).ToListAsync();
        }


        public async Task<IList<Player>> GetByNamePosition(string name, PositionEnum position)
        {
            return await _context.Players
                .Where(p => p.Name == name && (p.Position == position || p.Position == PositionEnum.ATH)).ToListAsync();
        }

        /// <summary>
        /// find out if player already created but without prospect info
        /// </summary>
        /// <param name="Player"></param>
        /// <returns>
        /// null -> multiple possible players
        /// Player -> none
        /// First -> 1 possible player
        /// </returns>
        public async Task<Player?> GetProspect(Player Player)
        {
            IList<Player> playerIds = await _context.Players.Where(p => p.Name == Player.Name && p.Height == Player.Height && p.Weight == Player.Weight
                            && p.Position == Player.Position && p.College == Player.College).ToListAsync();

            return playerIds.Count > 1 ? null
                : playerIds.Count == 0 ? Player
                : playerIds.First();
        }
        public async Task<bool?> PlayerExists(Player Player)
        {
            int count = await _context.Players.CountAsync(p => p.Name == Player.Name && p.Height == Player.Height && p.Weight == Player.Weight
                                                            && p.BirthDate == Player.BirthDate && p.Position == Player.Position && p.College == Player.College);
            return count > 1 ? null : count == 1;
        }
        public async Task<bool?> PlayerExists(string Name, int Height, int Weight, DateOnly BirthDate, PositionEnum Position, string College)
        {
            int count = await _context.Players.CountAsync(p =>
                p.Name == Name && p.Height == Height && p.Weight == Weight && p.BirthDate == BirthDate && p.Position == Position && p.College == College);
            return count > 1 ? null : count == 1;
        }


        public async Task<int> CountByName(string name)
        {
            int count = -1;
            try { count = await _context.Players.CountAsync(p => p.Name == name); }
            catch { };
            return count;
        }

        /// <summary> Obtener los TeamDtos con los valores de los Teams del User logeado.</summary>
        /// <returns>Opciones:
        /// Username (no se encontró ningún User para username).
        /// IEnumerable<TeamDto>? (Con los valores de los Teams encontrados).
        /// </returns>
        public async Task<IList<Player>> GetDraftProspects(int year)
        {
            return await _context.Players.Where(p => p.Prospect is Prospect && p.Prospect.Year == year).ToListAsync();
        }
        #endregion Get
    }
}
