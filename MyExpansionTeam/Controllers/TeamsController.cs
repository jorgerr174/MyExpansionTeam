using METCore.DTOs.Shared;
using METCore.DTOs.Team;
using METCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace METAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class TeamsController(TeamService teamService) : ControllerBase
    {
        private readonly TeamService _teamService = teamService;

        #region Private
        private async Task<IEnumerable<TeamInfoDto>?> ListTeams(bool? mine = false, string? username = null)
        {
            return await _teamService.ListTeams(mine ?? false, username);
        }
        #endregion Private


        #region CU006 Create
        /// <summary>
        /// Crear un nuevo Team.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>Opciones:
        /// Username (No se encontró ningún User con ese username).
        /// Abb (Abb vacío).
        /// Location (Location vacío).
        /// Mascot (Mascot vacío).
        /// Error (No se guardaron los cambios en la BBDD).
        /// Nada (Todo bien).
        /// </returns>
        [HttpPost("Create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] TeamBasicInfoDto dto)
        {
            string result = await _teamService.ValidateTeamBasicInfo(dto, User?.Identity?.Name);
            if (!String.IsNullOrWhiteSpace(result)) BadRequest(new MessageDto(result));

            TeamInfoDto dto2 = await _teamService.CreateTeamFromBasicInfo(dto, User?.Identity?.Name);
            return dto2 is null ? BadRequest(new MessageDto("Error")) : Ok(dto2);
        }
        #endregion CU006 Create


        #region Get
        /// <summary> Obtener el TeamDto con los valores de un Team.</summary>
        /// <returns>Opciones:
        /// Nada (no se encontró ningún Team con ese Id).
        /// TeamDto (Con los valores del Team encontrado).
        /// </returns>
        [HttpGet("Team")]
        public async Task<IActionResult> Team(int TeamId)
        {
            if (TeamId < 1) return BadRequest();
            TeamDto? team = await _teamService.GetDtoById(TeamId);
            return team == null ? BadRequest() : Ok(team);
        }

        /// <summary> Obtener el TeamDto con los valores de un Team.</summary>
        /// <returns>Opciones:
        /// Nada (no se encontró ningún Team con ese Id).
        /// TeamDto (Con los valores del Team encontrado).
        /// </returns>
        [HttpGet("TeamInfo")]
        public async Task<IActionResult> TeamInfo(int TeamId)
        {
            if (TeamId < 1) return BadRequest();
            TeamInfoDto? team = await _teamService.GetInfoDtoById(TeamId);
            return team == null ? BadRequest() : Ok(team);
        }

        /// <summary> Obtener el TeamDto con los valores de un Team.</summary>
        /// <returns>Opciones:
        /// Nada (no se encontró ningún Team con ese Id).
        /// TeamDto (Con los valores del Team encontrado).
        /// </returns>
        [HttpGet("TeamDraft")]
        public async Task<IActionResult> TeamDraft(int TeamId)
        {
            if (TeamId < 1) return BadRequest();
            DraftDto? team = await _teamService.GetTeamDraftDtoById(TeamId);
            return team == null ? BadRequest() : Ok(team);
        }

        /// <summary> Obtener el TeamDto con los valores de un Team.</summary>
        /// <returns>Opciones:
        /// Nada (no se encontró ningún Team con ese Id).
        /// TeamDto (Con los valores del Team encontrado).
        /// </returns>
        [HttpGet("TeamBasicInfo")]
        public async Task<IActionResult> TeamBasicInfo(int TeamId)
        {
            if (TeamId < 1) return BadRequest();
            TeamBasicInfoDto? team = await _teamService.GetBasicInfoDtoById(TeamId);
            return team == null ? BadRequest() : Ok(team);
        }

        [HttpGet("List")]
        public async Task<IActionResult> List()
        {
            IEnumerable<TeamInfoDto>? list = await this.ListTeams(false, User?.Identity?.Name);
            return Ok(list);
        }

        /// <summary> Obtener los TeamDtos con los valores de los Teams del User logeado.</summary>
        /// <returns>Opciones:
        /// Username (no se encontró ningún User para username).
        /// IEnumerable<TeamDto>? (Con los valores de los Teams encontrados).
        /// </returns>
        [HttpGet("MyTeams")]
        [Authorize]
        public async Task<IActionResult> MyTeams()
        {
            if (User?.Identity?.Name == null) return BadRequest(new MessageDto("Username"));

            IEnumerable<TeamInfoDto>? list = await this.ListTeams(true, User?.Identity?.Name);
            return list == null ? BadRequest(new MessageDto("Username")) : Ok(list);
        }
        #endregion Get


        #region Update
        /// <summary>Actualizar un Team a partir de los valores de TeamDto. </summary>
        /// <param name="dto">Clase con los nuevos valores.</param>
        /// <returns>Opciones:
        /// Username (No existe ningún User con Username igual a parámetro).
        /// Error (No se guardaron los cambios en la BBDD).
        /// Nada (Todo bien).
        /// </returns>
        [HttpPut("UpdateTeam")]
        [Authorize]
        public async Task<IActionResult> UpdateTeam([FromBody] TeamBasicInfoDto dto)
        {
            string? username = User?.Identity?.Name;
            if (String.IsNullOrWhiteSpace(username)) return BadRequest(new MessageDto("Username"));

            string result = await _teamService.UpdateTeam(User?.Identity?.Name, dto);

            if (!String.IsNullOrWhiteSpace(result)) return BadRequest(new MessageDto(result));
            return Ok();
        }

        [HttpPost("UpdateRosterSettings")]
        [Authorize]
        public async Task<IActionResult> UpdateRosterSettings([FromBody] TeamInfoDto dto)
        {
            string? username = User?.Identity?.Name;
            if (String.IsNullOrWhiteSpace(username)) return BadRequest(new MessageDto("Username"));

            string result = await _teamService.UpdateRosterSettings(User?.Identity?.Name, dto);

            return !String.IsNullOrWhiteSpace(result)
                ? BadRequest(new ResultDto<TeamInfoDto>(result, dto))
                : Ok();
        }

        [HttpPost("UpdateRoster")]
        [Authorize]
        public async Task<IActionResult> UpdateRoster([FromBody] TeamDto dto)
        {
            string? username = User?.Identity?.Name;
            if (String.IsNullOrWhiteSpace(username)) return BadRequest(new MessageDto("Username"));

            string result = await _teamService.UpdateRoster(User?.Identity?.Name, dto);

            return !String.IsNullOrWhiteSpace(result)
                ? BadRequest(new ResultDto<TeamInfoDto>(result, dto))
                : Ok(dto);
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
        [HttpPost("GetTradeDto")]
        [Authorize]
        public async Task<IActionResult> GetTradeDto([FromBody] TradeDto dto)
        {
            string? username = User?.Identity?.Name;
            if (String.IsNullOrWhiteSpace(username)) return BadRequest(new MessageDto("Username"));

            string? result = await _teamService.GetTradeDto(User?.Identity?.Name, dto);

            return !String.IsNullOrWhiteSpace(result)
                ? BadRequest(new MessageDto(result))
                : Ok(dto);
        }

        [HttpPost("SaveTrade")]
        [Authorize]
        public async Task<IActionResult> SaveTrade([FromBody] TradeDto dto)
        {
            string? username = User?.Identity?.Name;
            if (String.IsNullOrWhiteSpace(username)) return BadRequest(new MessageDto("Username"));

            string result = await _teamService.SaveTrade(User?.Identity?.Name, dto);

            return Ok(new ResultDto<TradeDto>(result, dto));
        }

        [HttpGet("GetTeamTrades")]
        public async Task<IActionResult> GetTeamTrades(int TeamId)
        {
            ResultDto<IList<TradeDto>> result = await _teamService.GetTeamTrades(TeamId);

            return !String.IsNullOrWhiteSpace(result.Message)
                ? BadRequest(new MessageDto(result.Message))
                : Ok(result.Value);
        }
        #endregion Trade


        #region Draft
        [HttpPost("SaveDraft")]
        [Authorize]
        public async Task<IActionResult> SaveDraft([FromBody] DraftDto dto)
        {
            string? username = User?.Identity?.Name;
            if (String.IsNullOrWhiteSpace(username)) return BadRequest(new MessageDto("Username"));

            string result = await _teamService.SaveDraft(User?.Identity?.Name, dto);

            return !String.IsNullOrWhiteSpace(result) ? BadRequest() : Ok();
        }
        #endregion Draft


        #region CU008 DeleteTeam
        [HttpDelete("DeleteTeam")]
        [Authorize]
        public async Task<IActionResult> DeleteTeam(IdDto dto)
        {
            string? username = User?.Identity?.Name;
            if (String.IsNullOrWhiteSpace(username)) return BadRequest(new MessageDto("Username"));

            string result = await _teamService.DeleteTeam(username, dto.Id);
            if (!String.IsNullOrWhiteSpace(result))
                return BadRequest(new ResultDto<TeamInfoDto>(result, await _teamService.GetInfoDtoById(dto.Id)));

            return Ok();
        }
        #endregion CU008 DeleteTeam


        #region CU009 DuplicateTeam
        [HttpPost("DuplicateTeam")]
        [Authorize]
        public async Task<IActionResult> DuplicateTeam(IdDto dto)
        {
            string? username = User?.Identity?.Name;
            if (String.IsNullOrWhiteSpace(username)) return BadRequest(new MessageDto("Username"));

            ResultDto<TeamBasicInfoDto> result = await _teamService.DuplicateTeam(username, dto.Id);

            return !String.IsNullOrWhiteSpace(result.Message)
                ? BadRequest(result)
                : Ok(result);
        }
        #endregion CU009 DuplicateTeam
    }
}
