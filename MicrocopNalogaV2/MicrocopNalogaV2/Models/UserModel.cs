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
    }
}
