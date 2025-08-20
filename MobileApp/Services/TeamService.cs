using METCore.DTOs.Admin;
using METCore.DTOs.Player;
using METCore.DTOs.Shared;
using METCore.DTOs.Team;
using METCore.DTOs.User;

namespace MobileApp.Services
{
    public class TeamService(IHttpClientFactory httpClientFactory) : BaseService(httpClientFactory)
    {
        #region GetTeamDetails
        public async Task<TeamInfoDto?> GetTeamDetailsAsync(int teamId)
        {
            try
            {
                var response = await SendRequest(HttpMethod.Get, "Teams", "Team", new IdDto(teamId));
                return response.IsSuccessStatusCode ? await GetResult<TeamInfoDto>(response) : null;
            }
            catch { return null; }
        }
        #endregion GetTeamDetails


        #region GetTeamRoster
        public async Task<TeamDto?> GetTeamRosterAsync(int teamId)
        {
            try
            {
                var response = await SendRequest(HttpMethod.Get, "Teams", "Roster", new IdDto(teamId));
                return response.IsSuccessStatusCode ? await GetResult<TeamDto>(response) : null;
            }
            catch { return null; }
        }
        #endregion GetTeamRoster


        #region GetMyTeams
        public async Task<IEnumerable<TeamInfoDto>?> GetMyTeamsAsync()
        {
            try
            {
                var response = await SendRequest(HttpMethod.Get, "Teams", "MyTeams");
                return response.IsSuccessStatusCode ? await GetResult<IEnumerable<TeamInfoDto>>(response) : null;
            }
            catch { return null; }
        }
        #endregion GetMyTeams


        #region GetAllTeams
        public async Task<IEnumerable<TeamInfoDto>?> GetAllTeamsAsync()
        {
            try
            {
                var response = await SendRequest(HttpMethod.Get, "Teams", "List");
                return response.IsSuccessStatusCode ? await GetResult<IEnumerable<TeamInfoDto>>(response) : null;
            }
            catch { return null; }
        }
        #endregion GetAllTeams


        #region GetProtectablePlayers
        public async Task<IList<SelectableDto>?> GetProtectablePlayersAsync(int teamId)
        {
            try
            {
                var response = await SendRequest(HttpMethod.Get, "Teams", "GetProtectablePlayers", new IdDto(teamId));
                return response.IsSuccessStatusCode ? await GetResult<IList<SelectableDto>>(response) : null;
            }
            catch { return null; }
        }
        #endregion GetProtectablePlayers


        #region GetSelectablePlayersAsync
        public async Task<IList<SelectableDto>?> GetSelectablePlayersAsync(int teamId)
        {
            try
            {
                var response = await SendRequest(HttpMethod.Get, "Teams", "GetSelectablePlayers", new IdDto(teamId));
                return response.IsSuccessStatusCode ? await GetResult<IList<SelectableDto>>(response) : null;
            }
            catch { return null; }
        }
        #endregion GetSelectablePlayersAsync


        #region CU006 CreateTeam
        public async Task<int?> CreateTeamAsync(string location, string abb, string mascot)
        {
            try
            {
                var teamDto = new TeamBasicInfoDto(0, location, abb, mascot, "", DateTime.Now, null);
                var response = await SendRequest(HttpMethod.Post, "Teams", "Create", teamDto);
                return response.IsSuccessStatusCode ? (await GetResult<TeamInfoDto>(response)).Id : null;
            }
            catch { return null; }
        }
        #endregion CU006 CreateTeam


