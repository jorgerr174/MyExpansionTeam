using MobileApp.Models.Admin;
using static METCore.Enums.Types;

namespace MobileApp.Views.Admin
{
    public partial class AssignRoles : ContentPage
    {
        private AssignRolesViewModel ViewModel => (AssignRolesViewModel)BindingContext;

        public AssignRoles(AssignRolesViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private async void OnPageSizeChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && picker.SelectedItem is PageSizeOption option)
            {
                await ViewModel.ChangePageSize(option);
            }
        }

        private void OnRoleChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker &&
                picker.BindingContext is UserItem user)
            {
                var newRole = (RoleEnum)picker.SelectedIndex;
                AssignRolesViewModel.OnRoleChanged(user, newRole);
            }
        }
    }
}