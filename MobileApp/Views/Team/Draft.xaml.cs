using METCore.DTOs.Player;
using MobileApp.Models.Team;

namespace MobileApp.Views.Team
{
    [QueryProperty(nameof(TeamId), "teamId")]
    public partial class Draft : ContentPage
    {
        public int TeamId { get; set; }

        public Draft(DraftViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;

            // Subscribe to ViewModel events
            viewModel.ProspectSelectionRequested += OnProspectSelectionRequested;
            viewModel.ShowAlertRequested += OnShowAlertRequested;
            viewModel.NavigateBackRequested += OnNavigateBackRequested;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is DraftViewModel vm)
            {
                vm.TeamId = TeamId;
                await vm.LoadDraftCommand.ExecuteAsync(null);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (BindingContext is DraftViewModel viewModel)
            {
                // Unsubscribe from events to prevent memory leaks
                viewModel.ProspectSelectionRequested -= OnProspectSelectionRequested;
                viewModel.ShowAlertRequested -= OnShowAlertRequested;
                viewModel.NavigateBackRequested -= OnNavigateBackRequested;
            }
        }

        private void OnDraftMethodChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value && sender is RadioButton radioButton && BindingContext is DraftViewModel vm)
            {
                vm.SelectedDraftMethod = radioButton.Value?.ToString() ?? "full";
                vm.OnDraftMethodChanged();
            }
        }

        private async Task<ProspectDto?> OnProspectSelectionRequested()
        {
            try
            {
                if (BindingContext is DraftViewModel vm)
                {
                    // Simple prospect selection - show action sheet with top prospects
                    var topProspects = vm.AvailableProspects.Take(10).ToList();
                    if (!topProspects.Any())
                    {
                        await DisplayAlert("No Prospects", "No prospects available", "OK");
                        return null;
                    }

                    var prospectNames = topProspects.Select(p => $"{p.Name} ({p.Position})").ToArray();
                    var cancelOption = "Cancel";

                    var result = await DisplayActionSheet("Select Prospect", cancelOption, null, prospectNames);

                    if (result != cancelOption && result != null)
                    {
                        var selectedIndex = Array.IndexOf(prospectNames, result);
                        if (selectedIndex >= 0 && selectedIndex < topProspects.Count)
                        {
                            return topProspects[selectedIndex];
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to show prospect selection: {ex.Message}", "OK");
                return null;
            }
        }

        private async Task OnShowAlertRequested(string title, string message, string cancel)
        {
            await DisplayAlert(title, message, cancel);
        }

        private async Task OnNavigateBackRequested()
        {
            await Navigation.PopAsync();
        }
    }
}