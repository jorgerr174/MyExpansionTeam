using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using METCore.DTOs.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Account;

namespace WebApp.Controllers
{
    public class AccountController(IHttpClientFactory httpClientFactory, IConfiguration configuration) : BaseController(httpClientFactory, configuration)
    {
        #region LogIn
        [HttpGet]
        public IActionResult LogIn(string? RedirectUrl = null)
        {
            return View(new LogInViewModel(RedirectUrl));
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(LogInViewModel model)
        {
            HttpResponseMessage response = await SendRequest(HttpMethod.Post, "Auth", "LogIn", model);

            if (response.IsSuccessStatusCode)
            {
                MessageDto result = await GetResult<MessageDto>(response);

                // Store the JWT securely (cookie, session, etc.)
                // Example: store in cookie
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new JwtSecurityTokenHandler().ReadJwtToken(result.Message).Claims,
                            CookieAuthenticationDefaults.AuthenticationScheme
                        )
                    ),
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddHours(2)
                    });

                Response.Cookies.Append("jwt", result.Message, new CookieOptions { HttpOnly = true });
                return String.IsNullOrWhiteSpace(model.RedirectUrl) ? RedirectToAction("Index", "Home") : Redirect(model.RedirectUrl);
            }

            ModelState.AddModelError("LogIn", "Credenciales inválidas.");
            return View(model);
        }
        #endregion


        #region LogOut
        [HttpGet]
        public async Task<IActionResult> LogOut(string? ReturnUrl)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("jwt");
            return String.IsNullOrWhiteSpace(ReturnUrl)
                ? RedirectToAction("Index", "Home")
                : RedirectToAction("LogIn", new RouteValueDictionary(new { ReturnUrl }));
        }
        #endregion


        #region SignUp

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            HttpResponseMessage response = await SendRequest(HttpMethod.Post, "Auth", "SignUp", model);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("LogIn");

            MessageDto result = await GetResult<MessageDto>(response);
            if (result.Message.Equals("Error"))
                ModelState.AddModelError("Error", "No se han podido crear el usuario en la BBDD.");
            else if (result.Message.Equals("Password"))
                ModelState.AddModelError("Password", "No se ha envíado una contraseña.");
            else
                ModelState.AddModelError(result.Message, result.Message + " ya usado.");
            return View(model);
        }
        #endregion


        #region Profile
        [Route("[Controller]/{username}", Name = "UserDetails", Order = 1)]
        [HttpGet]
        public async Task<IActionResult> GetUserProfile(string username)
        {
            if ((User?.Identity?.Name ?? string.Empty) == username) return RedirectToAction("Profile");
            HttpResponseMessage response = await SendRequest(HttpMethod.Get, "Users", "Profile", new MessageDto(username));
            return !response.IsSuccessStatusCode ? RedirectToAction("Index", "Home") : View(await GetResult<UserViewModel>(response));
        }

        private async Task<UserViewModel?> GetProfile()
        {
            HttpResponseMessage response = await SendRequest(HttpMethod.Get, "Users", "Profile");
            return !response.IsSuccessStatusCode ? null : await GetResult<UserViewModel>(response);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var result = await GetProfile();
            return result == null ? RedirectToAction("LogOut", new { RetController = "Account", ReturnMethod = "Profile" }) : View(result);
        }
        #endregion


        #region EditProfile
        private async Task<ProfileViewModel?> GetEditProfile()
        {
            var result = await GetProfile();
            return result == null ? null : new ProfileViewModel() { UpdateUser = result, UpdateCredentials = new CredentialsViewModel() };
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            ProfileViewModel? model = await GetEditProfile();
            if (model == null) return RedirectToAction("LogOut", new RouteValueDictionary(new { ReturnUrl = Url.Action("Profile") }));

            ViewData["userTab"] = true;
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateUser(UserViewModel userModel)
        {
            HttpResponseMessage response = await SendRequest(HttpMethod.Put, "Users", "UpdateUser", userModel);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Profile");

            MessageDto result = await GetResult<MessageDto>(response);
            if (result.Equals("Username")) return RedirectToAction("LogOut", new RouteValueDictionary(new { ReturnUrl = Url.Action("Profile") }));

            ProfileViewModel? model = await GetEditProfile();
            if (model == null) return RedirectToAction("LogOut", new RouteValueDictionary(new { ReturnUrl = Url.Action("Profile") }));

            ModelState.AddModelError("UpdateUser", "Operación de guardado no completada.");
            ViewData["userTab"] = true;
            return View("EditProfile", model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateCredentials(CredentialsViewModel credentialModel)
        {
            HttpResponseMessage response = await SendRequest(HttpMethod.Put, "Auth", "UpdateCredentials", credentialModel);
            if (response.IsSuccessStatusCode) return RedirectToAction("LogOut", new RouteValueDictionary(new { ReturnUrl = Url.Action("Profile") }));
            // se indica que al cambiar las credenciales correctamente se cerrará la sesión.

            MessageDto result = await GetResult<MessageDto>(response);
            if (result.Message.Equals("Username")) return RedirectToAction("LogOut", new RouteValueDictionary(new { ReturnUrl = Url.Action("Profile") }));
            ProfileViewModel? model = await GetEditProfile();
            if (model == null) return RedirectToAction("LogOut", new RouteValueDictionary(new { ReturnUrl = Url.Action("Profile") }));

            if (result.Message.Equals("Nothing")) ModelState.AddModelError("UpdateCredentials", "No se solicitó ningún cambio.");
            else if (result.Message == "Password") ModelState.AddModelError("Password", "Es obligatorio introducir la contraseña.");
            else if (result.Message == "Credentials") ModelState.AddModelError("Password", "Contraseña no es correcta.");
            else if (result.Message == "NewUsername") ModelState.AddModelError("NewUsername", "Nuevo username ya en uso.");
            else ModelState.AddModelError("UpdateCredentials", "Operación de guardado no completada.");

            model.UpdateCredentials = credentialModel;
            ViewData["userTab"] = false;
            return View("EditProfile", model);
        }
        #endregion


        #region DeleteUser
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DeleteUser()
        {
            HttpResponseMessage response = await SendRequest(HttpMethod.Delete, "Auth", "DeleteUser");
            if (response.IsSuccessStatusCode)
                return RedirectToAction("LogOut");

            MessageDto result = await GetResult<MessageDto>(response);
            if (result.Message.Equals("Username")) return RedirectToAction("LogOut", new RouteValueDictionary(new { ReturnUrl = Url.Action("Profile") }));

            var profileModel = await GetProfile();
            ModelState.AddModelError("Profile", "Operación de borrado no completada.");
            return View("Profile", profileModel);
        }
        #endregion DeleteUser
    }
}