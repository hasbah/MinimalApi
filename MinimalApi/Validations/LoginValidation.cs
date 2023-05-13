using FluentValidation;
using MinimalApi.Data.DTO;

namespace MinimalApi.Validations
{
    public class LoginValidation : AbstractValidator<LoginRequestDTO>
    {
        public LoginValidation()
        {
            RuleFor(model => model.Email)
                .NotEmpty()
                    .WithMessage("Email address is required")
                .EmailAddress()
                    .WithMessage("A valid email is required");
            RuleFor(model => model.Password)
                .NotEmpty()
                    .WithMessage("Password is required");
        }
    }
}
