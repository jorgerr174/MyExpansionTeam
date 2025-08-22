using METCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace METAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class PlayersController(PlayerService playerService) : ControllerBase
    {
        private readonly PlayerService _playerService = playerService;


        #region Get
        /// <summary> Obtener los TeamDtos con los valores de los Teams del User logeado.</summary>
        /// <returns>Opciones:
        /// Username (no se encontró ningún User para username).
        /// IEnumerable<TeamDto>? (Con los valores de los Teams encontrados).
        /// </returns>
        [HttpGet("GetDraftProspects")]
        public async Task<IActionResult> GetDraftProspects(int Year)
        {
            return Ok(await _playerService.GetDraftProspects(Year));
        }
        #endregion Get
    }
}