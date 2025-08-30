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


        #region Get
        /// <summary>
        /// Obtener un UserDto con los valores del User logeado.
        /// </summary>
        /// <returns>Opciones:
        /// Username (vacío o no existe un User con ese username).
        /// UserDto (Con los valores del User logeado).
        /// </returns>
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
        #endregion


        #region Update
        /// <summary>
        /// Actualizar el User logeado con el userDto recibido.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>Opciones:
        /// Username (vacío o no existe un User con ese username).
        /// Error (no se pudieron guardar los cambios).
        /// Nada (Ejecución correcta).
        /// </returns>
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
        #endregion


        #region AssignRoles
        [HttpPost("List")]
        [Authorize(Roles = "Admin")]
        public async Task<SearchResultDto<UserDto>> List(SearchDto dto)
        {
            return await _userService.Search(dto);
        }
        #endregion AssignRoles
    }
}
