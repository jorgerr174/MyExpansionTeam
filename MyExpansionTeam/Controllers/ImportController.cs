using METCore.DTOs.Admin;
using METCore.DTOs.Shared;
using METCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static METCore.Enums.Types;

namespace METAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ImportController(ImportService importService) : ControllerBase
    {
        private readonly ImportService _importService = importService;


        #region Import
        /// <summary>
        /// Crea un jugador por cada fila con valores válidos en un excel.
        /// </summary>
        /// <param name="dto">Archivo de donde se obtienen los datos (ImportDto)</param>
        /// <returns>Resultado del proceso de importación</returns>
        [HttpPost("Import")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Import([FromForm] ImportDto dto)
        {
            if (dto.File == null) return BadRequest(new MessageDto("No file uploaded."));
            else if (dto.File == null) return BadRequest(new MessageDto("File was empty."));

            if (dto.Type is ImportEnum.None || Enum.GetName(dto.Type) is null)
                return BadRequest(new MessageDto("Import type not valid."));
            else if (dto.Type is ImportEnum.Stats)
            {
                if (dto.Year > DateTime.Now.Year - 1 || dto.Year < DateTime.Now.Year - 3)
                    return BadRequest(new MessageDto("Import year not valid."));
                if (dto.Type is ImportEnum.Stats && Enum.GetName(dto.StatsType) is null)
                    return BadRequest(new MessageDto("Stats import type not valid."));
            }

            Byte[] result = await _importService.Import(dto);

            return Ok(new ResultImportDto { Content = result, Type = dto.Type });
        }
        #endregion Import
    }
}