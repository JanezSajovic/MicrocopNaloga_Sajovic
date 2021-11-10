
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicrocopNalogaV2
{
    public interface IUserRepository
    {

        // Definicija vseh možnih api klicev in operacij nad uporabniki
        Task<UserModel> Get(int id);
        Task<List<UserModel>> Gets();
        Task<UserModel> Create(UserModel user);
        Task Update(UserModel user);
        Task<string> Delete(int id);
        Task<string> ValidatePassword(string username, string password);
    }
}
