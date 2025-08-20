using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using METCore.DTOs.Admin;
using METCore.DTOs.User;
using METCore.Interfaces;
using METCore.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


namespace METCore.Services
{
    public class AuthService(IConfiguration configuration, IUserRepository userRepository, IMapper mapper)
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;

        #region Update
        /// <summary>
        /// Actualizar las credenciales del User logeado.
        /// </summary>
        /// <param name="username">Username del User logeado.</param>
        /// <param name="model">UpdateCredentialsDto con las nuevas credenciales.</param>
        /// <returns>Opciones:
        /// Nothing(tanto NewUsername y NewPassword están vacíos)
        /// Password (Password actual está vacío).
        /// Credentials (Username y Password no son una combinación válida).
        /// NewUsername (nuevo username ya usado por otro User).
        /// Error (no se guardaron los cambios en la BBDD).
        /// "" (Todo bien).
        /// </returns>
        public async Task<string> UpdateCredentials(string username, UpdateCredentialsDto model)
        {
            if (String.IsNullOrWhiteSpace(model.NewUsername) && String.IsNullOrWhiteSpace(model.NewPassword)) return "Nothing";
            if (String.IsNullOrWhiteSpace(model.Password)) return "Password";
            User? user = await _userRepository.ValidateLogIn(username, model.Password);
            if (user == null) return "Credentials";
            if (await _userRepository.GetUserByUsername(username) == null) return "NewUsername";

            return await _userRepository.UpdateCredentials(user, model.NewUsername, model.NewPassword) < 1 ? "Error" : "";
        }
        #endregion


        #region DeleteUser
        /// <summary>
        /// Borrar el User logeado.
        /// </summary>
        /// <param name="username">Username del User logeado.</param>
        /// <returns>Opciones:
        /// Username (No existe ningún User con Username igual a parámetro).
        /// Error (no se guardaron los cambios en la BBDD).
        /// "" (Todo bien).
        /// </returns>
        public async Task<string> DeleteUser(string username)
        {
            User? user = await _userRepository.GetUserByUsername(username);

            return user == null ? "Username" :
                await _userRepository.DeleteT(user) < 0 ? "Error" : "";
        }
        #endregion DeleteUser


        #region DeleteUser
        /// <summary>
        /// Borrar el User logeado.
        /// </summary>
        /// <param name="username">Username del User logeado.</param>
        /// <returns>Opciones:
        /// Username (No existe ningún User con Username igual a parámetro).
        /// Error (no se guardaron los cambios en la BBDD).
        /// "" (Todo bien).
        /// </returns>
        public async Task<string> AssignRole(string username, AssignRoleDto dto)
        {
            User? curUser = await _userRepository.GetUserByUsername(username);
            if (curUser is null) return "User";
            if (curUser.Role is not Enums.Types.RoleEnum.Admin) return "NotAdmin";

            User? user = await _userRepository.GetUserByUsername(dto.Username);
            if (user is null) return "Username";
            if (user.Role is Enums.Types.RoleEnum.Admin) return "Admin";

            user.Role = dto.Role;
            return await _userRepository.UpdateT(user) < 1 ? "Error" : String.Empty;
        }
        #endregion DeleteUser


        #region Other
        /// <summary>
        /// Validar el inicio de sesión con los parámetros como credenciales, y generar el token.
        /// </summary>
        /// <param name="identifier">Username o Email de un User.</param>
        /// <param name="password">Password del User encontrado.</param>
        /// <returns>Opciones:
        /// Null (credenciales inválidas).
        /// Token generado.
        /// </returns>
        public async Task<string?> Authenticate(string identifier, string password)
        {
            User? user = await _userRepository.ValidateLogIn(identifier, password);
            if (user == null) return null;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, identifier),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            // Generate token
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? ""));
            var creds = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken Token = new(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(Token);
        }

        /// <summary>
        /// Validar el inicio de sesión con los parámetros como credenciales, y generar el token.
        /// </summary>
        /// <param name="dto">UserDto con lo valores para el nuevo User.</param>
        /// <returns>Opciones:
        /// Password (Password vacío).
        /// Email (Email ya usado por otro usuario).
        /// Username (Username ya usado por otro usuario).
        /// Error (no se guardaron los cambios en la BBDD).
        /// "" (Todo bien).
        /// </returns>
        public async Task<string> SignUp(NewUserDto dto)
        {
            if (String.IsNullOrWhiteSpace(dto.Password))
                return "Password";
            if (await _userRepository.GetUserByEmail(dto.Email) != null)
                return "Email";
            if (await _userRepository.GetUserByUsername(dto.Username) != null)
                return "Username";

            User newUser = _mapper.Map<User>(dto);
            newUser.Password = dto.Password;

            return await _userRepository.CreateT(newUser) < 1 ? "Error" : "";
        }
        #endregion
    }

}
