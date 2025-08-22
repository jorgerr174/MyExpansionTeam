using METCore.DTOs.Player;
using METCore.DTOs.Shared;
using METCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace METAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class FranchisesController(FranchiseService franchiseService) : ControllerBase
    {
        private readonly FranchiseService _franchiseService = franchiseService;


        #region Get Protectable Players
        [HttpGet("GetProtectablePlayers")]
        public async Task<IActionResult> GetProtectablePlayers(int FranchiseId)
        {
            if (FranchiseId < 1 || FranchiseId > 32) return BadRequest(new MessageDto("<h2 style='color:red'>FranchiseId no válido</h2>"));

            IList<ProtectableDto> players = await _franchiseService.GetProtectablePlayers(FranchiseId);

            return players is null ? BadRequest(new MessageDto("<h2 style='color:red'>Error en la carga</h2>")) : Ok(players);
        }
        #endregion Get Protectable Players


        #region Get Selectable Players
        [HttpGet("GetSelectablePlayers")]
        public async Task<IActionResult> GetSelectablePlayers(int FranchiseId)
        {
            if (FranchiseId < 1 || FranchiseId > 32) return BadRequest(new MessageDto("<h2 style='color:red'>FranchiseId no válido</h2>"));

            IList<SelectableDto> players = await _franchiseService.GetSelectablePlayers(FranchiseId);

            return players is null ? BadRequest(new MessageDto("<h2 style='color:red'>Error en la carga</h2>")) : Ok(players);
        }
        #endregion Get Selectable Players
    }
}
