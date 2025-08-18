using METCore.DTOs.Admin;
using METCore.DTOs.Shared;
using METCore.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static METCore.Enums.Types;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController(IHttpClientFactory httpClientFactory, IConfiguration configuration) : BaseController(httpClientFactory, configuration)
    {
        #region Index
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        #endregion Index


        #region AssignRoles
        [HttpGet]
        public IActionResult AssignRoles()
        {
            return View();
        }
        #endregion AssignRoles


        #region AssignRole
        [HttpPost]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
        {
            return Json(await GetResult<MessageDto>(await SendRequest(HttpMethod.Post, "Auth", "AssignRole", dto)));
        }
        #endregion AssignRole


        #region UserList
        [HttpPost]
        public async Task<IActionResult> UserList([FromBody] SearchDto dto)
        {
            return Json(await GetResult<SearchResultDto<UserDto>>(await SendRequest(HttpMethod.Get, "Users", "List", dto)));
        }
        #endregion UserList


        #region Import
        [HttpGet]
        public IActionResult Import()
        {
            ImportDto model = new();
            ViewBag.thisYear = DateTime.Now.Year;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Import(ImportDto model)
        {
            if (model.File is null)
            {
                ModelState.Clear();
                ModelState.AddModelError("File", "No file sent.");
                return View(model);
            }
            else if (model.File.Length == 0)
            {
                ModelState.Clear();
                ModelState.AddModelError("File", "File was empty.");
                return View(model);
            }

            if (model.Type is ImportEnum.None || Enum.GetName(model.Type) is null)
            {
                ModelState.Clear();
                ModelState.AddModelError("File", "Choose a type of import.");
                return View(model);
            }
            else if (model.Type is ImportEnum.Stats)
            {
                if (model.Year > DateTime.Now.Year - 1 || model.Year < DateTime.Now.Year - 3)
                {
                    ModelState.Clear();
                    ModelState.AddModelError("Year", "Choose a year (from last three) to import to.");
                    return View(model);
                }
                if (Enum.GetName(model.StatsType) is null)
                {
                    ModelState.Clear();
                    ModelState.AddModelError("StatsType", "Choose a type of stat import.");
                    return View(model);
                }
            }

            string ogFileName = model.File?.FileName ?? string.Empty;
            var response = await SendRequest(HttpMethod.Post, "Import", "Import", model);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("File", (await GetResult<MessageDto>(response)).Message);
                return View(model);
            }

            ResultImportDto result = await GetResult<ResultImportDto>(response);
            if (result.Content is not null && result.Content.Length > 0)
            {
                TempStorage.StoreErrorFile(ogFileName, result.Content);
                ViewBag.errorFile = ogFileName;
            }
            else // not an actual error, import successfull
                ModelState.AddModelError("File", ogFileName + " imported successfuly.");

            return View(model);
        }

        #region DownloadErrorFile
        [HttpGet]
        public IActionResult DownloadErrorFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return BadRequest("File name is required");

            Byte[] fileContent = TempStorage.ErrorFiles.GetValueOrDefault(fileName) ?? [];
            return fileContent == null || fileContent.Length < 1 ? NotFound("Error file not found") : File(fileContent, "text/csv", "F_" + fileName);
        }
        #endregion DownloadErrorFile
        #endregion Import
    }
}