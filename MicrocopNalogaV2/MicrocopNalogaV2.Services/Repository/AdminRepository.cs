using MicrocopNalogaV2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
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
            return await _context.Admins.Where(x => x.Username == username && x.Password == password).FirstAsync();
        }


        // Pridobivanje Admina, na podlagi celotnega admin modela
        public async Task<AdminModel> GetByUsernamePassword(AdminModel admin)
        {
            _admin = await Get(admin.Username, admin.Password);
            return _admin;
        }
    }
}
