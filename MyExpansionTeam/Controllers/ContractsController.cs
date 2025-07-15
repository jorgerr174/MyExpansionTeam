using METCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace METAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ContractsController(ContractService contractService) : ControllerBase
    {
        private readonly ContractService _contractService = contractService;

    }
}
