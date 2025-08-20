using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Player;
using METCore.DTOs.Team;
using MobileApp.Services;
using static METCore.Enums.Types;

namespace MobileApp.Models.Team
{
    public partial class DraftResultsViewModel(TeamService teamService) : TeamBaseViewModel
    {
        private readonly TeamService _teamService = teamService;
        private int teamId = 0;
        [ObservableProperty] private bool hasDraftResults = false;

        public ObservableCollection<DraftResultInfo> DraftResults { get; } = [];

        [RelayCommand] public async Task GoToDraft() => await BaseService.GoToAsync(AppRoutes.Draft, new() { ["TeamId"] = teamId });

        public override async Task LoadViewAsync(int teamId)
        {
            try
            {
                IsLoading = true;
                DraftResults.Clear();

                if (await _teamService.GetTeamDraftAsync(teamId) is DraftDto draftData 
                    && draftData.Prospects.Any() && (draftData.Selections?.Any() ?? false) 
                    && draftData.Prospects.Count != draftData.Selections.Count)
                {
                    foreach (KeyValuePair<int, int> selection in draftData.Selections)
                    {
                        if(draftData.Prospects.First(p => p.Id == selection.Value) is not ProspectDto prospect) break;

                        DraftResults.Add(new DraftResultInfo
                        {
                            Round = selection.Key/100,
                            PickNumber = selection.Key%100,
                            OverallPick = DraftPicks.GetPickOverall(selection.Key),
                            SelectedProspect = prospect,
                            IsUserPick = true
                        });
                    }

                    HasDraftResults = DraftResults.Count > 0;
                }
                else
                {
                    HasDraftResults = false;
                }
            }
            catch (Exception ex)
            {
                LoadErrorMessage = $"Failed to load draft results: {ex.Message}";
                HasDraftResults = false;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}