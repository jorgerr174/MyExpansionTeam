using METCore.DTOs.Admin;
using METCore.DTOs.Shared;
using METCore.DTOs.User;
using METCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace METAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AuthController(AuthService authService) : ControllerBase
    {
        private readonly AuthService _authService = authService;


        #region CU001 SignUp
        /// <summary>
        /// Crear un nuevo User.
        /// </summary>
        /// <param name="dto">Datos del nuevo usuario (NewUserDto)</param>
        /// <returns>Resultado de la operación de registro de usuario</returns>
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] NewUserDto dto)
        {
            string result = await _authService.SignUp(dto);
            if (!String.IsNullOrWhiteSpace(result))
                return BadRequest(new MessageDto(result));

            return Ok();
        }
        #endregion CU001 SignUp


        #region CU002 LogIn
        /// <summary>
        /// Autenticar un usuario y obtener token JWT.
        /// </summary>
        /// <param name="dto">Credenciales de login (LogInDto)</param>
        /// <returns>Token JWT de acceso</returns>
        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn([FromBody] LogInDto dto)
        {
            string? Token = await _authService.Authenticate(dto.Identifier, dto.Password);
            if (String.IsNullOrWhiteSpace(Token))
                return Unauthorized();

            return Ok(new MessageDto(Token));
        }
        #endregion CU002 LogIn


        #region CU003 LogOut
        #endregion CU003 LogOut


        #region CU004 UpdateCredentials
        /// <summary>
        /// Actualizar las credenciales de acceso del User logeado.
        /// </summary>
        /// <param name="dto">Nuevas credenciales (UpdateCredentialsDto)</param>
        /// <returns>Resultado de la actualización de credenciales</returns>
        [HttpPut("UpdateCredentials")]
        [Authorize]
        public async Task<IActionResult> UpdateCredentials([FromBody] UpdateCredentialsDto dto)
        {
            string? username = User?.Identity?.Name;
            if (String.IsNullOrWhiteSpace(username)) return BadRequest(new MessageDto("Username"));

            var result = await _authService.UpdateCredentials(username, dto);
            if (!String.IsNullOrWhiteSpace(result))
                return BadRequest(new MessageDto(result));

            return Ok();
        }
        #endregion CU004 UpdateCredentials


        #region CU005 DeleteUser
        /// <summary>
        /// Borrar el User logeado.
        /// </summary>
        /// <returns>Resultado de la operación de eliminación de usuario</returns>
        [HttpDelete("DeleteUser")]
        [Authorize]
        public async Task<IActionResult> DeleteUser()
        {
            string? username = User?.Identity?.Name;
            if (String.IsNullOrWhiteSpace(username)) return BadRequest(new MessageDto("Username"));

            string result = await _authService.DeleteUser(username);
            if (!String.IsNullOrWhiteSpace(result))
                return BadRequest(new MessageDto(result));

            return Ok();
        }
        #endregion CU005 DeleteUser


        #region CU019 AssignRole
        /// <summary>
        /// Asignar rol a un usuario (solo Admin).
        /// </summary>
        /// <param name="dto">Datos de asignación de rol (AssignRoleDto)</param>
        /// <returns>Resultado de la asignación de rol</returns>
        [HttpPost("AssignRole")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole(AssignRoleDto dto)
        {
            string? username = User?.Identity?.Name;
            if (String.IsNullOrWhiteSpace(username)) return BadRequest(new MessageDto("Username"));

            string result = await _authService.AssignRole(username, dto);

            return !String.IsNullOrWhiteSpace(result)
                ? BadRequest(new MessageDto(result))
                : Ok();
        }
        #endregion CU019 AssignRole
    }
}
