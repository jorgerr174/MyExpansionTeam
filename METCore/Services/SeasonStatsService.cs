using AutoMapper;
using METCore.Interfaces;
using Microsoft.Extensions.Configuration;


namespace METCore.Services
{
    public class SeasonStatsService(IConfiguration configuration, ISeasonStatsRepository seasonStatsRepository, IPlayerRepository playerRepository, IMapper mapper)
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ISeasonStatsRepository _seasonStatsRepository = seasonStatsRepository;
        private readonly IPlayerRepository _playerRepository = playerRepository;
        private readonly IMapper _mapper = mapper;

    }
}
