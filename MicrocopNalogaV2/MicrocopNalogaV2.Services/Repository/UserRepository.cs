using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MicrocopNalogaV2.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserContext _context;
        private readonly IConfiguration _config;

        // V repozitorij pripeljemo kontext baze uporabnikov in konfiguracijo
        public UserRepository(UserContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // Dodajanje uporabnika v bazo | Geslo se hešira | Ko ga dodamo uporabnikovo geslo še ni validirano
        public async Task<UserModel> Create(UserModel user)
        {
            try
            {
                user.Password = HashPassword(user.Password);
                user.IsValidated = false;
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception)
            {
                throw;
            }
            
            
        }


        // Brisanje uporabnika iz baze | Brišemo na podlagi IDja | Če ID ne ustreza nobenemu zapisu v bazi vrnemo napako
        // V odgovor pošljemo ali je bil uporabnik izbrisan uspešno ali ne
        public async Task<string> Delete(int id)
        {
            string msg = "";
            try
            {
                var userDelete = await _context.Users.FindAsync(id);
                _context.Users.Remove(userDelete);
                await _context.SaveChangesAsync();
                msg = "Uporabnik izbrisan.";
            }
            catch (Exception)
            {
                msg = "Uporabnik ni izbrisan.";
            }
            return msg;
        }


        // Pridobivanje uporabnika na podlagi njegovega IDja
        // Vračamo vse uporabnikove informacije
        public async Task<UserModel> Get(int id)
        {
            try
            {
                return await _context.Users.FindAsync(id);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        // Izpis seznama vseh uporabnikov, ki jih hranimo v bazi

        public async Task<List<UserModel>> Gets()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
            
        }


        // Posodobitev ene ali večih uporabnikovih vrednosti
        public async Task Update(UserModel user)
        {
            try
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }


        // Validiranje/preverjanje ali je uporabnikovo geslo pravilno
        // Uporabnika pridobimo na poglagi njegovega uporabniškega imena in gesla, ki pa je v bazi heširano
        // Če se gesli ujemata uporabniku polje "IsValidated" spremenimo na "true" (privzeto je "false")
        public async Task<string> ValidatePassword(string username, string password)
        {
            string msg = "";
            try
            {
                var validatedUser = await _context.Users.Where(x => x.UserName == username && x.Password == HashPassword(password)).FirstOrDefaultAsync();
                validatedUser.IsValidated = true;
                _context.Users.Update(validatedUser);
                await _context.SaveChangesAsync();
                msg = "Uporabnikovo geslo uspešno potrjeno/validirano.";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
        }

        // Heširanje gesla uporabnika da se ob izpisu na pokaže geslo kot je bilo vpisano
        // Poskrbimo za varnost
        public string HashPassword(string password) {
            var hashed = "";
            using (var myHash = SHA256Managed.Create())
            {
                var byteArrayResultOfRawData = Encoding.UTF8.GetBytes(password);
                var byteArrayResult = myHash.ComputeHash(byteArrayResultOfRawData);
                hashed = string.Concat(Array.ConvertAll(byteArrayResult, h => h.ToString("X2")));
            }

            return hashed;
        }
    }
}