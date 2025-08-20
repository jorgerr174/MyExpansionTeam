using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using METCore.DTOs.User;
using MobileApp.Models.Shared;
using MobileApp.Services;
using static METCore.Enums.Types;

namespace MobileApp.Models.Account
{
    public partial class ProfileTabViewModel : BaseViewModel, IRecipient<AuthStateChangedMessage>
    {
        private readonly AccountService _accountService;

        public ProfileTabViewModel(AccountService accountService)
        {
            _accountService = accountService;
            WeakReferenceMessenger.Default.Register<AuthStateChangedMessage>(this);
        }

        // Auth state
        [ObservableProperty] private bool isAuthenticated = false;
        public bool IsNotAuthenticated => !IsAuthenticated;

        // Login properties
        [ObservableProperty] private string identifier = string.Empty;
        [ObservableProperty] private string password = string.Empty;
        [ObservableProperty] private string loginErrorMessage = string.Empty;
        public bool HasLoginError => !string.IsNullOrEmpty(LoginErrorMessage);

        // Profile properties
        [ObservableProperty] private string username = string.Empty;
        [ObservableProperty] private string firstName = string.Empty;
        [ObservableProperty] private string lastName = string.Empty;
        [ObservableProperty] private string email = string.Empty;
        [ObservableProperty] private string tlf = string.Empty;
        [ObservableProperty] private bool isAdmin = false;
        [ObservableProperty] private string profileErrorMessage = string.Empty;
        [ObservableProperty] private bool isProfileLoading = false;
        public bool HasProfileError => !string.IsNullOrEmpty(ProfileErrorMessage);

        public bool IsNotLoading => !IsLoading;

        // Message handling
        public void Receive(AuthStateChangedMessage message)
        {
            IsAuthenticated = message.IsAuthenticated;
            OnPropertyChanged(nameof(IsNotAuthenticated));

            if (IsAuthenticated)
            {
                _ = LoadProfile(); // Load profile when authenticated
            }
            else
            {
                ClearProfileData();
                ClearLoginForm();
            }
        }

        [RelayCommand]
        public async Task LoadAuthState()
        {
            IsAuthenticated = await BaseService.IsAuthenticatedAsync();
            OnPropertyChanged(nameof(IsNotAuthenticated));

            if (IsAuthenticated)
            {
                await LoadProfile();
            }
        }

        [RelayCommand]
        public async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Identifier) || string.IsNullOrWhiteSpace(Password))
            {
                LoginErrorMessage = "Please enter both identifier and password";
                return;
            }

            IsLoading = true;
            LoginErrorMessage = string.Empty;
            OnPropertyChanged(nameof(IsNotLoading));

            try
            {
                if (await _accountService.LogInAsync(Identifier, Password))
                {
                    // Send message to update auth state
                    WeakReferenceMessenger.Default.Send(new AuthStateChangedMessage(true));
                }
                else
                {
                    LoginErrorMessage = "Invalid credentials";
                }
            }
            catch (Exception ex)
            {
                LoginErrorMessage = $"Login failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
                OnPropertyChanged(nameof(IsNotLoading));
            }
        }

        private async Task LoadProfile()
        {
            IsProfileLoading = true;
            ProfileErrorMessage = string.Empty;

            try
            {
                if (await _accountService.GetProfileAsync() is UserDto profile)
                {
                    Username = profile.Username;
                    FirstName = profile.FirstName;
                    LastName = profile.LastName;
                    Email = profile.Email;
                    Tlf = profile.Tlf ?? string.Empty;
                    IsAdmin = profile.Role == RoleEnum.Admin;
                }
                else
                {
                    ProfileErrorMessage = "Failed to load profile";
                }
            }
            catch (Exception ex)
            {
                ProfileErrorMessage = $"Load failed: {ex.Message}";
            }
            finally
            {
                IsProfileLoading = false;
            }
        }

        [RelayCommand] public static async Task GoToEditProfile() => await BaseService.GoToAsync(AppRoutes.EditProfile, null);
        [RelayCommand] public static async Task GoToAdmin() => await BaseService.GoToAsync(AppRoutes.Admin, null);

        [RelayCommand]
        public async Task LogOut()
        {
            AccountService.LogOutAsync();
            WeakReferenceMessenger.Default.Send(new AuthStateChangedMessage(false));
        }

        [RelayCommand]
        public async Task DeleteUser()
        {
            bool confirm = await Shell.Current.DisplayAlert(
                "Delete Account",
                "Are you sure you want to delete your account? This action cannot be undone.",
                "Yes", "No");

            if (!confirm) return;

            IsProfileLoading = true;
            ProfileErrorMessage = string.Empty;

            try
            {
                if (await _accountService.DeleteUserAsync())
                {
                    WeakReferenceMessenger.Default.Send(new AuthStateChangedMessage(false));
                }
                else
                    ProfileErrorMessage = "Failed to delete account";
            }
            catch (Exception ex)
            {
                ProfileErrorMessage = $"Delete failed: {ex.Message}";
            }
            finally
            {
                IsProfileLoading = false;
            }
        }

        private void ClearProfileData()
        {
            Username = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            Tlf = string.Empty;
            IsAdmin = false;
            ProfileErrorMessage = string.Empty;
        }

        private void ClearLoginForm()
        {
            Identifier = string.Empty;
            Password = string.Empty;
            LoginErrorMessage = string.Empty;
        }
    }

    // Message for auth state changes
    public record AuthStateChangedMessage(bool IsAuthenticated);
}