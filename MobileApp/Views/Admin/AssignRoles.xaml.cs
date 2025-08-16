using MobileApp.Models.Admin;

namespace MobileApp.Views.Admin
{
    public partial class AssignRoles : ContentPage
    {
        public AssignRoles(AssignRolesViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}