using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileApp.Services;

namespace MobileApp.Models.Shared
{
    public abstract partial class BaseViewModel() : ObservableObject
    {
        [ObservableProperty]
        private bool isLoading = false;

        [ObservableProperty]
        private string errorMessage = string.Empty;


        [RelayCommand] public static async Task GoBack() => await BaseService.GoBackAsync(null);

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public bool IsFormEnabled => !IsLoading;

        partial void OnErrorMessageChanged(string value)
        {
            OnPropertyChanged(nameof(HasError));
        }

        partial void OnIsLoadingChanged(bool value)
        {
            OnPropertyChanged(nameof(IsFormEnabled));
        }
    }
}