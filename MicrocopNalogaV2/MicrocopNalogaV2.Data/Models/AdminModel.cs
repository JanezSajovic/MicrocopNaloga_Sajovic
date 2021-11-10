using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MicrocopNalogaV2.Models
{
    public class AdminModel
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
        public string ApiToken { get; set; }

        public override string ToString()
        {
            return String.Format("Id: {0}, Username: {1}, Password: {2}, ApiToken: {3}", Id, Username, Password, ApiToken);
        }

    }
}
