using METCore.DTOs.Player;
using METCore.DTOs.Shared;
using METCore.DTOs.Team;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class TeamController(IHttpClientFactory httpClientFactory, IConfiguration configuration) : BaseController(httpClientFactory, configuration)
    {
        #region Privates
        private async Task<TeamDto?> GetTeam(int Id)
        {
            HttpResponseMessage response = await SendRequest(HttpMethod.Get, "Teams", "Team", new string[] { $"TeamId={Id}" });
            return !response.IsSuccessStatusCode ? null : await GetResult<TeamDto>(response);
        }
        private async Task<TeamInfoDto?> GetTeamInfo(int Id)
        {
            HttpResponseMessage response = await SendRequest(HttpMethod.Get, "Teams", "TeamInfo", new string[] { $"TeamId={Id}" });
            return !response.IsSuccessStatusCode ? null : await GetResult<TeamInfoDto>(response);
        }
        private async Task<TeamBasicInfoDto?> GetBasicTeam(int Id)
        {
            HttpResponseMessage response = await SendRequest(HttpMethod.Get, "Teams", "TeamBasicInfo", new string[] { $"TeamId={Id}" });
            return !response.IsSuccessStatusCode ? null : await GetResult<TeamBasicInfoDto>(response);
        }

        private async Task<IList<TeamInfoDto>?> GetTeamList(bool? mine = false)
        {
            HttpResponseMessage response;
            if (User?.Identity is null) return null;

            if (mine.HasValue && mine.Value) response = await SendRequest(HttpMethod.Get, "Teams", "MyTeams");
            else return null;
            //esperando a que piense lo de los parámetros
            //response = await SendRequest(HttpMethod.Get, "Teams", "List");

            return !response.IsSuccessStatusCode ? null : await GetResult<IList<TeamInfoDto>>(response);
        }
        #endregion


        #region Create
        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            return View(new TeamBasicInfoDto(User.Identity.Name));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(TeamBasicInfoDto model)
        {
            if (!ModelState.IsValid) return View(model);

            HttpResponseMessage response = await SendRequest(HttpMethod.Post, "Teams", "Create", model);

            if (response.IsSuccessStatusCode) return RedirectToAction("RosterSettings", await GetResult<TeamInfoDto>(response));

            MessageDto result = await GetResult<MessageDto>(response);
            if (result.Message.Equals("Username")) return RedirectToAction("LogOut", new RouteValueDictionary(new { ReturnUrl = Url.Action("Create", "Team") }));

            if (result.Message.Equals("Error")) ModelState.AddModelError("Create", "No se pudo crear el equipo.");
            else ModelState.AddModelError(result.Message, result.Message + " no puede estar vacío.");
            return View(model);
        }
        #endregion


        #region Details
        [HttpGet]
        [Route("[Controller]/{Id}", Name = "TeamDetails", Order = 1)]
        public async Task<IActionResult> Details(int Id)
        {
            return Id < 1
                ? User?.Identity == null ? RedirectToAction("Index", "Home") : RedirectToAction("MyTeams")
                : View("Details", await this.GetTeam(Id));
        }
        #endregion


        #region List     
        /*[HttpGet]
        [Authorize]
        public async Task<IActionResult> List()
        {
            return await this.GetTeamList();
        }*/

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyTeams()
        {
            return View(await this.GetTeamList(true));
        }

        [HttpGet]
        public async Task<Object> GetProtectablePlayers(int FranchiseId)
        {
            HttpResponseMessage response = await SendRequest(HttpMethod.Get, "Franchises", "GetProtectablePlayers", new string[] { $"FranchiseId={FranchiseId}" });

            return response.IsSuccessStatusCode ? await GetResult<IList<ProtectableDto>>(response) : await GetResult<MessageDto>(response);
        }

        [HttpGet]
        public async Task<Object> GetSelectablePlayers(int FranchiseId)
        {
            HttpResponseMessage response = await SendRequest(HttpMethod.Get, "Franchises", "GetSelectablePlayers", new string[] { $"FranchiseId={FranchiseId}" });

            return response.IsSuccessStatusCode ? await GetResult<IList<SelectableDto>>(response) : await GetResult<MessageDto>(response);
        }
        #endregion


        #region Edit
        [HttpGet]
        [Authorize]
        [Route("[Controller]/Edit/{Id}", Name = "EditTeam")]
        public async Task<IActionResult> Edit(int Id)
        {
            return Id < 1 ? RedirectToAction("MyTeams") : View(await this.GetBasicTeam(Id));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditTeam(TeamBasicInfoDto model)
        {
            HttpResponseMessage response = await SendRequest(HttpMethod.Post, "Teams", "UpdateTeam", model);
            if (response.IsSuccessStatusCode) return RedirectToAction("Details", model.Id);

            MessageDto result = await GetResult<MessageDto>(response);
            if (result.Message.Equals("Username")) return RedirectToAction("LogOut", new RouteValueDictionary(new { ReturnUrl = Url.Action("MyTeams", "Team") }));
            else ModelState.AddModelError("Edit", "No se pudieron guardar los cambios.");

            return View(model);
        }
        #endregion


        #region EditRosterSettings
        [HttpGet]
        [Authorize]
        [Route("[Controller]/RosterSettings/{Id}", Name = "EditSettings")]
        public async Task<IActionResult> RosterSettings(int Id)
        {
            return Id > 0 && await this.GetTeamInfo(Id) is TeamInfoDto dto
                ? View(dto) : RedirectToAction("MyTeams");
        }

        [HttpGet]
        [Authorize]
        public IActionResult RosterSettings(TeamInfoDto dto)
        {
            return View(dto);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SaveRosterSettings(TeamInfoDto dto)
        {
            if (!ModelState.IsValid)
                return View("RosterSettings", dto);

            if (dto.RosterSettingsProtectedPlayersIds.Count != 0 &&
                    dto.RosterSettingsProtectedPlayersIds.Count != 32 * dto.RosterSettingsProtectedPerTeam)
            {
                ModelState.AddModelError("ProtectedPlayersIds", String.Format("Number of protected players not valid. Selected: {0}. Max: {1}.",
                    dto.RosterSettingsProtectedPlayersIds.Count, 32 * dto.RosterSettingsProtectedPerTeam));
                return View("RosterSettings", dto);
            }

            HttpResponseMessage response = await SendRequest(HttpMethod.Post, "Teams", "UpdateRosterSettings", dto);
            if (response.IsSuccessStatusCode) return RedirectToAction("EditRoster", new { dto.Id });

            ResultDto<TeamInfoDto> result = await GetResult<ResultDto<TeamInfoDto>>(response);
            if (result.Message.Equals("Username")) return RedirectToAction("LogOut", new RouteValueDictionary(new { ReturnUrl = Url.Action("MyTeams", "Team") }));
            else if (result.Message.Equals("ProtectedPlayersIds")) 
            {
                dto.RosterSettingsProtectedPlayersIds = result.Value.RosterSettingsProtectedPlayersIds;
                ModelState.AddModelError("", "Error al guardar los jugadores protegidos.");
            }
            else ModelState.AddModelError("", "No se pudieron guardar los cambios.");

            return View("RosterSettings", dto);
        }
        #endregion EditRosterSettings


        #region Roster
        [HttpGet]
        [Route("[Controller]/EditRoster/{Id}", Name = "EditRoster")]
        [Authorize]
        public async Task<IActionResult> EditRoster(int Id, bool? unsuccessfulDraft = false)
        {
            if (Id < 0) return RedirectToAction("MyTeams");

            TeamDto? dto = await this.GetTeam(Id);
            if (dto is null) return RedirectToAction("MyTeams");

            if (unsuccessfulDraft.HasValue && unsuccessfulDraft.Value) ModelState.AddModelError("", "Error durante el guardado del draft, no se pudo registrar.");
            return View("Roster", dto);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SaveRoster(TeamDto dto, bool? next = null) // null-> return, false-> MyTeams, true->Draft
        {
            //if (!ModelState.Any(e => e.Key != "next" ? false : e.Value.ValidationState == Va).Any().IsValid)
            if (!ModelState.IsValid)
                return View("Roster", dto);

            HttpResponseMessage response = await SendRequest(HttpMethod.Post, "Teams", "UpdateRoster", dto);
            if (response.IsSuccessStatusCode)
                return !next.HasValue ? RedirectToAction("Roster", dto.Id) : !next.Value ? RedirectToAction("Draft", new { dto.Id }) : RedirectToAction("MyTeams");

            MessageDto result = await GetResult<MessageDto>(response);
            if (result.Message.Equals("User")) return RedirectToAction("MyTeams");
            if (result.Message.Equals("Username")) return RedirectToAction("LogOut", new RouteValueDictionary(new { ReturnUrl = Url.Action("MyTeams", "Team") }));
            else ModelState.AddModelError("", "No se pudieron guardar los cambios.");

            return View("Roster", dto);
        }
        #endregion Roster


        #region Draft
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Draft(int Id)
        {
            return Id > 0 && await this.GetTeamDraft(Id) is DraftDto dto
                ? View(dto) : RedirectToAction("MyTeams");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Draft(DraftDto dto)
        {
            HttpResponseMessage response = await SendRequest(HttpMethod.Post, "Teams", "SaveDraft", dto);
            return RedirectToAction("EditRoster", new { Id = dto.Id, unsuccessfulDraft = !response.IsSuccessStatusCode });
        }


        [HttpGet]
        public async Task<Object> GetDraftProspects()
        {
            HttpResponseMessage response = await SendRequest(HttpMethod.Get, "Players", "GetDraftProspects", new string[] { $"Year={DateTime.Now.Year}" });
            return response.IsSuccessStatusCode ? await GetResult<IList<ProspectDto>>(response) : await GetResult<MessageDto>(response);
        }
        #endregion Draft


        #region Trade
        [HttpGet]
        public async Task<IList<TradeDto>> GetTeamTrades(int TeamId)
        {
            HttpResponseMessage response = await SendRequest(HttpMethod.Get, "Teams", "GetTeamTrades", new string[] { $"TeamId={TeamId}" });
            return !response.IsSuccessStatusCode ? [] : await GetResult<IList<TradeDto>>(response);
        }

        [HttpGet]
        public async Task<DraftDto?> GetTeamDraft(int TeamId)
        {
            HttpResponseMessage response = await SendRequest(HttpMethod.Get, "Teams", "TeamDraft", new string[] { $"TeamId={TeamId}" });
            return !response.IsSuccessStatusCode ? null : await GetResult<DraftDto>(response);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetTradePartialAsString(int TeamId, int FranchiseId, int CurrentPick)
        {
            HttpResponseMessage response = await SendRequest(HttpMethod.Post, "Teams", "GetTradeDto", new TradeDto(TeamId, FranchiseId));

            ResultDto<string> result;
            if (response.IsSuccessStatusCode)
            {
                ViewData["CurrentPick"] = CurrentPick;
                result = await RenderViewToString(this, "Trade", await GetResult<TradeDto>(response));
            }
            else
                result = new ResultDto<string>((await GetResult<MessageDto>(response)).Message);

            return Json(result);
        }

        [HttpPost]
        public async Task<MessageDto> RequestTrade(TradeDto dto)
        {
            return await GetResult<MessageDto>(await SendRequest(HttpMethod.Post, "Teams", "SaveTrade", dto));
        }
        #endregion Trade


        #region DeleteTeam
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DeleteTeam(int TeamId)
        {
            HttpResponseMessage response = await SendRequest(HttpMethod.Delete, "Teams", "DeleteTeam", new IdDto(TeamId));
            if (response.IsSuccessStatusCode) return RedirectToAction("MyTeams");

            ResultDto<TeamBasicInfoDto> result = await GetResult<ResultDto<TeamBasicInfoDto>>(response);
            if (result.Message.Equals("Username")) return RedirectToAction("LogOut", "Auth", new RouteValueDictionary(new { ReturnUrl = Url.Action("EditTeam", "Team", new { TeamId }) }));

            ModelState.AddModelError("Edit", "Operación de borrado no completada.");
            return View("Edit", result.Value);
        }
        #endregion DeleteTeam


        #region DuplicateTeam
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DuplicateTeam(int TeamId)
        {
            HttpResponseMessage response = await SendRequest(HttpMethod.Post, "Teams", "DuplicateTeam", new IdDto(TeamId));

            ResultDto<TeamBasicInfoDto> result = await GetResult<ResultDto<TeamBasicInfoDto>>(response);
            if (response.IsSuccessStatusCode && String.IsNullOrWhiteSpace(result.Message)) return RedirectToAction("Edit", new { result.Value.Id });

            if (result.Message.Equals("Username")) 
                return RedirectToAction("LogOut", "Auth", 
                    new RouteValueDictionary(new { ReturnUrl = Url.Action("EditTeam", "Team", new { TeamId }) }));

            ModelState.AddModelError("Edit", "Operación de borrado no completada.");
            return View();
        }
        #endregion DuplicateTeam
    }
}
