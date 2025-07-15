using METCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace METAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class SeasonStatsController(SeasonStatsService seasonStatsService) : ControllerBase
    {
        private readonly SeasonStatsService _seasonStatsService = seasonStatsService;


        /*#region Create
        /// <summary>Crea un jugador por cada fila con valores válidos en un excel.</summary>
        /// <param name="dto">Archivo de donde se obtienen los datos.</param>
        /// <returns>Opciones(números también como string):
        /// No file uploaded (Archivo nulo).
        /// File was empty (Archivo vacío).
        /// NoSeasonStatss (No se obtuvo ningún SeasonStats del fichero).
        /// Error (No se guardó ningún SeasonStats en la BBDD).
        /// Nada (Todo bien, también como string).
        /// </returns>
        [HttpPost("ImportStats")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ImportStats([FromForm] ImportDto dto)
        {
            if (dto.File == null) return BadRequest(new MessageDto { Message = "No file uploaded." });
            else if (dto.File.Length < 1) return BadRequest(new MessageDto { Message = "File was empty." });

            if (int.TryParse(dto.File.FileName[0..4], out int season) || season < DateTime.Now.Year - 3 || season > DateTime.Now.Year - 1) 
                return BadRequest(new MessageDto { Message = "The file must be named like: YYYY_StatType.cvs." });

            Byte[] result;
            #region FileNameSwitch
            switch (Path.GetFileNameWithoutExtension(dto.File.FileName))
            {
                case "field-goals": result = await _seasonStatsService.ImportStats<ImportFGStatsDto, FGStats>(dto, PositionEnum.K, season); break;
                case "interceptions": result = await _seasonStatsService.ImportStats<ImportIntStatsDto, IntStats>(dto, PositionEnum.DB, season); break;
                case "kickoffs": result = await _seasonStatsService.ImportStats<ImportKOStatsDto, KOStats>(dto, PositionEnum.K, season); break;
                case "punts": result = await _seasonStatsService.ImportStats<ImportPuntStatsDto, PuntStats>(dto, PositionEnum.P, season); break;
                case "kickoff-returns": result = await _seasonStatsService.ImportStats<ImportKRStatsDto, KRStats>(dto, PositionEnum.ATH, season); break;
                case "punt-returns": result = await _seasonStatsService.ImportStats<ImportPRStatsDto, PRStats>(dto, PositionEnum.ATH, season); break;
                case "passing": result = await _seasonStatsService.ImportStats<ImportPassStatsDto, PassStats>(dto, PositionEnum.QB, season); break;
                case "receiving": result = await _seasonStatsService.ImportStats<ImportRecStatsDto, RecStats>(dto, PositionEnum.WR, season); break;
                case "rushing": result = await _seasonStatsService.ImportStats<ImportRushStatsDto, RushStats>(dto, PositionEnum.RB, season); break;
                case "tackles": result = await _seasonStatsService.ImportStats<ImportTackleStatsDto, TackleStats>(dto, PositionEnum.MLB, season); break;
                default: return BadRequest(new MessageDto { Message = "FileName not valid." });
            }
            #endregion FileNameSwitch
            return Ok(new ResultImportDto { Content = result, Type = dto.Type });
        }
        #endregion Create*/
    }
}