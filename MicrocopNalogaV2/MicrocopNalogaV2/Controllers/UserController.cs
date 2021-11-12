using MicrocopNalogaV2.Models.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MicrocopNalogaV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserRepository adminRepository, ILogger<UserController> logger)
        {
            _userRepository = adminRepository;
            _logger = logger;
        }


        // Api Get klic za seznam vseh uporabnikov v bazi
        [HttpGet]
        [Route("ListOfUsers")]
        public async Task<IActionResult> Gets() {
            var userList = await _userRepository.Gets();
            if (userList == null) {
                LoggingCalls("Error", "HttpGet", "no parameters", "No users in database.");
                return StatusCode((int)HttpStatusCode.InternalServerError, "No users in database.");
            }
            LoggingCalls("Info", "HttpGet", "no parameters", "Geting list of all users in database.");
            return Ok(userList);
        }

        // Api Get klic za pridobitev posameznega uporabnika na podlagi njegovega IDja
        // Klic je zaščiten z JwtBearer tokenom
        // Do klica lahko dostopajo samo registrirani in prijavljeni Admini
        [HttpGet("GetUser/{userId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Get(int userId)
        {
            var tempUser = await _userRepository.Get(userId);
            if (tempUser == null)
            {
                LoggingCalls("Error", "HttpGet", userId.ToString(), "User not found by given ID.");
                return StatusCode((int)HttpStatusCode.NotFound, "User not found by given ID");
            }
            LoggingCalls("Info", "HttpGet", userId.ToString(), "Getting one user by its ID.");
            return Ok(tempUser);
        }

        // Api post klic za kreiranje novega uporabnika
        // Podatke preberemo iz telesa json zapisa ([FromBody])
        // Klic je zaščiten z JwtBearer tokenom
        [HttpPost("CreateUser")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Create([FromBody]UserMiniModel user)
        {
            UserModel tempUser = new UserModel()
            {
                UserName = user.UserName,
                FullName = user.FullName,
                Password = user.Password,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Culture = user.Culture,
                Language = user.Language
            };
            var _User = await _userRepository.Create(tempUser);
            if (_User == null)
            {
                LoggingCalls("Error", "HttpPost", _User.ToString(), "Creating user failed.");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Creating user failed.");
            }
            LoggingCalls("Info", "HttpPost", _User.ToString(), "Creating user with body parameters successfull.");
            return Ok(_User);
        }

        // Api Put klic za posodobitev uporabnikovih informacij
        // Klic je zaščiten z JwtBearer tokenom
        [HttpPut("UpdateUser/{userId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Update(int userId, [FromBody]UserMiniModel user)
        {
            UserModel tempUser = new UserModel()
            {
                UserName = user.UserName,
                FullName = user.FullName,
                Password = user.Password,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Culture = user.Culture,
                Language = user.Language
            };
            var _User = _userRepository.Get(userId).Result;
            if (userId != _User.Id || _User == null)
            {
                LoggingCalls("Error", "HttpPut", userId + " " + _User.ToString(), "Bad request error.");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Updating user failed.");
            }
            await _userRepository.Update(_User);
            LoggingCalls("Info", "HttpPut", userId + " " + _User.ToString(), "User data successfuly updated.");
            return Ok(_User);
        }

        // Api Delete klic za brisanje uporabnika, na podlagi njegovega IDja
        // Klic je zaščiten z JwtBearer tokenom
        [HttpDelete("DeleteUser/{userId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Delete(int userId)
        {
            var tempUser = _userRepository.Get(userId).Result;
            if (tempUser == null)
            {
                LoggingCalls("Error", "HttpDelete", userId.ToString(), "User does not exists.");
                return StatusCode((int)HttpStatusCode.NotFound, "Deliting user failed. User not found. ");
            }
            await _userRepository.Delete(tempUser.Id);
            LoggingCalls("Info", "HttpDelete", userId.ToString(), "User successfuly deleted.");
            return Ok("User successfuly deleted.");
        }


        // Api Post klic za validiranje uporabnikovega gesla
        // Klic je zaščiten z JwtBearer tokenom
        [HttpPost("ValidatePassword/{username}/{password}")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<string> ValidatePassword(string username, string password)
        {
            return await _userRepository.ValidatePassword(username, password);
        }


        // Logiranje posameznega klica preko api kontrolerja
        // Zapisujemo na dnevni ravni v /LogFilesDaily s pomočjo NLog paketa
        // Konfiguracija se nahaja v nlog.config
        private void LoggingCalls(string level, string method, string para, string msg)
        {
            var clientIP = HttpContext.Connection.RemoteIpAddress?.ToString();
            var clientName = HttpContext.Features.Get<IServerVariablesFeature>()["REMOTE_HOST"];
            var hostName = string.Concat(this.Request.Scheme, "://", this.Request.Host, this.Request.Path, this.Request.QueryString);
            string log =
                "Log level: " + level + " Time: " + DateTime.Now +
                " ClientIp: " + clientIP + " ClientName: " + clientName +
                " Host name: " + hostName + " API method: " + method +
                " Request parameters: " + para + " Message: " + msg + "";
            _logger.LogInformation(log);
        }
    }
}
