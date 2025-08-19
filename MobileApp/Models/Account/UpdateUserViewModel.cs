using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Account
{
    public partial class UpdateUserViewModel : BaseViewModel
    {
        private readonly AccountService _accountService;

        public UpdateUserViewModel(AccountService accountService)
        {
            _accountService = accountService;
        }

        // Current user data (for display)
        [ObservableProperty] private string currentFirstName = string.Empty;
        [ObservableProperty] private string currentLastName = string.Empty;
        [ObservableProperty] private string currentEmail = string.Empty;
        [ObservableProperty] private string currentTlf = string.Empty;

        // New user data (for update)
        [ObservableProperty] private string newFirstName = string.Empty;
        [ObservableProperty] private string newLastName = string.Empty;
        [ObservableProperty] private string newEmail = string.Empty;
        [ObservableProperty] private string newTlf = string.Empty;

        [RelayCommand]
        public async Task LoadProfile()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var profile = await _accountService.GetProfileAsync();
                if (profile != null)
                {
                    CurrentFirstName = profile.FirstName;
                    CurrentLastName = profile.LastName;
                    CurrentEmail = profile.Email;
                    CurrentTlf = profile.Tlf;
                }
                else
                {
                    ErrorMessage = "Failed to load profile";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Load failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task UpdateUser()
        {
            // Only update if at least one field has new data
            if (string.IsNullOrWhiteSpace(NewFirstName) &&
                string.IsNullOrWhiteSpace(NewLastName) &&
                string.IsNullOrWhiteSpace(NewEmail) &&
                string.IsNullOrWhiteSpace(NewTlf))
            {
                ErrorMessage = "Please enter at least one field to update";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                // Use new values if provided, otherwise keep current values
                string firstName = string.IsNullOrWhiteSpace(NewFirstName) ? CurrentFirstName : NewFirstName;
                string lastName = string.IsNullOrWhiteSpace(NewLastName) ? CurrentLastName : NewLastName;
                string email = string.IsNullOrWhiteSpace(NewEmail) ? CurrentEmail : NewEmail;
                string tlf = string.IsNullOrWhiteSpace(NewTlf) ? CurrentTlf : NewTlf;

                bool success = await _accountService.UpdateUserAsync(firstName, lastName, email, tlf);
                if (success)
                {
                    await _accountService.GoToAsync(AppRoutes.Profile);
                }
                else
                {
                    ErrorMessage = "Failed to update profile";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Update failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task GoBack()
        {
            await _accountService.GoToAsync(AppRoutes.Profile);
        }
    }
}