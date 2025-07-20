using AutoMapper;
using METCore.DTOs.Player;
using METCore.Interfaces;
using METCore.Models;
using Microsoft.Extensions.Configuration;


namespace METCore.Services
{
    public class FranchiseService(IConfiguration configuration, IFranchiseRepository franchiseRepository, IMapper mapper)
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IFranchiseRepository _franchiseRepository = franchiseRepository;
        private readonly IMapper _mapper = mapper;


        #region Get Protectable Players
        public async Task<IList<ProtectableDto>> GetProtectablePlayers(int FranchiseId)
        {
            Franchise? franchise = await _franchiseRepository.GetTById(FranchiseId);

            if (franchise is null) return [];

            IList<ProtectableDto> aux = _mapper.Map<IList<ProtectableDto>>(franchise.PlayersToProtect);
            aux.First().DefaultProtected = true;
            aux.ElementAt(2).DefaultProtected = true;
            aux.ElementAt(3).DefaultProtected = true;

            return aux;
        }
        #endregion Get Protectable Players

        #region Get Selectable Players
        public async Task<IList<SelectableDto>> GetSelectablePlayers(int FranchiseId)
        {
            Franchise? franchise = await _franchiseRepository.GetTById(FranchiseId);

            return _mapper.Map<IList<SelectableDto>>(franchise.PlayersByPosition);
        }
        #endregion Get Selectable Players
    }
}
