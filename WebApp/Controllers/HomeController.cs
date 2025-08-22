using System.Diagnostics;
using METCore.DTOs.Team;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Shared;

namespace WebApp.Controllers
{
    public class HomeController(IHttpClientFactory httpClientFactory, IConfiguration configuration) : BaseController(httpClientFactory, configuration)
    {
        #region Index
        public async Task<IActionResult> Index()
        {
            if (User.Identity == null)
                return View();

            HttpResponseMessage response = await SendRequest(HttpMethod.Get, "Teams", "List");
            return !response.IsSuccessStatusCode ? View() : View(await GetResult<IEnumerable<TeamInfoDto>>(response));
        }
        #endregion Index


        #region Privacy
        public IActionResult Privacy()
        {
            return View();
        }
        #endregion Privacy


        #region Error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion Error
    }
}
