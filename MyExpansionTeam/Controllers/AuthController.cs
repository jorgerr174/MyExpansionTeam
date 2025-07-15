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


        #region Create
        /// <summary>
        /// Crear un nuevo User.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>Opciones:
        /// Password (Password vacío).
        /// Email (Email ya usado por otro usuario).
        /// Username (Username ya usado por otro usuario).
        /// Error (no se guardaron los cambios en la BBDD).
        /// Nada (Ejecución correcta).
        /// </returns>
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] UserDto dto)
        {
            string result = await _authService.SignUp(dto);
            if (!String.IsNullOrWhiteSpace(result))
                return BadRequest(new MessageDto(result));

            return Ok();
        }
        #endregion Create


        #region Update
        /// <summary>
        /// Actualizar las credenciales de acceso del User logeado.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>Opciones:
        /// Username (vacío o no existe un User con ese username).
        /// Nothing(tanto NewUsername y NewPassword están vacíos)
        /// Password (Password actual está vacío).
        /// Credentials (Username y Password no son una combinación válida).
        /// NewUsername (nuevo username ya usado por otro User).
        /// Error (no se guardaron los cambios en la BBDD).
        /// Nada (Ejecución correcta).
        /// </returns>
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
        #endregion


        #region Delete
        /// <summary>
        /// Borrar el User logeado.
        /// </summary>
        /// <returns>Opciones:
        /// Username (vacío o no existe un User con ese username).
        /// Error (no se guardaron los cambios en la BBDD).
        /// Nada (Ejecución correcta).
        /// </returns>
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
        #endregion


        #region Other
        /// <summary>
        /// Actualizar las credenciales de acceso del User logeado.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>Opciones:
        /// Null (credenciales inválidas).
        /// Token generado.
        /// </returns>
        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn([FromBody] LogInDto dto)
        {
            string? Token = await _authService.Authenticate(dto.Identifier, dto.Password);
            if (String.IsNullOrWhiteSpace(Token))
                return Unauthorized();

            return Ok(new MessageDto(Token));
        }
        #endregion Other
    }
}
