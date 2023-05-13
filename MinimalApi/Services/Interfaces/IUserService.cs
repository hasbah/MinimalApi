using MinimalApi.Core;
using MinimalApi.Data.DTO;

namespace MinimalApi.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> IsUniqueEmail(string email);
        Task<APIResponse> Login(LoginRequestDTO loginRequestDTO);
        Task<APIResponse> Register(RegistrationRequestDTO RegisterationRequestDTO);
        Task<APIResponse> Verify(string id, string code);
        Task<APIResponse> ForgotPassword(ForgotPasswordRequestDTO forgotPasswordRequestDTO);
        Task<APIResponse> ResetPassword(ResetPasswordRequestDTO resetPasswordRequestDTO); 
    }
}
