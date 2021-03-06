using MicrocopNalogaV2.Models;
using MicrocopNalogaV2.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MicrocopNalogaV2.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class AdminController : ControllerBase { 
        private readonly IAdminRepository _adminRepository;
        private IConfiguration _config;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminRepository adminRepository, IConfiguration config, ILogger<AdminController> logger)
        {
            _adminRepository = adminRepository;
            _config = config;
            _logger = logger;
        }


        // Api Post klic da ustvarimo nov Admin profil
        // Admini nimajo zaščite nad svojimi klici. Samo nad api klici uporabnikov
        [HttpPost]
        [Route("Registration")]
        public async Task<IActionResult> Registration([FromBody] AdminMiniModel admin)
        {
            AdminModel tempAdmin = new AdminModel()
            {
                Username = admin.Username,
                Password = admin.Password
            };
            var _Admin = await _adminRepository.Create(tempAdmin);
            if (_Admin == null) {
                LoggingCalls("Error", "HttpPost", _Admin.ToString(), "Creating admin failed.");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Creating new admin was not successful.");
            }
            LoggingCalls("Info", "HttpPost", _Admin.ToString(), "Crating admin successful.");
            return Ok(_Admin);
        }


        // Api Get klic za prijavo Admina, pri tem prejme svoj JwtToken
        // ki ga uporabi za api klice nad uporabniki
        [HttpGet("Signin/{username}/{password}")]
        public async Task<IActionResult> Signin(string username, string password)
        {
            AdminModel tempAdmin = new AdminModel()
            {
                Username = username,
                Password = password
            };
            var admin = await AuthUser(tempAdmin);
            if (admin.Id == 0 || admin == null)
            {
                LoggingCalls("Error", "HttpGet", username+" "+password, "Signin in for admin failed.");
                return StatusCode((int)HttpStatusCode.NotFound, "Admin does not exists.");
            }
            admin.ApiToken = GenerateJSONWebToken(tempAdmin);
            LoggingCalls("Error", "HttpGet", username +" "+ password, "Signin in for admin successful.");
            return Ok(admin);
        }


        // Avtentikacija uporabnika, s tem da pridobi svoj JwtToken
        public async Task<AdminModel> AuthUser(AdminModel admin)
        {
            return await _adminRepository.GetByUsernamePassword(admin);
        }

        // Generiranje JwtToken-a za vsakega novega uporabnika, ki se registrira in prijavi
        // Generiranje temelji na vnešenih podatkih s strani adminove registracije
        private string GenerateJSONWebToken(AdminModel adminInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

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

