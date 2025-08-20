using CommunityToolkit.Mvvm.Input;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Admin
{
    public partial class AdminViewModel(AdminService adminService) : BaseViewModel
    {
        private readonly AdminService _adminService = adminService;

        [RelayCommand] public static async Task GoToImport() => await BaseService.GoToAsync(AppRoutes.Import, null);
        [RelayCommand] public static async Task GoToAssignRoles() => await BaseService.GoToAsync(AppRoutes.AssignRoles, null);
    }
}