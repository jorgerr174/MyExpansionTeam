using CommunityToolkit.Mvvm.Input;
using MobileApp.Models.Shared;

namespace MobileApp.Models.Admin
{
    public partial class AdminViewModel : BaseViewModel
    {
        public AdminViewModel()
        {
            base.backPath = "..";
        }

        [RelayCommand]
        public async Task GoToImport()
        {
            await Shell.Current.GoToAsync("Import");
        }

        [RelayCommand]
        public async Task GoToAssignRoles()
        {
            await Shell.Current.GoToAsync("AssignRoles");
        }
    }
}