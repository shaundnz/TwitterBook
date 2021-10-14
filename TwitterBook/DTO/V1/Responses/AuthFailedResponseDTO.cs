using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterBook.DTO.V1.Responses
{
    public class AuthFailedResponseDTO
    {
        public IEnumerable<string> Errors { get; set; }
    }
}
