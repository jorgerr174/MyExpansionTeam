using METCore.DTOs.Shared;
using METCore.DTOs.User;
using METCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace METAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class UsersController(UserService userService) : ControllerBase
    {
        private readonly UserService _userService = userService;


        #region Profile
        /// <summary>
        /// Obtener un UserDto con los valores del User logeado.
        /// </summary>
        /// <returns>Información de perfil del usuario autenticado</returns>
        [HttpGet("Profile")]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            string? username = User?.Identity?.Name;
            if (String.IsNullOrWhiteSpace(username))
                return BadRequest(new MessageDto("Username"));

            UserDto? user = await _userService.GetDtoByUsername(username);
            if (user == null)
                return BadRequest(new MessageDto("Username"));

            return Ok(user);
        }
        #endregion Profile


        #region UpdateUser
        /// <summary>
        /// Actualizar el User logeado con el userDto recibido.
        /// </summary>
        /// <param name="dto">Nuevos datos del usuario (UpdateUserDto)</param>
        /// <returns>Resultado de la actualización del usuario</returns>
        [HttpPut("UpdateUser")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto dto)
        {
            string? username = User?.Identity?.Name;
            if (String.IsNullOrWhiteSpace(username))
                return BadRequest(new MessageDto("Username"));

            string result = await _userService.UpdateUser(username, dto);
            if (!String.IsNullOrWhiteSpace(result))
                return BadRequest(new MessageDto(result));

            return Ok();
        }
        #endregion UpdateUser


        #region AssignRoles
        /// <summary>
        /// Buscar usuarios con filtros (solo Admin).
        /// </summary>
        /// <param name="dto">Criterios de búsqueda (SearchDto)</param>
        /// <returns>Resultados paginados de usuarios encontrados</returns>
        [HttpPost("List")]
        [Authorize(Roles = "Admin")]
        public async Task<SearchResultDto<UserDto>> List(SearchDto dto)
        {
            return await _userService.Search(dto);
        }
        #endregion AssignRoles
    }
}
