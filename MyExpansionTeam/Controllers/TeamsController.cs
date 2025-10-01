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
        /// <param name="dto">Información básica del equipo (TeamBasicInfoDto)</param>
        /// <returns>Información del equipo creado</returns>
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
        #region Team
        /// <summary>
        /// Obtener el TeamDto completo con los valores de un Team.
        /// </summary>
        /// <param name="TeamId">ID del equipo</param>
        /// <returns>Información completa del equipo incluyendo jugadores y formaciones</returns>
        [HttpGet("Team")]
        public async Task<IActionResult> Team(int TeamId)
        {
            if (TeamId < 1) return BadRequest();
            TeamDto? team = await _teamService.GetDtoById(TeamId);
            return team == null ? BadRequest() : Ok(team);
        }
        #endregion Team

        #region TeamInfo
        /// <summary>
        /// Obtener información básica de un Team.
        /// </summary>
        /// <param name="TeamId">ID del equipo</param>
        /// <returns>Información del equipo</returns>
        [HttpGet("TeamInfo")]
        public async Task<IActionResult> TeamInfo(int TeamId)
        {
            if (TeamId < 1) return BadRequest();
            TeamInfoDto? team = await _teamService.GetInfoDtoById(TeamId);
            return team == null ? BadRequest() : Ok(team);
        }
        #endregion TeamInfo

        #region TeamBasicInfo
        /// <summary>
        /// Obtener información básica de un Team.
        /// </summary>
        /// <param name="TeamId">ID del equipo</param>
        /// <returns>Información básica del equipo</returns>
        [HttpGet("TeamBasicInfo")]
        public async Task<IActionResult> TeamBasicInfo(int TeamId)
        {
            if (TeamId < 1) return BadRequest();
            TeamBasicInfoDto? team = await _teamService.GetBasicInfoDtoById(TeamId);
            return team == null ? BadRequest() : Ok(team);
        }
        #endregion TeamBasicInfo

        #region List
        /// <summary>
        /// Obtener listado de todos los equipos.
        /// </summary>
        /// <returns>IEnumerable<TeamInfoDto> con información de todos los equipos.</returns>
        [HttpGet("List")]
        public async Task<IActionResult> List()
        {
            IEnumerable<TeamInfoDto>? list = await this.ListTeams(false, User?.Identity?.Name);
            return Ok(list);
        }
        #endregion List

        #region MyTeams
        /// <summary>
        /// Obtener los equipos del usuario logeado.
        /// </summary>
        /// <returns>Lista de equipos del usuario autenticado</returns>
        [HttpGet("MyTeams")]
        [Authorize]
        public async Task<IActionResult> MyTeams()
        {
            if (User?.Identity?.Name == null) return BadRequest(new MessageDto("Username"));

            IEnumerable<TeamInfoDto>? list = await this.ListTeams(true, User?.Identity?.Name);
            return list == null ? BadRequest(new MessageDto("Username")) : Ok(list);
        }
        #endregion MyTeams
        #endregion Get


        #region Update
        #region UpdateTeam
        /// <summary>
        /// Actualizar un Team a partir de los valores de TeamBasicInfoDto.
        /// </summary>
        /// <param name="dto">Clase con los nuevos valores (TeamBasicInfoDto)</param>
        /// <returns>Resultado de la actualización</returns>
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
        #endregion UpdateTeam

        #region UpdateRosterSettings
        /// <summary>
        /// Actualizar configuración de roster de un equipo.
        /// </summary>
        /// <param name="dto">Configuración del roster (TeamInfoDto)</param>
        /// <returns>Resultado de la actualización de configuración</returns>
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
        #endregion UpdateRosterSettings

        #region UpdateRoster
        /// <summary>
        /// Actualizar roster completo de un equipo.
        /// </summary>
        /// <param name="dto">Datos completos del equipo (TeamDto)</param>
        /// <returns>Resultado de la actualización del roster</returns>
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
        #endregion UpdateRoster
        #endregion Update


        #region Trade
        #region GetTradeDto
        /// <summary>
        /// Obtener información para realizar un intercambio.
        /// </summary>
        /// <param name="dto">Datos del intercambio (TradeDto)</param>
        /// <returns>Información detallada del intercambio con jugadores disponibles</returns>
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
        #endregion GetTradeDto

        #region SaveTrade
        /// <summary>
        /// Guardar un intercambio realizado.
        /// </summary>
        /// <param name="dto">Datos del intercambio (TradeDto)</param>
        /// <returns>Resultado del intercambio guardado</returns>
        [HttpPost("SaveTrade")]
        [Authorize]
        public async Task<IActionResult> SaveTrade([FromBody] TradeDto dto)
        {
            string? username = User?.Identity?.Name;
            if (String.IsNullOrWhiteSpace(username)) return BadRequest(new MessageDto("Username"));

            string result = await _teamService.SaveTrade(User?.Identity?.Name, dto);

            return Ok(new ResultDto<TradeDto>(result, dto));
        }
        #endregion SaveTrade

        #region GetTeamTrades
        /// <summary>
        /// Obtener todos los intercambios de un equipo.
        /// </summary>
        /// <param name="TeamId">ID del equipo</param>
        /// <returns>Lista de intercambios del equipo</returns>
        [HttpGet("GetTeamTrades")]
        public async Task<IActionResult> GetTeamTrades(int TeamId)
        {
            ResultDto<IList<TradeDto>> result = await _teamService.GetTeamTrades(TeamId);

            return !String.IsNullOrWhiteSpace(result.Message)
                ? BadRequest(new MessageDto(result.Message))
                : Ok(result.Value);
        }
        #endregion GetTeamTrades
        #endregion Trade


        #region Draft
        #region TeamDraft
        /// <summary>
        /// Obtener información del draft de un Team.
        /// </summary>
        /// <param name="TeamId">ID del equipo</param>
        /// <returns>Información del draft del equipo incluyendo prospectos y selecciones</returns>
        [HttpGet("TeamDraft")]
        public async Task<IActionResult> TeamDraft(int TeamId)
        {
            if (TeamId < 1) return BadRequest();
            DraftDto? team = await _teamService.GetTeamDraftDtoById(TeamId);
            return team == null ? BadRequest() : Ok(team);
        }
        #endregion TeamDraft

        #region SaveDraft
        /// <summary>
        /// Guardar selecciones del draft.
        /// </summary>
        /// <param name="dto">Selecciones del draft (DraftDto)</param>
        /// <returns>Resultado del guardado del draft</returns>
        [HttpPost("SaveDraft")]
        [Authorize]
        public async Task<IActionResult> SaveDraft([FromBody] DraftDto dto)
        {
            string? username = User?.Identity?.Name;
            if (String.IsNullOrWhiteSpace(username)) return BadRequest(new MessageDto("Username"));

            string result = await _teamService.SaveDraft(User?.Identity?.Name, dto);

            return !String.IsNullOrWhiteSpace(result) ? BadRequest() : Ok();
        }
        #endregion SaveDraft
        #endregion Draft


        #region CU008 DeleteTeam
        /// <summary>
        /// Eliminar un equipo.
        /// </summary>
        /// <param name="dto">ID del equipo a eliminar (IdDto)</param>
        /// <returns>Resultado de la eliminación</returns>
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
        /// <summary>
        /// Duplicar un equipo existente.
        /// </summary>
        /// <param name="dto">ID del equipo a duplicar (IdDto)</param>
        /// <returns>Información del equipo duplicado</returns>
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
