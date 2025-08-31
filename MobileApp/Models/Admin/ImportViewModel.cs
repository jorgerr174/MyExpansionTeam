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

        [ObservableProperty] private string selectedFileName = "No hay archivo seleccionado";
        [ObservableProperty] private FileResult? selectedFile;
        [ObservableProperty] private ImportTypeItem? selectedImportType;
        [ObservableProperty] private StatsTypeItem? selectedStatsType;
        [ObservableProperty] private int selectedYear = DateTime.Now.Year - 1;
        [ObservableProperty] private bool isImportStatsSelected = false;
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
            ImportTypes.Add(new ImportTypeItem { Type = Types.ImportEnum.Players, DisplayName = "Jugadores" });
            ImportTypes.Add(new ImportTypeItem { Type = Types.ImportEnum.Stats, DisplayName = "Estadísticas" });
            ImportTypes.Add(new ImportTypeItem { Type = Types.ImportEnum.Contracts, DisplayName = "Contratos" });
            ImportTypes.Add(new ImportTypeItem { Type = Types.ImportEnum.Prospects, DisplayName = "Prospectos" });

            // Stats types
            StatsTypes.Add(new StatsTypeItem { Type = Types.StatsEnum.PassStats, DisplayName = "Pasador" });
            StatsTypes.Add(new StatsTypeItem { Type = Types.StatsEnum.RecStats, DisplayName = "Receptor" });
            StatsTypes.Add(new StatsTypeItem { Type = Types.StatsEnum.RushStats, DisplayName = "Corredor" });
            StatsTypes.Add(new StatsTypeItem { Type = Types.StatsEnum.IntStats, DisplayName = "Intercepciones" });
            StatsTypes.Add(new StatsTypeItem { Type = Types.StatsEnum.TackleStats, DisplayName = "Placajes" });
            StatsTypes.Add(new StatsTypeItem { Type = Types.StatsEnum.KOStats, DisplayName = "Saques" });
            StatsTypes.Add(new StatsTypeItem { Type = Types.StatsEnum.KRStats, DisplayName = "Retornos de saque" });
            StatsTypes.Add(new StatsTypeItem { Type = Types.StatsEnum.PuntStats, DisplayName = "Pateos" });
            StatsTypes.Add(new StatsTypeItem { Type = Types.StatsEnum.PRStats, DisplayName = "Retornos de pateo" });
            StatsTypes.Add(new StatsTypeItem { Type = Types.StatsEnum.FGStats, DisplayName = "Goles de campo" });

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
                        { DevicePlatform.iOS, new[] { "public.comma-separated-values-text", ".csv" } },
                        { DevicePlatform.Android, new[] { "text/csv", "text/plain", "application/vnd.ms-excel", ".csv" } },
                        { DevicePlatform.WinUI, new[] { ".csv" } },
                        { DevicePlatform.Tizen, new[] { "*/*" } },
                        { DevicePlatform.macOS, new[] { "csv" } },
                    });

                PickOptions options = new()
                {
                    PickerTitle = "Por favor, seleccione un archivo CSV",
                    FileTypes = customFileType,
                };

                if (await FilePicker.Default.PickAsync(null) is FileResult result)
                {
                    SelectedFile = result;
                    SelectedFileName = result.FileName;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al elegir archivo: {ex.Message}", "OK");
            }
        }

        partial void OnSelectedImportTypeChanged(ImportTypeItem? value)
        {
            IsImportStatsSelected = value?.Type == Types.ImportEnum.Stats;
            ImportResult = string.Empty;
            HasImportResult = false;
        }

        [RelayCommand]
        public async Task ImportData()
        {
            try
            {
                if (SelectedFile == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Por favor, seleccione un archivo", "OK");
                    return;
                }

                if (SelectedImportType == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Por favor, seleccione un tipo de importación", "OK");
                    return;
                }

                if (SelectedImportType.Type == Types.ImportEnum.Stats && (SelectedYear < DateTime.Now.Year - 3 || SelectedYear > DateTime.Now.Year - 1))
                {
                    await Shell.Current.DisplayAlert("Error", "Por favor, seleccione una temporada válida (3 últimas)", "OK");
                    return;
                }

                if (SelectedImportType.Type == Types.ImportEnum.Stats && SelectedStatsType == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Por favor, seleccione un tipo de estadísticas", "OK");
                    return;
                }

                IsLoading = true;
                ErrorMessage = string.Empty;

                ImportDto importDto = new();
                importDto.Type = SelectedImportType.Type;
                if (IsImportStatsSelected)
                {
                    importDto.StatsType = SelectedStatsType.Type;
                    importDto.Year = SelectedYear;
                }
                ;

                using Stream stream = await SelectedFile.OpenReadAsync();
                using MemoryStream memoryStream = new();
                await stream.CopyToAsync(memoryStream);
                byte[] fileContent = memoryStream.ToArray();

                if (await _adminService.ImportDataAsync(importDto, fileContent, SelectedFile.FileName) is ResultImportDto result)
                {
                    if (result.Content?.Length > 0)
                    {
                        // There were errors
                        ImportResult = $"Importación parcialmente completa. Algunos registros de {SelectedFile.FileName} no pudieron ser importados.";
                        HasErrorFile = true;
                        ErrorFileContent = result.Content;
                        ErrorFileName = $"Error_{SelectedFile.FileName}";
                    }
                    else
                    {
                        // Success
                        ImportResult = $"{SelectedFile.FileName} importado exitosamente!";
                        HasErrorFile = false;
                    }
                    HasImportResult = true;
                }
                else
                    ErrorMessage = "Importación fallida. Por favor, revise su archivo y pruebe de nuevo.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Importación fallida: {ex.Message}";
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
                    await Shell.Current.DisplayAlert("Error", "No hay fallo de errores disponible", "OK");
                    return;
                }

                await SaveErrorFileToDevice();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al guardar el archivo de errores: {ex.Message}", "OK");
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
                    await Shell.Current.DisplayAlert("Éxito", $"Archivo de errores guardado en Descargas: {fileName}", "OK");
                }
#elif IOS
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var filePath = Path.Combine(documentsPath, fileName);
                await File.WriteAllTextAsync(filePath, content);
                await Shell.Current.DisplayAlert("Éxito", $"Archivo de errores guardado en Documentos: {fileName}", "OK");
#else
                await Shell.Current.DisplayAlert("Error File Content",
                    $"File: {fileName}\n\nContent:\n{content[..Math.Min(content.Length, 1000)]}...",
                    "OK");
#endif
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al guardar archivo: {ex.Message}", "OK");
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