using AutoMapper;
using METCore.DTOs.Player;
using METCore.Interfaces;
using Microsoft.Extensions.Configuration;


namespace METCore.Services
{
    public class PlayerService(IConfiguration configuration, IPlayerRepository playerRepository, IMapper mapper)
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IPlayerRepository _playerRepository = playerRepository;
        private readonly IMapper _mapper = mapper;


        #region Get
        /// <summary> Obtener los TeamDtos con los valores de los Teams del User logeado.</summary>
        /// <returns>Opciones:
        /// Username (no se encontró ningún User para username).
        /// IEnumerable<TeamDto>? (Con los valores de los Teams encontrados).
        /// </returns>
        public async Task<IList<ProspectDto>> GetDraftProspects(int year)
        {
            return [.. _mapper.Map<IList<ProspectDto>>(await _playerRepository.GetDraftProspects(year)).OrderBy(p => p.Consensus)];
        }
        #endregion Get
    }
}
