using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Admin;
using METCore.DTOs.Shared;
using MobileApp.Models.Shared;
using MobileApp.Services;
using static METCore.Enums.Types;

namespace MobileApp.Models.Admin
{
    public partial class AssignRolesViewModel : BaseViewModel
    {
        private readonly AdminService _adminService;

        public AssignRolesViewModel(AdminService adminService)
        {
            base.backPath = "..";
            _adminService = adminService;

            // Set default page size to match web app
            SelectedPageSize = PageSizeOptions.First(x => x.Value == 25);
            PageSize = 25;

            _ = LoadUsers(); // Initial load
        }

        [ObservableProperty] private string searchFilter = string.Empty;
        [ObservableProperty] private ObservableCollection<UserItem> users = new();
        [ObservableProperty] private int currentPage = 1;
        [ObservableProperty] private int pageSize = 25;
        [ObservableProperty] private int totalUsers = 0;
        [ObservableProperty] private int totalPages = 0;
        [ObservableProperty] private bool hasUsers = false;
        [ObservableProperty] private bool showPagination = false;
        [ObservableProperty] private string resultsInfo = string.Empty;
        [ObservableProperty] private PageSizeOption selectedPageSize;

        public List<PageSizeOption> PageSizeOptions { get; } = new()
        {
            new PageSizeOption { Value = 10, Display = "10" },
            new PageSizeOption { Value = 25, Display = "25" },
            new PageSizeOption { Value = 50, Display = "50" }
        };

        // Pagination properties
        public string PageInfo => TotalPages > 0 ? $"Page {CurrentPage} of {TotalPages}" : "";
        public int PreviousPage => CurrentPage - 1;
        public int NextPage => CurrentPage + 1;
        public bool CanGoPrevious => CurrentPage > 1;
        public bool CanGoNext => CurrentPage < TotalPages;

        [RelayCommand]
        private async Task Search()
        {
            CurrentPage = 1;
            await LoadUsers();
        }

        [RelayCommand]
        private async Task LoadUsers()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var searchDto = new SearchDto
                {
                    Filter = SearchFilter,
                    Page = CurrentPage,
                    PageSize = PageSize
                };

                var result = await _adminService.GetUsersAsync(searchDto);
                if (result != null)
                {
                    TotalUsers = result.Total;
                    TotalPages = (int)Math.Ceiling((double)TotalUsers / PageSize);

                    Users.Clear();
                    foreach (var user in result.List)
                    {
                        Users.Add(new UserItem
                        {
                            Username = user.Username,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            Phone = user.Tlf ?? string.Empty,
                            IsActive = user.Active ?? true,
                            CurrentRole = user.Role ?? RoleEnum.User,
                            SelectedRole = user.Role ?? RoleEnum.User,
                            IsAdmin = user.Role == RoleEnum.Admin,
                            HasChanges = false
                        });
                    }

                    HasUsers = Users.Count > 0;
                    ShowPagination = TotalPages > 1;
                    UpdateResultsInfo();

                    // Update pagination properties
                    OnPropertyChanged(nameof(PageInfo));
                    OnPropertyChanged(nameof(CanGoPrevious));
                    OnPropertyChanged(nameof(CanGoNext));
                }
                else
                {
                    ErrorMessage = "Failed to load users";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading users: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task GoToPage(int page)
        {
            if (page >= 1 && page <= TotalPages && page != CurrentPage)
            {
                CurrentPage = page;
                await LoadUsers();
            }
        }

        // Called when page size changes - matches web app behavior
        public async Task ChangePageSize(PageSizeOption option)
        {
            if (option.Value != PageSize)
            {
                PageSize = option.Value;
                SelectedPageSize = option;
                CurrentPage = 1; // Reset to first page like web app
                await LoadUsers();
            }
        }

        [RelayCommand]
        private async Task SaveRole(UserItem user)
        {
            if (user.CurrentRole == user.SelectedRole)
                return;

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var assignRoleDto = new AssignRoleDto(user.Username, user.SelectedRole);
                bool success = await _adminService.AssignRoleAsync(assignRoleDto);

                if (success)
                {
                    user.CurrentRole = user.SelectedRole;
                    user.IsAdmin = user.SelectedRole == RoleEnum.Admin;
                    user.HasChanges = false;

                    // Update UI properties
                    user.OnPropertyChanged(nameof(user.CanChangeRole));
                    user.OnPropertyChanged(nameof(user.SelectedRoleIndex));
                }
                else
                {
                    user.SelectedRole = user.CurrentRole; // Revert selection
                    user.OnPropertyChanged(nameof(user.SelectedRoleIndex));
                    ErrorMessage = "Failed to update user role";
                }
            }
            catch (Exception ex)
            {
                user.SelectedRole = user.CurrentRole; // Revert selection
                user.OnPropertyChanged(nameof(user.SelectedRoleIndex));
                ErrorMessage = $"Error updating role: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void OnRoleChanged(UserItem user, RoleEnum newRole)
        {
            user.SelectedRole = newRole;
            user.HasChanges = user.CurrentRole != user.SelectedRole;
        }

        private void UpdateResultsInfo()
        {
            if (TotalUsers == 0)
            {
                ResultsInfo = "No users found";
                return;
            }

            int startItem = (CurrentPage - 1) * PageSize + 1;
            int endItem = Math.Min(CurrentPage * PageSize, TotalUsers);
            ResultsInfo = $"Showing {startItem}-{endItem} of {TotalUsers} users";
        }
    }

    public partial class UserItem : ObservableObject
    {
        [ObservableProperty] private string username = string.Empty;
        [ObservableProperty] private string firstName = string.Empty;
        [ObservableProperty] private string lastName = string.Empty;
        [ObservableProperty] private string email = string.Empty;
        [ObservableProperty] private string phone = string.Empty;
        [ObservableProperty] private bool isActive = true;
        [ObservableProperty] private RoleEnum currentRole = RoleEnum.User;
        [ObservableProperty] private RoleEnum selectedRole = RoleEnum.User;
        [ObservableProperty] private bool isAdmin = false;
        [ObservableProperty] private bool hasChanges = false;

        public string FullName => $"{FirstName} {LastName}";

        public int SelectedRoleIndex
        {
            get => (int)SelectedRole;
            set
            {
                if (Enum.IsDefined(typeof(RoleEnum), value))
                {
                    var newRole = (RoleEnum)value;
                    if (SelectedRole != newRole)
                    {
                        SelectedRole = newRole;
                        HasChanges = CurrentRole != SelectedRole;
                    }
                }
            }
        }

        // Admin users cannot have their role changed
        public bool CanChangeRole => !IsAdmin;
    }

    public class PageSizeOption
    {
        public int Value { get; set; }
        public string Display { get; set; } = string.Empty;
    }
}