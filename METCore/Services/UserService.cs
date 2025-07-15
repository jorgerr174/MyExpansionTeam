using AutoMapper;
using METCore.DTOs.User;
using METCore.Interfaces;
using METCore.Models;
using Microsoft.Extensions.Configuration;


namespace METCore.Services
{
    public class UserService(IConfiguration configuration, IUserRepository userRepository, IMapper mapper)
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;


        #region Get
        /// <summary>
        /// Obtener un UserDto con los valores de un User.
        /// </summary>
        /// <param name="username">Valor de búsqeda de un User.</param>
        /// <returns>Opciones:
        /// Null (no se encontró ningún User con ese username).
        /// UserDto (Con los valores del User encontrado).
        /// </returns>
        public async Task<UserDto?> GetDtoByUsername(string username)
        {
            User? user = await _userRepository.GetUserByUsername(username);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }
        #endregion Get


        #region Update
        /// <summary>
        /// Actualizar un User a partir de los valores de UserDto.
        /// </summary>
        /// <param name="username">Valor de búsqeda de un User.</param>
        /// <param name="dto">Clase con los nuevos valores.</param>
        /// <returns>Opciones:
        /// Username (No existe ningún User con Username igual a parámetro).
        /// Error (No se guardaron los cambios en la BBDD).
        /// "" (Todo bien).
        /// </returns>
        public async Task<string> UpdateUser(string username, UserDto dto)
        {
            User? user = await _userRepository.GetUserByUsername(username);
            return user == null ? "Username" :
                await _userRepository.UpdateT(_mapper.Map<User>(dto)) < 1 ? "Error" : "";
        }
        #endregion Update
    }
}
