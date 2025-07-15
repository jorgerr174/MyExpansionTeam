using System.ComponentModel.DataAnnotations;
using METCore.Interfaces;
using METCore.Models;
using METDAL.Data;
using Microsoft.EntityFrameworkCore;

namespace METDAL.Repositories
{
    public class UserRepository(ApplicationDbContext context) : BaseRepository<User>(context), IUserRepository
    {
        #region Get
        /// <summary>
        /// Busca un User por Username o Email.
        /// </summary>
        /// <param name="identifier">Parámetro de búsqueda.</param>
        /// <returns>User con Username o Email con igual a identifier, o Null en su defecto.</returns>
        public async Task<User?> GetUserByIdentifier(string identifier)
        {
            User? user = new EmailAddressAttribute().IsValid(identifier)
                ? await _context.Users.SingleOrDefaultAsync(u => u.Email == identifier)
                : await _context.Users.SingleOrDefaultAsync(u => u.Username == identifier);

            return user;
        }

        /// <summary>
        /// Busca un User por Username.
        /// </summary>
        /// <param name="username">Parámetro de búsqueda de User.</param>
        /// <returns>User con Username con igual a username, o Null en su defecto.</returns>
        public async Task<User?> GetUserByUsername(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
        }

        /// <summary>
        /// Busca un User por Email.
        /// </summary>
        /// <param name="email">Parámetro de búsqueda de User.</param>
        /// <returns>User con Email con igual a email, o Null en su defecto.</returns>
        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }
        #endregion


        #region Update
        /// <summary>
        /// Actualiza el User con nuevo Username y/o Password.
        /// </summary>
        /// <param name="user">User a actualizar.</param>
        /// <param name="newUsername">Nuevo valor para Username.</param>
        /// <param name="newPassword">Nuevo valor para Password.</param>
        /// <returns>Número de objetos alterados en la BBDD.</returns>
        public async Task<int> UpdateCredentials(User user, string? newUsername, string? newPassword)
        {
            if (!String.IsNullOrWhiteSpace(newUsername)) user.Username = newUsername;
            if (!String.IsNullOrWhiteSpace(newPassword)) user.Password = newPassword;
            return await _context.SaveChangesAsync();
        }
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
        public async Task<User?> ValidateLogIn(string identifier, string password)
        {
            User? user = await GetUserByIdentifier(identifier);
            return (user == null || user.Password != password) ? null : user;
        }
        #endregion
    }
}
