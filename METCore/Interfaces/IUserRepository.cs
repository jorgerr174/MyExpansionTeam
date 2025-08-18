using METCore.DTOs.Shared;
using METCore.Models;

namespace METCore.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        #region Get
        /// <summary>
        /// Busca un User por Username o Email.
        /// </summary>
        /// <param name="identifier">Parámetro de búsqueda de User.</param>
        /// <returns>User con Username o Email con igual a identifier, o Null en su defecto.</returns>
        Task<User?> GetUserByIdentifier(string identifier);

        /// <summary>
        /// Busca un User por Username.
        /// </summary>
        /// <param name="username">Parámetro de búsqueda de User.</param>
        /// <returns>User con Username con igual a username, o Null en su defecto.</returns>
        Task<User?> GetUserByUsername(string username);

        /// <summary>
        /// Busca un User por Email.
        /// </summary>
        /// <param name="email">Parámetro de búsqueda de User.</param>
        /// <returns>User con Email con igual a email, o Null en su defecto.</returns>
        Task<User?> GetUserByEmail(string email);

        Task<(List<User> Users, int TotalCount)> SearchUsersAsync(SearchDto dto);
        #endregion


        #region Update
        /// <summary>
        /// Actualiza el User con nuevo Username y/o Password.
        /// </summary>
        /// <param name="user">User a actualizar.</param>
        /// <param name="newUsername">Nuevo valor para Username.</param>
        /// <param name="newPassword">Nuevo valor para Password.</param>
        /// <returns>Número de objetos alterados en la BBDD.</returns>
        Task<int> UpdateCredentials(User user, string? newUsername, string? newPassword);
        #endregion


        #region Others
        /// <summary>
        /// Comprueba si unas credenciales on correctas.
        /// Primero busca que existe un usuario con Username o Email igual a identifier.
        /// Luego comprueba si la contraseña coincide con el del usuario si existe.
        /// </summary>
        /// <param name="identifier">Parámetro de búsqueda de User.</param>
        /// <param name="password">Contraseña a comprobar en el User.</param>
        /// <returns>User que tiene esos parámetros o null en su defecto.</returns>
        Task<User?> ValidateLogIn(string identifier, string password);
        #endregion
    }
}
