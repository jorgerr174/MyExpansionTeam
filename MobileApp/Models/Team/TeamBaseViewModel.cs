using CommunityToolkit.Mvvm.ComponentModel;
using MobileApp.Models.Shared;

namespace MobileApp.Models.Team
{
    public abstract partial class TeamBaseViewModel : BaseViewModel//, IQueryAttributable
    {
        [ObservableProperty] private bool hasLoadError = false;
        [ObservableProperty] private string loadErrorMessage = string.Empty;

        //public void ApplyQueryAttributes(IDictionary<string, object> query)
        //{
        //    if (query.ContainsKey("TeamId") && int.TryParse(query["TeamId"].ToString(), out int teamId))
        //        LoadViewAsync(teamId);
        //    else
        //    {
        //        HasLoadError = true;
        //        LoadErrorMessage = "No team ID provided";
        //    }
        //}

        public abstract Task LoadViewAsync(int teamId);
    }
}