using MicrocopNalogaV2.Models;
using MicrocopNalogaV2.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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

        public AdminController(IAdminRepository adminRepository, IConfiguration config)
        {
            _adminRepository = adminRepository;
            _config = config;
        }


        // Api Post klic da ustvarimo nov Admin profil
        // Admini nimajo zaščite nad svojimi klici. Samo nad api klici uporabnikov
        [HttpPost]
        [Route("Registration")]
        public async Task<IActionResult> Registration([FromBody] AdminModel admin)
        {
                try
                {
                    var tempAdmin = await _adminRepository.Create(admin);
                    return Ok(tempAdmin);
                }
                catch (Exception)
                {

                    return StatusCode((int)HttpStatusCode.InternalServerError, "Creating new admin was not successful.");
                }
        
        }


        // Api Get klic za prijavo Admina, pri tem prejme svoj JwtToken
        // ki ga uporabi za api klice nad uporabniki
        [HttpGet("Signin/{username}/{password}")]
        public async Task<IActionResult> Signin(string username, string password)
        {
            try
            {
                AdminModel tempAdmin = new AdminModel()
                {
                    Username = username,
                    Password = password
                };
                var admin = await AuthUser(tempAdmin);
                if (admin.Id == 0) return StatusCode((int)HttpStatusCode.NotFound, "User does not exists.");
                admin.ApiToken = GenerateJSONWebToken(tempAdmin);
                return Ok(admin);
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "Username or password is incorect.");
            }
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
    }
}

