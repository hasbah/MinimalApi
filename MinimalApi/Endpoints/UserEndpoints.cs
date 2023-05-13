using Microsoft.AspNetCore.Mvc; 
using MinimalApi.Core;
using MinimalApi.Data.DTO;
using MinimalApi.Filters;
using MinimalApi.Services.Interfaces; 

namespace MinimalApi.Endpoints
{
    public static class UserEndpoints
    {
        public static void ConfigureUserEndpoints(this WebApplication app)
        {
            app.MapPost("/api/auth/login", Login)
                .WithName("Login")
                .Accepts<LoginRequestDTO>("application/json")
                .Produces<APIResponse>(200)
                .Produces(400)
                .AddEndpointFilter<BasicValidator<LoginRequestDTO>>()
                .AllowAnonymous(); 

            app.MapPost("/api/auth/register", Register)
                .WithName("Register")
                .Accepts<RegistrationRequestDTO>("application/json")
                .Produces<APIResponse>(200)
                .Produces<APIResponse>(400)
                .AddEndpointFilter<BasicValidator<RegistrationRequestDTO>>()
                .AllowAnonymous(); 

            app.MapGet("/api/auth/verify/{userid}/{email}", Verify)
                .WithName("Verify") 
                .Produces<APIResponse>(200)
                .Produces<APIResponse>(400) 
                .AllowAnonymous(); 

            app.MapPost("/api/auth/forgot-password", ForgotPassword)
                .WithName("ForgotPassword")
                .Accepts<ForgotPasswordRequestDTO>("application/json")
                .Produces<APIResponse>(200)
                .Produces<APIResponse>(400)
                .AllowAnonymous(); 

            app.MapPost("/api/auth/reset-password", ResetPassword)
                .WithName("ResetPassword")
                .Accepts<ResetPasswordRequestDTO>("application/json")
                .Produces<APIResponse>(200)
                .Produces<APIResponse>(400)
                .AllowAnonymous();
        } 

        private async static Task<IResult> Login(IUserService _userService, [FromBody] LoginRequestDTO model)
        {
            var result = await _userService.Login(model);
            if (!result.IsSuccess)
                return Results.BadRequest(result);

            return Results.Ok(result);
        }

        private async static Task<IResult> Register(IUserService _userService, [FromBody] RegistrationRequestDTO model)
        { 
            var result = await _userService.Register(model);

            if (!result.IsSuccess) 
                return Results.BadRequest(result);

            return Results.Ok(result);

        }

        private async static Task<IResult> Verify(IUserService _userService, string id, string code)
        {
            var result = await _userService.Verify(id, code);

            if (!result.IsSuccess)
                return Results.BadRequest(result);

            return Results.Ok(result);

        }

        private async static Task<IResult> ForgotPassword(IUserService _userService, ForgotPasswordRequestDTO forgotPasswordRequestDTO)
        {
            var result = await _userService.ForgotPassword(forgotPasswordRequestDTO);

            if (!result.IsSuccess)
                return Results.BadRequest(result);

            return Results.Ok(result); 
        }
          
        private async static Task<IResult> ResetPassword(IUserService _userService, ResetPasswordRequestDTO resetPasswordRequestDTO)
        {
            var result = await _userService.ResetPassword(resetPasswordRequestDTO);

            if (!result.IsSuccess)
                return Results.BadRequest(result);

            return Results.Ok(result);
        }
    }
}
