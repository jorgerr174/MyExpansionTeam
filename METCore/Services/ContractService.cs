using METCore.Interfaces;
using Microsoft.Extensions.Configuration;


namespace METCore.Services
{
    public class ContractService
    {
        private readonly IConfiguration _configuration;
        private readonly IContractRepository _contractRepository;

        public ContractService(IConfiguration configuration, IContractRepository contractRepository)
        {
            _configuration = configuration;
            _contractRepository = contractRepository;
        }

    }
}
