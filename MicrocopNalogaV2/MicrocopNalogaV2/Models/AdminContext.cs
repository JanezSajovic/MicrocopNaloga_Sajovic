using MicrocopNalogaV2.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicrocopNalogaV2
{
    public class AdminContext : DbContext
    {
        public AdminContext(DbContextOptions<AdminContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<AdminModel> Admins { get; set; }
    }


}
