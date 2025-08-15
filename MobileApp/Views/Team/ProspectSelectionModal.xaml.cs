using METCore.DTOs.Player;
using MobileApp.Models.Team;

namespace MobileApp.Views.Team
{
    public partial class ProspectSelectionModal : ContentPage
    {
        private readonly TaskCompletionSource<ProspectDto> _taskCompletionSource;
        private readonly ProspectSelectionViewModel _viewModel;

        public ProspectSelectionModal(List<ProspectDto> prospects)
        {
            InitializeComponent();

            _taskCompletionSource = new TaskCompletionSource<ProspectDto>();
            _viewModel = new ProspectSelectionViewModel(prospects);
            BindingContext = _viewModel;

            // Subscribe to ViewModel events
            _viewModel.ProspectSelected += OnProspectSelected;
            _viewModel.SelectionCancelled += OnSelectionCancelled;
            _viewModel.ShowProspectDetailsRequested += OnShowProspectDetailsRequested;
        }

        public async Task<ProspectDto> ShowAsync()
        {
            await Navigation.PushModalAsync(this);
            return await _taskCompletionSource.Task;
        }

        private async Task OnProspectSelected(ProspectDto prospect)
        {
            _taskCompletionSource.SetResult(prospect);
            await Navigation.PopModalAsync();
        }

        private async Task OnSelectionCancelled()
        {
            _taskCompletionSource.SetResult(null);
            await Navigation.PopModalAsync();
        }

        private async Task OnShowProspectDetailsRequested(ProspectDto prospect)
        {
            var details = _viewModel.GetProspectDetails(prospect);
            await DisplayAlert($"{prospect.Name} - Details", details, "OK");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Unsubscribe from events to prevent memory leaks
            _viewModel.ProspectSelected -= OnProspectSelected;
            _viewModel.SelectionCancelled -= OnSelectionCancelled;
            _viewModel.ShowProspectDetailsRequested -= OnShowProspectDetailsRequested;
        }
    }
}