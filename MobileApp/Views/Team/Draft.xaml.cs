using METCore.DTOs.Player;
using Microsoft.Maui.Controls.Internals;
using MobileApp.Models.Team;
using MobileApp.Services;

namespace MobileApp.Views.Team
{
    public partial class Draft : ContentPage
    {
        private readonly DraftViewModel _viewModel;

        public Draft(TeamService teamService, int teamId)
        {
            InitializeComponent();

            _viewModel = new DraftViewModel(teamService, teamId);
            BindingContext = _viewModel;

            // Subscribe to ViewModel events
            _viewModel.ProspectSelectionRequested += OnProspectSelectionRequested;
            _viewModel.NavigateToTradeRequested += OnNavigateToTradeRequested;
            _viewModel.NavigateBackRequested += OnNavigateBackRequested;
            _viewModel.ShowAlertRequested += OnShowAlertRequested;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.LoadDraftCommand.Execute(null);
        }

        private async Task<ProspectDto> OnProspectSelectionRequested()
        {
            try
            {
                var prospects = _viewModel.AvailableProspects.ToList();
                var modal = new ProspectSelectionModal(prospects);
                return await modal.ShowAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to show prospect selection: {ex.Message}", "OK");
                return null;
            }
        }

        private async Task OnNavigateToTradeRequested()
        {
            try
            {
                // Navigate to Trade view - adjust constructor as needed based on your Trade view
                var tradeView = new Trade(/* pass required parameters */);
                await Navigation.PushAsync(tradeView);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to open trade: {ex.Message}", "OK");
            }
        }

        private void OnNavigateBackRequested()
        {
            try
            {
                // Check if there's unsaved progress
                if (_viewModel.IsDraftActive && _viewModel.Selections.Any())
                {
                    var result = DisplayAlert("Unsaved Progress",
                        "You have unsaved draft progress. Do you want to save before leaving?",
                        "Save", "Discard");

                    if ((bool)result.AsyncState)
                    {
                        _viewModel.SaveDraftCommand.Execute(null);
                    }
                }

                Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Failed to navigate back: {ex.Message}", "OK");
            }
        }

        private async Task OnShowAlertRequested(string title, string message, string cancel)
        {
            await DisplayAlert(title, message, cancel);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Unsubscribe from events to prevent memory leaks
            _viewModel.ProspectSelectionRequested -= OnProspectSelectionRequested;
            _viewModel.NavigateToTradeRequested -= OnNavigateToTradeRequested;
            _viewModel.NavigateBackRequested -= OnNavigateBackRequested;
            _viewModel.ShowAlertRequested -= OnShowAlertRequested;
        }
    }
}