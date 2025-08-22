using MobileApp.Models.Team;

namespace MobileApp.Views.Team
{
    //[QueryProperty(nameof(TeamId), "teamId")]
    public interface ITeamBase<T>
        where T : TeamBaseViewModel
    {
        public int TeamId { get; set; }

        //public ITeamBase(T viewModel);
        //{
        //    BindingContext = viewModel;
        //}

        //protected override asyncvoid OnAppearing()
        //{
        //    base.OnAppearing();
        //    if (BindingContext is ViewModel viewModel)
        //        await viewModel.LoadViewAsync(TeamId);
        //}
    }
}