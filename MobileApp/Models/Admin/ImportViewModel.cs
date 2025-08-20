using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METCore.DTOs.Admin;
using METCore.Enums;
using MobileApp.Models.Shared;
using MobileApp.Services;

namespace MobileApp.Models.Admin
{
    public partial class ImportViewModel : BaseViewModel
    {
        private readonly AdminService _adminService;

        public ImportViewModel(AdminService adminService)
        {
            _adminService = adminService;
            InitializeData();
        }

        [ObservableProperty] private string selectedFileName = "No file selected";
        [ObservableProperty] private FileResult? selectedFile;
        [ObservableProperty] private ImportTypeItem? selectedImportType;
        [ObservableProperty] private StatsTypeItem? selectedStatsType;
        [ObservableProperty] private int selectedYear = DateTime.Now.Year - 1;
        [ObservableProperty] private bool showYearSelection = false;
        [ObservableProperty] private bool showStatsTypeSelection = false;
        [ObservableProperty] private string importResult = string.Empty;
        [ObservableProperty] private bool hasImportResult = false;
        [ObservableProperty] private bool hasErrorFile = false;
        [ObservableProperty] private byte[]? errorFileContent;
        [ObservableProperty] private string errorFileName = string.Empty;

        public ObservableCollection<ImportTypeItem> ImportTypes { get; } = [];
        public ObservableCollection<StatsTypeItem> StatsTypes { get; } = [];
        public ObservableCollection<int> AvailableYears { get; } = [];

        private void InitializeData()
        {
            // Import types
            ImportTypes.Add(new ImportTypeItem { Type = Types.ImportEnum.Players, DisplayName = "Players" });
            ImportTypes.Add(new ImportTypeItem { Type = Types.ImportEnum.Stats, DisplayName = "Stats" });
            ImportTypes.Add(new ImportTypeItem { Type = Types.ImportEnum.Contracts, DisplayName = "Contracts" });
            ImportTypes.Add(new ImportTypeItem { Type = Types.ImportEnum.Prospects, DisplayName = "Prospects" });

            // Stats types
            StatsTypes.Add(new StatsTypeItem { Type = Types.StatsEnum.PassStats, DisplayName = "Pass Stats" });
            StatsTypes.Add(new StatsTypeItem { Type = Types.StatsEnum.RecStats, DisplayName = "Rec Stats" });
            StatsTypes.Add(new StatsTypeItem { Type = Types.StatsEnum.RushStats, DisplayName = "Rush Stats" });
            StatsTypes.Add(new StatsTypeItem { Type = Types.StatsEnum.IntStats, DisplayName = "Int Stats" });
            StatsTypes.Add(new StatsTypeItem { Type = Types.StatsEnum.TackleStats, DisplayName = "Tackle Stats" });
            StatsTypes.Add(new StatsTypeItem { Type = Types.StatsEnum.KOStats, DisplayName = "KO Stats" });
            StatsTypes.Add(new StatsTypeItem { Type = Types.StatsEnum.KRStats, DisplayName = "KR Stats" });
            StatsTypes.Add(new StatsTypeItem { Type = Types.StatsEnum.PuntStats, DisplayName = "Punt Stats" });
            StatsTypes.Add(new StatsTypeItem { Type = Types.StatsEnum.PRStats, DisplayName = "PR Stats" });
            StatsTypes.Add(new StatsTypeItem { Type = Types.StatsEnum.FGStats, DisplayName = "FG Stats" });

            // Available years (last 3 years)
            var currentYear = DateTime.Now.Year;
            AvailableYears.Add(currentYear - 1);
            AvailableYears.Add(currentYear - 2);
            AvailableYears.Add(currentYear - 3);

            // Set defaults
            SelectedImportType = ImportTypes.First();
            SelectedStatsType = StatsTypes.First();
        }

        [RelayCommand]
        public async Task SelectFile()
        {
            try
            {
                FilePickerFileType customFileType = new(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "public.comma-separated-values-text" } },
                        { DevicePlatform.Android, new[] { "text/csv" } },
                        { DevicePlatform.WinUI, new[] { ".csv" } },
                        { DevicePlatform.Tizen, new[] { "*/*" } },
                        { DevicePlatform.macOS, new[] { "csv" } },
                    });

                PickOptions options = new()
                {
                    PickerTitle = "Please select a CSV file",
                    FileTypes = customFileType,
                };

