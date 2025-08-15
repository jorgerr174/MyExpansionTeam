using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Team
{
    public partial class TradeErrorViewModel : BaseViewModel
    {
        private readonly TeamService _teamService;

        public TradeErrorViewModel(TeamService teamService)
        {
            _teamService = teamService;
        }
    }
}