using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MicrocopNalogaV2
{
    public class UserModel
    {
        public UserModel() {
            IsValidated = false;
        }

        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string Language { get; set; }

        [Required]
        public string Culture { get; set; }

        [Required]
        public string Password { get; set; }

        public bool IsValidated { get; set; }


        public override string ToString()
        {
            return String.Format("Id: {0}, Username: {1}, Password: {2}, Full name: {3}, Email: {4}, " +
                "Phone number: {5}, Language: {6}, Culture: {7}, Is validated: {8}", 
                Id, UserName, Password, FullName, Email, PhoneNumber, Language, Culture, IsValidated);
        }
    }
}
