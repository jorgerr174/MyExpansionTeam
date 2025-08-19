using AutoMapper;
using METCore.DTOs.Player;
using METCore.DTOs.Shared;
using METCore.DTOs.Team;
using METCore.Interfaces;
using METCore.Models;
using METCore.Models.Players;
using METCore.Models.Teams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            dto.Picks = DraftPicks.Team;
            IList<int> tradedPlayersIds = [];
            IList<int> teamPlayersIds = team.PlayersIds;

            if (team.Trades is not null && team.Trades.Count > 0)
                foreach (Trade trade in team.Trades.OrderBy(t => t.Date))
                {
                    foreach (int tpi in trade.TeamPicks)
                        dto.Picks.Remove(tpi);
                    foreach (int fpi in trade.FranchisePicks)
                        if (!dto.Picks.Contains(fpi)) dto.Picks.Add(fpi);

                    foreach (int tpl in trade.TeamPlayers)
                    {
                        if (!tradedPlayersIds.Contains(tpl)) tradedPlayersIds.Add(tpl);
                        teamPlayersIds.Remove(tpl);
                    }
                    foreach (int fpl in trade.FranchisePlayers)
                    {
                        if (!teamPlayersIds.Contains(fpl)) teamPlayersIds.Add(fpl);
                        tradedPlayersIds.Remove(fpl);
                    }
                }
            dto.TradedPlayers = [.. dto.TradedPlayers.Concat(_mapper.Map<IList<PlayerBasicDto>>(await _playerRepository.GetManyTByIds(tradedPlayersIds)))];
            if ((team.PlayersIds?.Count ?? 0) > 0) dto.Players = [.. _mapper.Map<IList<RosteredDto>>(await _playerRepository.GetManyTByIds(teamPlayersIds))];

            if ((team.Selections?.Count ?? 0) > 0)
            {
                foreach (KeyValuePair<int, int> selection in team.Selections)
                {
                    RosteredDto rookie = _mapper.Map<RosteredDto>(await _playerRepository.GetTById(selection.Value));

                    int pickAPY = DraftPicks.GetPickAPY(selection.Key);
                    rookie.PureAPY = Math.Round(pickAPY / 1000000.0, 2).ToString();
                    rookie.APY = "$" + rookie.PureAPY + "M";
                    rookie.PureAPY = rookie.PureAPY.Replace(',', '.');

                    dto.Players.Add(rookie);
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

            dto.Prospects = dto.Selections.Count < 1 ? [] : _mapper.Map<IList<ProspectDto>>(await _playerRepository.GetManyTByIds([.. dto.Selections.Values]));

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
            if (mine && user == null) return null;

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
        public async Task<string> UpdateTeam(string? username, TeamBasicInfoDto dto)
        {
            User? user = await _userRepository.GetUserByUsername(username);
            if (user is null) return "Username";

            Team newTeam = _mapper.Map<Team>(dto);

            newTeam.PlayersIds = [.. newTeam.PlayersIds.Where(p => !newTeam.Selections?.Values?.Contains(p) ?? true)];
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
            if (team.Selections != null && team.Selections.Count > 0 && team.PlayersIds != null && team.PlayersIds.Count > 0)
                team.PlayersIds = [.. team.PlayersIds.Where(p => !team.Selections.Values.Contains(p))];

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

            IList<int> teamPlayers = team.PlayersIds;
            IList<int> franchisePlayers = [.. franchise.Players.Select(p => p.Id)];

            //            if ((team.PlayersIds?.Count ?? 0) > 0) dto.TeamPlayers = [.. _mapper.Map<IList<SelectableDto>>(await _playerRepository.GetManyTByIds(team.PlayersIds))];

            dto.FranchisePicks = DraftPicks.GetFranchisePicks(dto.FranchiseId);
            //            dto.FranchisePlayers = _mapper.Map<IList<SelectableDto>>(franchise.Players);

            if (team.Trades is not null && team.Trades.Count > 0)
                foreach (Trade trade in team.Trades.OrderBy(t => t.Date))
                {
                    bool isFranchise = trade.FranchiseId == dto.FranchiseId;

                    foreach (int tpi in trade.TeamPicks)
                    {
                        dto.TeamPicks.Remove(tpi);
                        if (isFranchise) dto.FranchisePicks.Add(tpi);
                    }
                    foreach (int fpi in trade.FranchisePicks)
                    {
                        dto.TeamPicks.Add(fpi);
                        if (isFranchise) dto.FranchisePicks.Remove(fpi);
                    }

                    if (isFranchise)
                    {
                        foreach (int tpl in trade.TeamPlayers)
                            franchisePlayers.Add(tpl);
                        foreach (int fpl in trade.FranchisePlayers)
                            franchisePlayers.Remove(fpl);
                    }
                }
            dto.TeamPicks = dto.TeamPicks.OrderBy(p => p).ToList();
            dto.FranchisePicks = dto.FranchisePicks.OrderBy(p => p).ToList();

            foreach (int pId in teamPlayers)
                franchisePlayers.Remove(pId);

            dto.TeamPlayers =
                [.. _mapper.Map<IList<SelectableDto>>((await _playerRepository.GetManyTByIds(teamPlayers)).OrderBy(p => p.Position).ThenByDescending(p => p.APY))];
            dto.FranchisePlayers =
                [.. _mapper.Map<IList<SelectableDto>>((await _playerRepository.GetManyTByIds(franchisePlayers)).OrderBy(p => p.Position).ThenByDescending(p => p.APY))];

            return null;
        }

        public async Task<string?> SaveTrade(string username, TradeDto dto)
        {
            User? user = await _userRepository.GetUserByUsername(username);
            if (user is null) return "Username";

            if (dto.TeamId < 1) return "TeamId";
            Team? team = await _teamRepository.GetTById(dto.TeamId);
            if (team is null) return "Team";

            if (user.Id != team.User.Id) return "User";

            if (!dto.Force && !await ValidTrade(dto)) return "Unfair";

            dto.Date = DateOnly.FromDateTime(DateTime.Now);
            Trade trade = _mapper.Map<Trade>(dto);

            if (await _tradeRepository.CreateT(trade) < 1) return "Error";

            foreach (int tpl in trade.TeamPlayers)
                team.PlayersIds.Remove(tpl);
            foreach (int fpl in trade.FranchisePlayers)
                if (!team.PlayersIds.Contains(fpl)) team.PlayersIds.Add(fpl);

            return await _teamRepository.UpdateT(team) < 1 ? "Error" : null;
        }

        private async Task<bool> ValidTrade(TradeDto dto)
        {

            int teamValue = 0;
            int franchiseValue = 0;

            foreach (int pick in dto.TeamPicks)
                teamValue += DraftPicks.GetPickValue(pick);
            foreach (int pick in dto.FranchisePicks)
                franchiseValue += DraftPicks.GetPickValue(pick);

            foreach (Player player in await _playerRepository.GetManyTByIds(dto.TeamPlayers.Select(p => p.Id).ToList()))
                teamValue += DraftPicks.GetPlayerValue(player);
            foreach (Player player in await _playerRepository.GetManyTByIds(dto.FranchisePlayers.Select(p => p.Id).ToList()))
                franchiseValue += DraftPicks.GetPlayerValue(player);

            return Math.Abs(teamValue - franchiseValue) < 50;
        }

        public async Task<ResultDto<IList<TradeDto>>> GetTeamTrades(string username, int TeamId)
        {
            User? user = await _userRepository.GetUserByUsername(username);
            if (user is null) return new ResultDto<IList<TradeDto>>("Username");

            if (TeamId < 1) return new ResultDto<IList<TradeDto>>("TeamId");
            Team? team = await _teamRepository.GetTById(TeamId);
            if (team is null) return new ResultDto<IList<TradeDto>>("Team");

            if (user.Id != team.User.Id) return new ResultDto<IList<TradeDto>>("User");

            IList<TradeDto> list = [];
            TradeDto dto;
            foreach (Trade trade in team.Trades)
            {
                dto = _mapper.Map<TradeDto>(trade);
                if (trade.TeamPlayers.Count > 0)
                    dto.TeamPlayers = _mapper.Map<IList<SelectableDto>>(await _playerRepository.GetManyTByIds(trade.TeamPlayers));
                if (trade.FranchisePlayers.Count > 0)
                    dto.FranchisePlayers = _mapper.Map<IList<SelectableDto>>(await _playerRepository.GetManyTByIds(trade.FranchisePlayers));

                list.Add(dto);
            }

            return new ResultDto<IList<TradeDto>>(string.Empty, list);
        }
        #endregion Trade


        #region Draft
        [HttpPost("SaveDraft")]
        [Authorize]
        public async Task<string?> SaveDraft(string username, DraftDto dto)
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
            team.Selections = dto.Selections;

            return await _teamRepository.UpdateT(team) < 1 ? "Error" : null;
        }
        #endregion Draft


        #region DeleteTeam
        public async Task<string> DeleteTeam(string username, int Id)
        {
            User? user = await _userRepository.GetUserByUsername(username);
            Team? team = await _teamRepository.GetTById(Id);

            return team is null ? "Team"
                : team.User != user ? "User"
                    : await _teamRepository.DeleteT(team) < 1 ? "Error" : "";
        }
        #endregion DeleteTeam


        #region DuplicateTeam
        public async Task<ResultDto<TeamBasicInfoDto>> DuplicateTeam(string username, int Id)
        {
            User? user = await _userRepository.GetUserByUsername(username);
            if (user is null) return new ResultDto<TeamBasicInfoDto>("Username");
            Team? team = await _teamRepository.GetTById(Id);
            if (team is null) return new ResultDto<TeamBasicInfoDto>("Team");

            Team newTeam = (Team)((ICloneable)team).Clone();
            newTeam.Id = 0;
            newTeam.User = user;
            newTeam.Date = DateTime.Now;
            newTeam.Complete = false;

            bool created = await _teamRepository.CreateT(newTeam) > 0;
            return new(
                created ? string.Empty : "Error",
                created ? await GetBasicInfoDtoById(newTeam.Id) : null);
        }
        #endregion DuplicateTeam
    }
}
