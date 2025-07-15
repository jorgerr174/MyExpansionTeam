using System.Diagnostics;
using System.Security.Cryptography;
using AutoMapper;
using METCore.DTOs.Team;
using METCore.Interfaces;
using METCore.Models;
using METCore.Models.Players;
using METCore.Models.Teams;
using Microsoft.Extensions.Configuration;
using static METCore.Enums.Types;


namespace METCore.Services
{
    public class TeamService(IConfiguration configuration, ITeamRepository teamRepository, IPlayerRepository playerRepository,
        IUserRepository userRepository, IFranchiseRepository franchiseRepository, IMapper mapper)
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ITeamRepository _teamRepository = teamRepository;
        private readonly IPlayerRepository _playerRepository = playerRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IFranchiseRepository _franchiseRepository = franchiseRepository;
        private readonly IMapper _mapper = mapper;


        #region Create
        /// <summary>
        /// Crear un nuevo equipo asociado al usuario idetificado por el parámetro username.
        /// </summary>
        /// <param name="dto">Datos del equipo a crear.</param>
        /// <param name="username">Username del creador del equipo.</param>
        /// <returns>Opciones:
        /// Username (No se encontró ningún User con ese username).
        /// Abb (Abb vacío).
        /// Location (Location vacío).
        /// Mascot (Mascot vacío).
        /// Error (No se guardaron los cambios en la BBDD).
        /// Nada (Todo bien).
        /// </returns>
        public async Task<string> ValidateTeamBasicInfo(TeamBasicInfoDto dto, string? username)
        {
            return await _userRepository.GetUserByUsername(username ?? string.Empty) == null ? "Username"
                : String.IsNullOrWhiteSpace(dto.Abb) ? "Abb"
                : String.IsNullOrWhiteSpace(dto.Location) ? "Location"
                : String.IsNullOrWhiteSpace(dto.Mascot) ? "Mascot"
                : string.Empty;
        }

        public async Task<TeamInfoDto?> CreateTeamFromBasicInfo(TeamBasicInfoDto dto, string username)
        {
            Team newTeam = _mapper.Map<Team>(dto);
            newTeam.User = await _userRepository.GetUserByUsername(username);
            newTeam.Date = DateTime.Now;
            newTeam.Complete = false;

            return await _teamRepository.CreateT(newTeam) < 1 ? null : _mapper.Map<TeamInfoDto>(newTeam);
        }
        #endregion Create


        #region Get
        /// <summary>
        /// Obtener un TeamDto con los valores de un Team con id = Id.
        /// </summary>
        /// <param name="id">Valor de búsqeda de un Team.</param>
        /// <returns>Opciones:
        /// Null (no se encontró ningún Team con ese id).
        /// TeamDto (Con los valores del Team encontrado).
        /// </returns>
        public async Task<TeamDto?> GetDtoById(int id)
        {
            Team? team = await _teamRepository.GetTById(id);
            return team == null ? null : _mapper.Map<TeamDto>(team);
        }

        /// <summary>
        /// Obtener un TeamDto con los valores de un Team con id = Id.
        /// </summary>
        /// <param name="id">Valor de búsqeda de un Team.</param>
        /// <returns>Opciones:
        /// Null (no se encontró ningún Team con ese id).
        /// TeamDto (Con los valores del Team encontrado).
        /// </returns>
        public async Task<TeamInfoDto?> GetInfoDtoById(int id)
        {
            Team? team = await _teamRepository.GetTById(id);
            return team == null ? null : _mapper.Map<TeamInfoDto>(team);
        }

        /// <summary>
        /// Obtener un TeamDto con los valores de un Team con id = Id.
        /// </summary>
        /// <param name="id">Valor de búsqeda de un Team.</param>
        /// <returns>Opciones:
        /// Null (no se encontró ningún Team con ese id).
        /// TeamDto (Con los valores del Team encontrado).
        /// </returns>
        public async Task<DraftDto?> GetTeamDraftDtoById(int id)
        {
            Team? team = await _teamRepository.GetTById(id);
            return team == null ? null : _mapper.Map<DraftDto>(team);
        }

        /// <summary>
        /// Obtener un TeamDto con los valores de un Team con id = Id.
        /// </summary>
        /// <param name="id">Valor de búsqeda de un Team.</param>
        /// <returns>Opciones:
        /// Null (no se encontró ningún Team con ese id).
        /// TeamDto (Con los valores del Team encontrado).
        /// </returns>
        public async Task<TeamBasicInfoDto?> GetBasicInfoDtoById(int id)
        {
            Team? team = await _teamRepository.GetTById(id);
            return team == null ? null : _mapper.Map<TeamBasicInfoDto>(team);
        }

        /// <summary> Obtener un TeamDto con los valores de un Team con id = Id.</summary>
        /// <param name="mine">Teams del User logeado.</param>
        /// <param name="username">username del User logeado.</param>
        /// <returns>Opciones:
        /// null (no se encontró ningún User para username).
        /// IEnumerable<TeamDto> (Con los valores de los Teams encontrados).
        /// </returns>
        public async Task<IEnumerable<TeamInfoDto>?> ListTeams(bool mine, string? username)
        {
            User? user = await _userRepository.GetUserByUsername(username);
            if (user == null) return null;

            IEnumerable<Team> list = await _teamRepository.ListTeams(mine, user);
            return _mapper.Map<IEnumerable<TeamInfoDto>>(list);
        }

