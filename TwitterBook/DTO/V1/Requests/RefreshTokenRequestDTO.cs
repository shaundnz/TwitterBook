using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterBook.DTO.V1.Requests
{
    public class RefreshTokenRequestDTO
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }
    }
}
