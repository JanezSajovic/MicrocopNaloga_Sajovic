using MicrocopNalogaV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MicrocopNaloga.Tests
{
    class IUserServiceFake : IUserRepository
    {
        private readonly List<UserModel> _allUsers;

        public IUserServiceFake()
        {
            _allUsers = new List<UserModel>()
            {
                new UserModel() { 
                    Id = 1,
                    UserName = "Tomaž", 
                    Password = "Tomaz123!", 
                    Email = "tomaz@gmial.com",
                    PhoneNumber = "030825825",
                    Culture = "si",
                    Language = "slovenščina",
                    IsValidated = false
                },
                new UserModel() {
                    Id = 2,
                    UserName = "Janez",
                    Password = "Janez123!",
                    Email = "janez@gmial.com",
                    PhoneNumber = "040825825",
                    Culture = "si",
                    Language = "slovenščina",
                    IsValidated = false
                },
                new UserModel() {
                    Id = 4,
                    UserName = "Anna",
                    Password = "Anna123!",
                    Email = "anna@gmial.com",
                    PhoneNumber = "070825825",
                    Culture = "en",
                    Language = "angleščina",
                    IsValidated = true
                }
            };
        }

        public async Task<UserModel> Create(UserModel user)
        {
            user.Password = HashPassword(user.Password);
            _allUsers.Add(user);
            return user;
        }

        public async Task<string> Delete(int id)
        {
            var deleteUser = _allUsers.Find(x => x.Id == id);
            if (deleteUser != null) {
                _allUsers.Remove(deleteUser);
                return "User deleted successfuly.";
            }
            return "User not deleted.";
        }

        public async Task<UserModel> Get(int id)
        {
            var findUser = _allUsers.Find(x => x.Id == id);
            if (findUser != null)
            {
                return findUser;
            }
            return new UserModel();
        }

        public async Task<List<UserModel>> Gets()
        {
            return _allUsers;
        }

        public Task Update(UserModel user)
        {
            throw new NotImplementedException();
        }

        public Task<string> ValidatePassword(string username, string password)
        {
            throw new NotImplementedException();
        }

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
