using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MinimalApi.Data.DTO
{
    public class ResetPasswordRequestDTO
    { 
        public string Email { get; set; } 
        public string Password { get; set; } 
        public string ConfirmPassword { get; set; } 
        public string Code { get; set; }
    }
}
