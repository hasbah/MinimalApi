using FluentValidation;
using MinimalApi.Data.DTO;
using MinimalApi.Services.Interfaces;

namespace MinimalApi.Validations
{
    public class RegistrationValidation : AbstractValidator<RegistrationRequestDTO>
    {
        private readonly IUserService _userRepository;
        public RegistrationValidation(IUserService userRepository)
        {
            _userRepository = userRepository;

            RuleFor(model => model.Name)
                .NotEmpty()
                    .WithMessage("Name is required");
            RuleFor(model => model.Surname)
                .NotEmpty()
                    .WithMessage("Surname is required");
            RuleFor(model => model.Email)
                .NotEmpty()
                    .WithMessage("Email address is required")
                .EmailAddress()
                    .WithMessage("A valid email is required")
                .MustAsync(IsUniqueEmail)
                    .WithMessage("Username already exists");
            RuleFor(model => model.Password)
                .NotEmpty() 
                    .WithMessage("Password is required")
                .MinimumLength(8)
                    .WithMessage("Your password length must be at least 8")
                .MaximumLength(16)
                    .WithMessage("Your password length must not exceed 16")
                .Matches(@"[A-Z]+")
                    .WithMessage("Your password must contain at least one uppercase letter")
                .Matches(@"[a-z]+")
                    .WithMessage("Your password must contain at least one lowercase letter")
                .Matches(@"[0-9]+")
                    .WithMessage("Your password must contain at least one number")
                .Matches(@"[\!\?\*\.]+")
                    .WithMessage("Your password must contain at least one (!? *.)");
            RuleFor(model => model.ConfirmPassword)
                .NotEmpty()
                    .WithMessage("Confirm Password is required")
                .Equal(model => model.Password)
                    .WithMessage("Your password and confirmation password do not match");
        }

        private async Task<bool> IsUniqueEmail(string email, CancellationToken token)
        {
            return await _userRepository.IsUniqueEmail(email);
        }
    }
}
