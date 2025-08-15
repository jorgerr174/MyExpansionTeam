using MobileApp.Models.Home;

namespace MobileApp.Views.Home
{
    public partial class Index : ContentPage
    {
        public Index(IndexViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Load data when page appears (equivalent to controller action)
            if (BindingContext is IndexViewModel viewModel)
            {
                await viewModel.LoadDataCommand.ExecuteAsync(null);
            }
        }
    }
}