                if (await FilePicker.Default.PickAsync(options) is FileResult result)
                {
                    SelectedFile = result;
                    SelectedFileName = result.FileName;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to select file: {ex.Message}", "OK");
            }
        }

        public void OnImportTypeChanged()
        {
            ShowYearSelection = SelectedImportType?.Type == Types.ImportEnum.Stats;
            ShowStatsTypeSelection = SelectedImportType?.Type == Types.ImportEnum.Stats;

            // Reset import result when changing type
            ImportResult = string.Empty;
            HasImportResult = false;
            HasErrorFile = false;
        }

        [RelayCommand]
        public async Task ImportData()
        {
            try
            {
                if (SelectedFile == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Please select a file first", "OK");
                    return;
                }

                if (SelectedImportType == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Please select an import type", "OK");
                    return;
                }

                if (SelectedImportType.Type == Types.ImportEnum.Stats && (SelectedYear < DateTime.Now.Year - 3 || SelectedYear > DateTime.Now.Year - 1))
                {
                    await Shell.Current.DisplayAlert("Error", "Please select a valid year for stats import", "OK");
                    return;
                }

                if (SelectedImportType.Type == Types.ImportEnum.Stats && SelectedStatsType == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Please select a stats type", "OK");
                    return;
                }

                IsLoading = true;
                ErrorMessage = string.Empty;

                // Create ImportDto
                ImportDto importDto = new()
                {
                    Type = SelectedImportType.Type,
                    StatsType = SelectedStatsType?.Type ?? Types.StatsEnum.PassStats,
                    Year = ShowYearSelection ? SelectedYear : null
                };

                // Read file content
                using Stream stream = await SelectedFile.OpenReadAsync();
                using MemoryStream memoryStream = new();
                await stream.CopyToAsync(memoryStream);
                byte[] fileContent = memoryStream.ToArray();

                if (await _adminService.ImportDataAsync(importDto, fileContent, SelectedFile.FileName) is ResultImportDto result)
                {
                    if (result.Content?.Length > 0)
                    {
                        // There were errors
                        ImportResult = $"Import partially completed. Some records from {SelectedFile.FileName} failed to import.";
                        HasErrorFile = true;
                        ErrorFileContent = result.Content;
                        ErrorFileName = $"Error_{SelectedFile.FileName}";
                    }
                    else
                    {
                        // Success
                        ImportResult = $"{SelectedFile.FileName} imported successfully!";
                        HasErrorFile = false;
                    }
                    HasImportResult = true;
                }
                else
                    ErrorMessage = "Import failed. Please check your file and try again.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Import failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task DownloadErrorFile()
        {
            try
            {
                if (ErrorFileContent == null || ErrorFileContent.Length == 0)
                {
                    await Shell.Current.DisplayAlert("Error", "No error file available", "OK");
                    return;
                }

                await SaveErrorFileToDevice();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to save error file: {ex.Message}", "OK");
            }
        }

        private async Task SaveErrorFileToDevice()
        {
            try
            {
                var fileName = $"Error_{ErrorFileName}";
                var content = System.Text.Encoding.UTF8.GetString(ErrorFileContent ?? []);

#if ANDROID
                var documentsPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads)?.AbsolutePath;
                if (documentsPath != null)
                {
                    var filePath = Path.Combine(documentsPath, fileName);
                    await File.WriteAllTextAsync(filePath, content);
                    await Shell.Current.DisplayAlert("Success", $"Error file saved to Downloads: {fileName}", "OK");
                }
#elif IOS
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var filePath = Path.Combine(documentsPath, fileName);
                await File.WriteAllTextAsync(filePath, content);
                await Shell.Current.DisplayAlert("Success", $"Error file saved to Documents: {fileName}", "OK");
#else
                // For other platforms, just show content
                await Shell.Current.DisplayAlert("Error File Content",
                    $"File: {fileName}\n\nContent:\n{content[..Math.Min(content.Length, 1000)]}...",
                    "OK");
#endif
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to save file: {ex.Message}", "OK");
            }
        }
    }

    public class ImportTypeItem
    {
        public Types.ImportEnum Type { get; set; }
        public string DisplayName { get; set; } = string.Empty;
    }

    public class StatsTypeItem
    {
        public Types.StatsEnum Type { get; set; }
        public string DisplayName { get; set; } = string.Empty;
    }
}