        private async Task<IList<int>> GetValidProtectedIds(IList<int> Ids)
        {
            return (await _playerRepository.GetManyTByIds(Ids)).Select(p => p.Id).ToList();
        }

        private IList<int> GetDefaultProtectedPlayers(int numPerFranchise)
        {
            return _franchiseRepository.GetDefaultProtected(numPerFranchise).Select(p => p.Id).ToList();
        }

        public async Task<string?> GetUserControlledPicks(UserPicksDto inDto, string username)
        {
            User? user = await _userRepository.GetUserByUsername(username);
            if (user is null) return "Username";

            Team? team = await _teamRepository.GetTById(inDto.TeamId);
            if (team is null) return "TeamId";
            else if (team.User.Id != user.Id) return "User";

            try
            {
                UserPicksDto outDto = new()
                {
                    TeamId = inDto.TeamId,
                    TeamPicks = [101, 201, 301, 401, 501, 601, 701],
                    Rounds = inDto.Rounds
                };

                foreach (int fid in inDto.OthersPicks)
                {
                    // con los ids, pilla el franchiseEnum, con FranchiseEnum pilla el valor de DraftOrderEnum con mismo nombre y devuelve el int de su posicion en el draft
                    // ej => de 20 obtengo 13 => id = 20 == FranchiseEnum.MIA -> DraftOrderEnum.MIA == 13 
                    int intDoe = (int)Enum.Parse<DraftOrderEnum>(((FranchiseEnum)fid).ToString());
                    for (int r = 1; r < outDto.Rounds+1; r++)
                    {
                        // por cada uno, se añaden los picks correspondiente a cada ronda
                        outDto.OthersPicks.Add((100*r)+intDoe);
                    }
                }

                if(team.Trades is not null &&  team.Trades.Count > 0)
                    foreach (Trade trade in team.Trades.OrderBy(t => t.Id))
                    {
                        bool updateFranchisePicks = inDto.OthersPicks.Contains(trade.FranchiseId);
                        foreach (int ps in trade.PicksSent)
                        {
                            outDto.TeamPicks.Remove(ps);
                            if(updateFranchisePicks && (ps%100)< outDto.Rounds+1) outDto.OthersPicks.Add(ps);
                        }
                        foreach (int pt in trade.PicksTaken)
                        {
                            outDto.TeamPicks.Add(pt);
                            if (updateFranchisePicks) outDto.OthersPicks.Remove(pt);
                        }
                    }

                outDto.OthersPicks = [.. outDto.OthersPicks.Order()];
                inDto = outDto;
            }
            catch (Exception ex)
            {
                return "Error";
            }

            return null;
        }
        #endregion Get


        #region Update
        /// <summary>  Actualizar un Team a partir de los valores de TeamDto. </summary>
        /// <param name="username">Valor de búsqeda de un User.</param>
        /// <param name="dto">Clase con los nuevos valores.</param>
        /// <returns>Opciones:
        /// Username (No existe ningún User con Username igual a parámetro).
        /// Error (No se guardaron los cambios en la BBDD).
        /// "" (Todo bien).
        /// </returns>
        public async Task<string> UpdateTeam(string? username, TeamInfoDto dto)
        {
            User? user = await _userRepository.GetUserByUsername(username);
            if (user is null) return "Username";

            Team newTeam = _mapper.Map<Team>(dto);
            newTeam.Date = DateTime.Now;
            return await _teamRepository.UpdateT(newTeam) < 1 ? "Error" : "";
        }

        public async Task<string> UpdateRosterSettings(string? username, TeamInfoDto dto)
        {
            User? user = await _userRepository.GetUserByUsername(username);
            if (user is null) return "Username";

            if (dto.Id < 1) return "TeamId";
            Team? team = await _teamRepository.GetTById(dto.Id);
            if (team is null) return "Team";

            if (user.Id != team.User.Id) return "User";

            if ((dto.RosterSettingsProtectedPlayersIds?.Count ?? 0) == 0)
                dto.RosterSettingsProtectedPlayersIds = this.GetDefaultProtectedPlayers(dto.RosterSettingsProtectedPerTeam);

            IList<int> validProtectedPlayersIds = await GetValidProtectedIds(dto.RosterSettingsProtectedPlayersIds);
            if (dto.RosterSettingsProtectedPlayersIds.Count != validProtectedPlayersIds.Count)
            {
                dto.RosterSettingsProtectedPlayersIds = validProtectedPlayersIds;
                return "ProtectedPlayersIds";
            }

            team.RosterSettings = _mapper.Map<RosterSettings>(dto);
            return await _teamRepository.UpdateT(team) < 1 ? "Error" : "";
        }

        public async Task<string> UpdateRoster(string? username, TeamDto dto)
        {
            User? user = await _userRepository.GetUserByUsername(username);
            if (user is null) return "Username";

            if (dto.Id < 1) return "TeamId";
            Team? team = await _teamRepository.GetTById(dto.Id);
            if (team is null) return "Team";

            if (user.Id != team.User.Id) return "User";

            int idCount = dto.PlayersIds?.Count ?? 0;
            if ( idCount != 0)
            {
                dto.PlayersIds = (await _playerRepository.GetManyTByIds(dto.PlayersIds)).Select(players => players.Id).ToList();
                if (dto.PlayersIds.Count != idCount) return "PlayerIds";
            }

            team.PlayersIds = dto.PlayersIds;
            return await _teamRepository.UpdateT(team) < 1 ? "Error" : "";
        }
        #endregion Update
    }
}
