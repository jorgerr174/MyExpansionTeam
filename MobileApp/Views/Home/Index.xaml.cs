using METCore.DTOs.Team;
using MobileApp.Services;

namespace MobileApp.Views.Home;

public partial class Index : ContentPage
{
    private readonly HomeService _service;

    public Index(HomeService service)
    {
        InitializeComponent();
        _service = service;
    }

    // Exact equivalent of HomeController.Index() execution
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _service.IndexAsync(this);
    }

    public void IsNotAuthenticated()
    {
        AuthenticatedContent.IsVisible = false;
        NotAuthenticatedContent.IsVisible = true;
    }

    public void SetTeams(IEnumerable<TeamInfoDto> teams)
    {
        TeamsCollectionView.ItemsSource = teams;
    }


    // Event handlers for buttons (equivalent of asp-action links)
    private async void OnMyTeamsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("Team/MyTeams");
    }

    private async void OnCreateTeamClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("Team/Create");
    }

    private async void OnAdminClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("Admin/Index");
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("Account/LogIn");
    }
}