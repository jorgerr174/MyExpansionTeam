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


        #region Create
        /// <summary>Crea un jugador por cada fila con valores válidos en un excel.</summary>
        /// <param name="dto">Archivo de donde se obtienen los datos.</param>
        /// <returns>Opciones(números también como string):
        /// No file uploaded (Archivo nulo).
        /// File was empty (Archivo vacío).
        /// NoPlayers (No se obtuvo ningún Player del fichero).
        /// Error (No se guardó ningún Player en la BBDD).
        /// Nada (Todo bien, también como string).
        /// </returns>
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
        #endregion Create
    }
}