using System.Collections.Generic;

namespace TwitterBook.DTO.V1.Responses
{
    public class ErrorResponseDTO
    {
        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();
    }
}