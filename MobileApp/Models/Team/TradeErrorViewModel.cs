using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Team
{
    public partial class TradeErrorViewModel(TeamService teamService) : BaseViewModel
    {
        private readonly TeamService _teamService = teamService;
    }
}