        #region CU007 UpdateTeam
        public async Task<bool> UpdateTeamAsync(int teamId, string location, string abb, string mascot)
        {
            try
            {
                var teamDto = new TeamBasicInfoDto(teamId, location, abb, mascot, "", DateTime.Now, null);
                var response = await SendRequest(HttpMethod.Put, "Teams", "Edit", teamDto);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }
        #endregion CU007 UpdateTeam


        #region CU008 DuplicateTeam
        public async Task<TeamBasicInfoDto?> DuplicateTeamAsync(int teamId)
        {
            try
            {
                var response = await SendRequest(HttpMethod.Post, "Teams", "DuplicateTeam", new IdDto(teamId));
                if (response.IsSuccessStatusCode)
                {
                    var result = await GetResult<ResultDto<TeamBasicInfoDto>>(response);
                    return result.Value;
                }
                return null;
            }
            catch { return null; }
        }
        #endregion CU008 DuplicateTeam


        #region CU009 DeleteTeam
        public async Task<bool> DeleteTeamAsync(int teamId)
        {
            try
            {
                var response = await SendRequest(HttpMethod.Delete, "Teams", "DeleteTeam", new IdDto(teamId));
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }
        #endregion CU009 DeleteTeam


        #region GetRosterSettingsAsync
        public async Task<TeamInfoDto?> GetRosterSettingsAsync(int teamId)
        {
            try
            {
                var response = await SendRequest(HttpMethod.Get, "Teams", "TeamInfo", new IdDto(teamId));
                return response.IsSuccessStatusCode ? await GetResult<TeamInfoDto>(response) : null;
            }
            catch { return null; }
        }
        #endregion GetRosterSettingsAsync


        #region CU010 UpdateRosterSettings
        public async Task<bool> UpdateRosterSettingsAsync(TeamInfoDto settings)
        {
            try
            {
                var response = await SendRequest(HttpMethod.Put, "Teams", "UpdateRosterSettings", settings);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }
        #endregion CU010 UpdateRosterSettings


        #region CU011 UpdateRoster
        public async Task<bool> UpdateRosterAsync(TeamDto teamDto)
        {
            try
            {
                var response = await SendRequest(HttpMethod.Put, "Teams", "UpdateRoster", teamDto);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }
        #endregion CU011 UpdateRoster


        #region GetTeamTrades
        public async Task<IList<TradeDto>?> GetTeamTradesAsync(int teamId)
        {
            try
            {
                var response = await SendRequest(HttpMethod.Get, "Teams", "GetTeamTrades", new IdDto(teamId));
                return response.IsSuccessStatusCode ? await GetResult<IList<TradeDto>>(response) : null;
            }
            catch { return null; }
        }
        #endregion GetTeamTrades


        #region GetTeamDraft
        public async Task<DraftDto?> GetTeamDraftAsync(int teamId)
        {
            try
            {
                var response = await SendRequest(HttpMethod.Get, "Teams", "TeamDraft", new IdDto(teamId));
                return response.IsSuccessStatusCode ? await GetResult<DraftDto>(response) : null;
            }
            catch { return null; }
        }
        #endregion GetTeamDraft


        public async Task<TradeDto?> GetTradeDataAsync(int teamId, int franchiseId)
        {
            try
            {
                var tradeDto = new TradeDto(teamId, franchiseId);
                var response = await SendRequest(HttpMethod.Post, "Teams", "GetTradeDto", tradeDto);
                return response.IsSuccessStatusCode ? await GetResult<TradeDto>(response) : null;
            }
            catch { return null; }
        }

        public async Task<bool> SaveTradeAsync(TradeDto tradeDto)
        {
            try
            {
                var response = await SendRequest(HttpMethod.Post, "Teams", "SaveTrade", tradeDto);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        // Add these methods to the existing TeamService class

        public async Task SaveDraftProgressAsync(DraftDto draftData)
        {
            var response = await SendRequest(HttpMethod.Post, "Teams", "SaveDraftProgress", draftData);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to save draft progress: {error}");
            }
        }

        public async Task<IEnumerable<ProspectDto>> GetDraftProspectsAsync(int year = 0)
        {
            try
            {
                if (year == 0) year = DateTime.Now.Year;
                var response = await SendRequest(HttpMethod.Get, "Players", "GetDraftProspects", new IdDto(year));
                return response.IsSuccessStatusCode ? await GetResult<IEnumerable<ProspectDto>>(response) : [];
            }
            catch { return []; }
        }

        public async Task<bool> SaveDraftAsync(DraftDto draftDto)
        {
            try
            {
                var response = await SendRequest(HttpMethod.Post, "Teams", "SaveDraft", draftDto);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<SearchResultDto<UserDto>?> GetUsersAsync(SearchDto searchDto)
        {
            try
            {
                var response = await SendRequest(HttpMethod.Get, "Users", "List", searchDto);
                return response.IsSuccessStatusCode ? await GetResult<SearchResultDto<UserDto>>(response) : null;
            }
            catch { return null; }
        }

        public async Task<bool> AssignRoleAsync(AssignRoleDto assignRoleDto)
        {
            try
            {
                var response = await SendRequest(HttpMethod.Post, "Auth", "AssignRole", assignRoleDto);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }
    }
}