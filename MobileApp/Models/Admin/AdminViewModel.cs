using CommunityToolkit.Mvvm.Input;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Admin
{
    public partial class AdminViewModel(AdminService adminService) : BaseViewModel
    {
        private readonly AdminService _adminService = adminService;

        [RelayCommand] public async Task GoToImport() => await _adminService.GoToAsync(AppRoutes.Import, null);
        [RelayCommand] public async Task GoToAssignRoles() => await _adminService.GoToAsync(AppRoutes.AssignRoles, null);
        [RelayCommand] public async Task GoBack() => await _adminService.GoBackAsync(null);
    }
}