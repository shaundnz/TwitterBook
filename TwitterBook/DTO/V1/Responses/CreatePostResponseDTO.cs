using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterBook.Models;

namespace TwitterBook.DTO.V1.Responses
{
    public class CreatePostResponseDTO
    {
        public Guid Id { get; set; }
        
        public List<string> Tags { get; set; }
    }
}
