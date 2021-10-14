using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterBook.DTO.V1.Requests
{
    public class UserRegistrationRequestDTO
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
