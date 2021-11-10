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
using System.Threading.Tasks;

namespace MicrocopNalogaV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private IConfiguration _config;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserRepository adminRepository, IConfiguration config, ILogger<UserController> logger)
        {
            _userRepository = adminRepository;
            _config = config;
            _logger = logger;
        }


        // Api Get klic za seznam vseh uporabnikov v bazi
        [HttpGet]
        [Route("ListOfUsers")]
        public async Task<List<UserModel>> Gets() {
            try
            {
                var userList = await _userRepository.Gets();
                LoggingCalls("Info", "HttpGet", "no parameters", "Geting list of all users in database.");
                return userList;
            }
            catch (Exception ex)
            {
                LoggingCalls("Error", "HttpGet", "no parameters", ex.Message);
                throw;
            }
            
            
        }

        // Api Get klic za pridobitev posameznega uporabnika na podlagi njegovega IDja
        // Klic je zaščiten z JwtBearer tokenom
        // Do klica lahko dostopajo samo registrirani in prijavljeni Admini
        [HttpGet("GetUser/{userId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<UserModel> Get(int userId)
        {
            try
            {
                var tempUser = await _userRepository.Get(userId);
                if (tempUser != null) {
                    LoggingCalls("Info", "HttpGet", userId.ToString(), "Getting one user by its ID.");
                    return tempUser;
                }
            }
            catch (Exception ex)
            {
                LoggingCalls("Error", "HttpGet", userId.ToString(), ex.Message);
                throw;
            }
            return await _userRepository.Get(userId);
        }

        // Api post klic za kreiranje novega uporabnika
        // Podatke preberemo iz telesa json zapisa ([FromBody])
        // Klic je zaščiten z JwtBearer tokenom
        [HttpPost("CreateUser")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<UserModel> Create([FromBody]UserModel user)
        {
            try
            {
                var tempUser = await _userRepository.Create(user);
                if (tempUser != null) {
                    LoggingCalls("Info", "HttpPost", user.ToString(), "Creating user with body parameters.");
                    return tempUser;
                }
                
            }
            catch (Exception ex)
            {
                LoggingCalls("Error", "HttpPost", user.ToString(), ex.Message);
                throw;
            }
            return await _userRepository.Create(user);
        }

        // Api Put klic za posodobitev uporabnikovih informacij
        // Klic je zaščiten z JwtBearer tokenom
        [HttpPut("UpdateUser/{userId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Update(int userId, [FromBody]UserModel user)
        {
            try
            {
                var tempUser = _userRepository.Get(userId);
                if (userId != tempUser.Id || tempUser == null)
                {
                    LoggingCalls("Error", "HttpPut", userId + " " + user.ToString(), "Bad request error.");
                    return BadRequest();
                }
                await _userRepository.Update(tempUser.Result);
                LoggingCalls("Info", "HttpPut", userId + " " + user.ToString(), "Updating the user data.");
                return NoContent();
            }
            catch (Exception ex)
            {
                LoggingCalls("Error", "HttpPut", userId + " "+ user.ToString(), ex.Message);
                throw;
            }
            

            
        }

        // Api Delete klic za brisanje uporabnika, na podlagi njegovega IDja
        // Klic je zaščiten z JwtBearer tokenom
        [HttpDelete("DeleteUser/{userId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<string> Delete(int userId)
        {
            try
            {
                var tempUser = _userRepository.Get(userId).Result;
                if (tempUser == null)
                {
                    LoggingCalls("Error", "HttpDelete", userId.ToString(), "User does not exists.");
                    return "Uporabnik ne obstaja";
                }
                await _userRepository.Delete(tempUser.Id);
                LoggingCalls("Info", "HttpDelete", userId.ToString(), "User was successfuly deleted.");
                return "Izbris uspešen";
            }
            catch (Exception ex)
            {
                LoggingCalls("Error", "HttpDelete", userId.ToString() , ex.Message);
                throw;
            }
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
