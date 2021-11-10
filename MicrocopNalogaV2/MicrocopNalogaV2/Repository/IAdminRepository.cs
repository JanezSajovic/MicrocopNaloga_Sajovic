using MicrocopNalogaV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicrocopNalogaV2.Repository
{
    public interface IAdminRepository
    {
        // Definicija vseh možnih api klicev in operacij nad admini
        Task<AdminModel> Get(int id);
        Task<AdminModel> Get(string username, string password);
        Task<AdminModel> Create(AdminModel user);
        Task<AdminModel> GetByUsernamePassword(AdminModel user);
    }
}
