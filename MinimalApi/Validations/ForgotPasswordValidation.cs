using FluentValidation;
using MinimalApi.Data.DTO;

namespace MinimalApi.Validations
{
    public class ForgotPasswordValidation : AbstractValidator<ForgotPasswordRequestDTO>
    {
        public ForgotPasswordValidation()
        {
            RuleFor(model => model.Email)
                .NotEmpty()
                    .WithMessage("Email address is required")
                .EmailAddress()
                    .WithMessage("A valid email is required"); 
        }
    }
}
