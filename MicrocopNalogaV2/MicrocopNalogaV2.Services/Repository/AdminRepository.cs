using MicrocopNalogaV2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MicrocopNalogaV2.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AdminContext _context;
        private readonly IConfiguration _config;
        AdminModel _admin = new AdminModel();

        public AdminRepository(AdminContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }


        // Kreiranje novega admin računa in shranjevanje v bazo admin-ov
        public async Task<AdminModel> Create(AdminModel admin)
        {
            admin.Password = HashPassword(admin.Password);
            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();
            return admin;
        }

        // Pridobivanje informacij, na podlagi adminovega IDja
        public async Task<AdminModel> Get(int id)
        {
            return await _context.Admins.FindAsync(id);
        }

        // Pridobivanje Admina, na podlagi adminovega uporabniškega ime in gesla
        public async Task<AdminModel> Get(string username, string password)
        {
            return await _context.Admins.Where(x => x.Username == username && x.Password == HashPassword(password)).FirstAsync();
        }


        // Pridobivanje Admina, na podlagi celotnega admin modela
        public async Task<AdminModel> GetByUsernamePassword(AdminModel admin)
        {
            _admin = await Get(admin.Username, admin.Password);
            return _admin;
        }

        // Heširanje gesla admina da se ob izpisu na pokaže geslo kot je bilo vpisano
        // Poskrbimo za varnost
        public string HashPassword(string password)
        {
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
