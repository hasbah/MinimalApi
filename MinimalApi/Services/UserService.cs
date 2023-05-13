using Microsoft.AspNetCore.Identity;
using MinimalApi.Data.Entity;
using MinimalApi.Data;
using MinimalApi.Services.Interfaces;
using MinimalApi.Data.DTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Core;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
#nullable disable
namespace MinimalApi.Services
{
    public class UserService: IUserService
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager; 
        private readonly ILogger<UserService> _logger;
        private readonly IEmailService _emailService;
        private string secretKey;
        private string url;


        public UserService(ApplicationDbContext db, 
            IMapper mapper, 
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager, 
            IEmailService emailService,
            ILogger<UserService> logger)
        {
            _db = db;
            _mapper = mapper;
            _emailService = emailService;
            _configuration = configuration;
            _userManager = userManager; 
            _logger = logger;
            secretKey = _configuration.GetValue<string>("ApiSettings:Secret");
            url = _configuration.GetValue<string>("ApiSettings:Url");
        } 
        public async Task<APIResponse> Register(RegistrationRequestDTO registerationRequestDTO)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            var applicationUser = _mapper.Map<ApplicationUser>(registerationRequestDTO);
            var result = await _userManager.CreateAsync(applicationUser, registerationRequestDTO.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(applicationUser, Constants.User);

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);

                var verificationUrl = $"{url}/auth/verify/{applicationUser.Id}/{code}";
                var emailBody = Constants.GetRegistrationEmailBody(verificationUrl);

                await _emailService
                    .SendEmailAsync(new() { applicationUser.Email }, "Confirm registration", emailBody);

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.Created;
                response.Result = _mapper.Map<UserDTO>(applicationUser);

                _logger.LogInformation($"User with email <{registerationRequestDTO.Email}> successfully created");
                return response;
            }

            _logger.LogInformation($"User with email <{registerationRequestDTO.Email}> did not create");
            return response;
        }
         
        public async Task<APIResponse> Verify(string id, string code)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            if (id is null)
            {
                _logger.LogInformation($"UserId '{id}' is null");

                response.ErrorMessages.Add("Invalid token");
                return response;
            }
            if (code is null)
            {
                _logger.LogInformation($"Code '{code}' is null");

                response.ErrorMessages.Add("Invalid token");
                return response;
            }

            var applicationUser = await _userManager.FindByIdAsync(id);
            if (applicationUser is null)
            {
                _logger.LogInformation($"User with id '{id}' not found");
                response.ErrorMessages.Add("Invalid token");
                return response;
            }

            var result = await _userManager.ConfirmEmailAsync(applicationUser, code);
            if (result.Succeeded)
            {
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;

                _logger.LogInformation($"User with email <{applicationUser.Email}> successfully verified");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogInformation($"User with email <{applicationUser.Email}> did not verify: {error.Description}");
                    response.ErrorMessages.Add(error.Description);
                }
            }

            return response;
        }

        public async Task<APIResponse> Login(LoginRequestDTO loginRequestDTO)
        { 
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
             
            var user = await _db.ApplicationUsers.SingleOrDefaultAsync(x => x.UserName == loginRequestDTO.Email);
            if (user is null)
            { 
                _logger.LogInformation($"User with email <{loginRequestDTO.Email}> not found");
                response.ErrorMessages.Add("Username or password is incorrect"); 
                return response;
            } 

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password); 
            if (!isValid)
            {
                _logger.LogInformation($"Password for user with email <{loginRequestDTO.Email}> is not valid");
                response.ErrorMessages.Add("Username or password is incorrect");
                return response;
            } 
            
            var roles = await _userManager.GetRolesAsync(user);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            { 
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Surname, user.Surname),
                    new Claim(ClaimTypes.Role, string.Join(", ", roles.Select(p=>p))),
                }),
                Expires = DateTime.UtcNow.AddDays(Constants.ExpireDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)  
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDTO loginResponseDTO = new()
            {
                User = _mapper.Map<UserDTO>(user),
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = loginResponseDTO; 

            _logger.LogInformation($"User with email '{loginRequestDTO.Email}' successfully logged in");
            return response; 
        }
         
        public async Task<APIResponse> ForgotPassword(ForgotPasswordRequestDTO forgotPasswordRequestDTO)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            var applicationUser = await _userManager.FindByEmailAsync(forgotPasswordRequestDTO.Email);
            if (applicationUser is null)
            { 
                response.ErrorMessages.Add("Invalid email");
                _logger.LogInformation($"User with email <{forgotPasswordRequestDTO.Email}> not found");

                return response;
            } 

            var code = await _userManager.GeneratePasswordResetTokenAsync(applicationUser);
            var resetPasswordUrl = $"{url}/auth/reset-passsword/{code}";
            var emailBody = Constants.GetForgotPasswordEmailBody(resetPasswordUrl); 
            
            var emailSendResult = await _emailService
                    .SendEmailAsync(new() { applicationUser.Email }, "Confirm registration", emailBody);

            if (!emailSendResult.IsSuccess)
            {
                response.ErrorMessages = emailSendResult.ErrorMessage; 
            }
            else
            {  
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;

                _logger.LogInformation($"Reset password email successfully sent to user with email <{forgotPasswordRequestDTO.Email}>");
            }
            return response;
        }

        public async Task<APIResponse> ResetPassword(ResetPasswordRequestDTO resetPasswordRequestDTO)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
            var applicationUser = await _userManager.FindByEmailAsync(resetPasswordRequestDTO.Email);
            if (applicationUser is null)
            {
                response.ErrorMessages.Add("Invalid email");
                _logger.LogInformation($"User with email <{resetPasswordRequestDTO.Email}> not found");

                return response;
            }

            var result = await _userManager.ResetPasswordAsync(applicationUser, resetPasswordRequestDTO.Code, resetPasswordRequestDTO.Password);
            if (result.Succeeded)
            { 
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;

                _logger.LogInformation($"Password successfully reset for user <{resetPasswordRequestDTO.Email}>");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    response.ErrorMessages.Add(error.Description);
                    _logger.LogInformation($"Password for user with email <{applicationUser.Email}> did not reset: {error.Description}");
                }
            }

            return response;
        }
         
        public async Task<bool> IsUniqueEmail(string email)
        {
            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(x => x.Email == email);
            if (user is null)
            {
                _logger.LogInformation($"Email '{email}' exists in data base");
                return true;
            }

            _logger.LogInformation($"Email '{email}' does not exists in data base");
            return false;
        }
    }
}
#nullable enable