using FluentValidation;
using TwitterBook.DTO.V1.Requests;

namespace TwitterBook.Validators
{
    public class CreatePostRequestDTOValidator : AbstractValidator<CreatePostRequestDTO>
    {
        public CreatePostRequestDTOValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Matches("^[a-zA-Z0-9 ]*$");
        }
    }
}