using METCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace METAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class PlayersController(PlayerService playerService) : ControllerBase
    {
        private readonly PlayerService _playerService = playerService;


        #region GetDraftProspects
        /// <summary>
        /// Obtener prospectos del draft para un año específico.
        /// </summary>
        /// <param name="Year">Año del draft</param>
        /// <returns>Lista de prospectos disponibles para el draft del año especificado</returns>
        [HttpGet("GetDraftProspects")]
        public async Task<IActionResult> GetDraftProspects(int Year)
        {
            return Ok(await _playerService.GetDraftProspects(Year));
        }
        #endregion GetDraftProspects
    }
}