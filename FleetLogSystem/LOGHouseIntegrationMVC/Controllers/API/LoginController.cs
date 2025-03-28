using AlienRoulleteAPI.Adapters;
using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Model.API.Responses;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LOGHouseSystem.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private UserRepository _userRepository;

        public LoginController()
        {
            _userRepository = new UserRepository();
        }

        [HttpPost]
        public ActionResult<LoginResponse> PostLogin(Login loginRequest)
        {
            LoginResponse response = new LoginResponse();

            if (string.IsNullOrEmpty(loginRequest.LoginName) || string.IsNullOrEmpty(loginRequest.Password))
                return BadRequest("Informe seus dados para fazer login");

            User user = _userRepository.SearchByLogin(loginRequest.LoginName, 0);

            if (user == null)
            {
                user = _userRepository.SearchByEmail(loginRequest.LoginName, 0);
            }

            if (user == null || !user.ValidPassword(loginRequest.Password))
            {
                return Unauthorized("Usuário ou senha inválidos");
            }

            return Ok(new LoginResponse
            {
                UserName = user.Name,
                UserRole = user.PermissionLevel.ToString(),
                AccessToken = JWTAdapter.GenerateToken(user)
            });
        }
    }
}
