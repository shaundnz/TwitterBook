using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterBook.DTO.V1.Responses
{
    public class PostResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string UserId { get; set; }

        public List<TagResponseDTO> Tags { get; set; }


    }
}
