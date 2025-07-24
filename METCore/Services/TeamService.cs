using AutoMapper;
using METCore.DTOs.Player;
using METCore.DTOs.Team;
using METCore.Interfaces;
using METCore.Models;
using METCore.Models.Teams;
using Microsoft.Extensions.Configuration;
using static METCore.Enums.Types;


namespace METCore.Services
{
    public class TeamService(IConfiguration configuration, ITeamRepository teamRepository, IPlayerRepository playerRepository,
        IUserRepository userRepository, IFranchiseRepository franchiseRepository, ITradeRepository tradeRepository, IMapper mapper)
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ITeamRepository _teamRepository = teamRepository;
        private readonly IPlayerRepository _playerRepository = playerRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IFranchiseRepository _franchiseRepository = franchiseRepository;
        private readonly ITradeRepository _tradeRepository = tradeRepository;
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
            if (team is null) return null;

            TeamDto dto = _mapper.Map<TeamDto>(team);
            if (team.Trades is not null && team.Trades.Count > 0)
                foreach (Trade trade in team.Trades.OrderBy(t => t.Id))
                {
                    foreach (int tpi in trade.TeamPicks)
                        dto.Picks.Remove(tpi);
                    foreach (int fpi in trade.FranchisePicks)
                        dto.Picks.Add(fpi);

                    dto.TradedPlayers = [.. dto.TradedPlayers, _mapper.Map<PlayerBasicDto>(_playerRepository.GetManyTByIds(trade.TeamPlayers))];
                }
            if ((team.PlayersIds?.Count ?? 0) > 0) dto.Players = [.. _mapper.Map<IList<RosteredDto>>(await _playerRepository.GetManyTByIds(team.PlayersIds))];
            return dto;
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
            if (team is null) return null;
            DraftDto dto = _mapper.Map<DraftDto>(team);

            if (team.Trades is not null && team.Trades.Count > 0)
                foreach (Trade trade in team.Trades.OrderBy(t => t.Id))
                {
                    foreach (int tp in trade.FranchisePicks)
                    {
                        dto.Picks[0].Remove(tp);
                        dto.Picks[trade.FranchiseId].Add(tp);
                    }
                    foreach (int fp in trade.FranchisePlayers)
                    {
                        dto.Picks[0].Remove(fp);
                        dto.Picks[trade.FranchiseId].Add(fp);
                    }
                }
            return dto;
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

        private List<int> GetDefaultProtectedPlayers(int numPerFranchise)
        {
            return _franchiseRepository.GetDefaultProtected(numPerFranchise).Select(p => p.Id).ToList();
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

            int idCount = dto.SelectedIds?.Count ?? 0;
            if (idCount != 0 && await _playerRepository.CountManyTByIds(dto.SelectedIds) != idCount) return "PlayerIds";

            team.PlayersIds = dto.SelectedIds;

            team.OffLineup = _mapper.Map<Lineup>(dto.OffLineup);
            team.DefLineup = _mapper.Map<Lineup>(dto.DefLineup);
            team.SPLineup = _mapper.Map<SPLineup>(dto.SPLineup);

            return await _teamRepository.UpdateT(team) < 1 ? "Error" : "";
        }
        #endregion Update


        #region Trade
        /// <summary>Actualizar un Team a partir de los valores de TeamDto. </summary>
        /// <param name="dto">Clase con los nuevos valores.</param>
        /// <returns>Opciones:
        /// Username (No existe ningún User con Username igual a parámetro).
        /// Error (No se guardaron los cambios en la BBDD).
        /// Nada (Todo bien).
        /// </returns>
        public async Task<string?> GetTradeDto(string username, TradeDto dto)
        {
            User? user = await _userRepository.GetUserByUsername(username);
            if (user is null) return "Username";

            if (dto.TeamId < 1) return "TeamId";
            Team? team = await _teamRepository.GetTById(dto.TeamId);
            if (team is null) return "Team";

            if (user.Id != team.User.Id) return "User";

            Franchise? franchise = await _franchiseRepository.GetTById(dto.FranchiseId);
            if (franchise is null) return "FranchiseId";

            dto.Force = false;

            dto.TeamPicks = DraftPicks.Team;
            if ((team.PlayersIds?.Count ?? 0) > 0) dto.TeamPlayers = [.. _mapper.Map<IList<SelectableDto>>(await _playerRepository.GetManyTByIds(team.PlayersIds))];
            dto.TeamCurrentCap = dto.TeamPlayers.Sum(tpl => decimal.Round(decimal.Parse(tpl.PureAPY.Replace('.', ',')), 2));

            dto.FranchisePicks = DraftPicks.GetFranchisePicks(dto.FranchiseId);
            dto.FranchisePlayers = _mapper.Map<IList<SelectableDto>>(franchise.Players);

            if (team.Trades is not null && team.Trades.Count > 0)
                foreach (Trade trade in team.Trades.OrderBy(t => t.Id))
                {
                    bool isFranchise = trade.FranchiseId == dto.FranchiseId;

                    foreach (int tpl in trade.TeamPlayers)
                    {
                        if (isFranchise) dto.FranchisePlayers.Add(_mapper.Map<SelectableDto>(_playerRepository.GetTById(tpl)));
                    }
                    foreach (int tpi in trade.TeamPicks)
                    {
                        dto.TeamPicks.Remove(tpi);
                        if (isFranchise) dto.FranchisePicks.Add(tpi);
                    }

                    foreach (int fpl in trade.FranchisePlayers)
                    {
                        SelectableDto franchisePlayer = _mapper.Map<SelectableDto>(_playerRepository.GetTById(fpl));
                        dto.TeamPlayers.Add(franchisePlayer);
                        if (isFranchise) dto.FranchisePlayers.Remove(dto.FranchisePlayers.First(pl => pl.Id == fpl));
                    }
                    foreach (int fpi in trade.FranchisePicks)
                    {
                        dto.TeamPicks.Add(fpi);
                        if (isFranchise) dto.FranchisePicks.Remove(fpi);
                    }
                }

            return null;
        }

        public async Task<string?> SaveTrade(string username, TradeDto dto)
        {
            User? user = await _userRepository.GetUserByUsername(username);
            if (user is null) return "Username";

            if (dto.Id < 1) return "TeamId";
            Team? team = await _teamRepository.GetTById(dto.Id);
            if (team is null) return "Team";

            if (user.Id != team.User.Id) return "User";

            // lógica de:
            // trade is not Force
            // and control si trade es equilibrado

            Trade trade = _mapper.Map<Trade>(dto);

            return await _tradeRepository.CreateT(trade) < 1 ? "Error" : null;
        }
        #endregion Trade
    }
}
