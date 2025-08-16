using MobileApp.Models.Admin;

namespace MobileApp.Views.Admin
{
    public partial class Import : ContentPage
    {
        public Import(ImportViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is ImportViewModel vm)
            {
                vm.OnImportTypeChanged();
            }
        }
    }
}